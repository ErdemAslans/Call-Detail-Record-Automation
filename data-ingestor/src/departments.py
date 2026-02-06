
import os
import pandas as pd
import numpy as np
from pymongo import MongoClient
from helpers.config import load_config

config = load_config()

def is_valid_department(department):
    """Departman değerinin geçerli olup olmadığını kontrol et."""
    # None, NaN, boş kontrolleri
    if pd.isna(department) or department is None:
        return False
    
    # String'e çevir ve kontrol et
    dept_str = str(department).strip()
    
    # Boş string kontrolleri
    if not dept_str or dept_str == '':
        return False
    
    # NaN string kontrolleri (case insensitive)
    invalid_values = ['nan', 'null', 'none', '#n/a', '#ref!', '#value!', '#name?', '#div/0!']
    if dept_str.lower() in invalid_values:
        return False
    
    # Sadece sayı mı kontrol et (Excel'de yanlışlıkla sayı girilmişse)
    try:
        float(dept_str)
        # Eğer sayıysa ve makul bir departman adı değilse geçersiz
        if len(dept_str) < 3:  # Çok kısa sayılar muhtemelen yanlış
            return False
    except ValueError:
        pass  # Sayı değilse sorun yok
    
    return True

def clean_department_name(department):
    """Departman adını temizle ve standartlaştır."""
    if not is_valid_department(department):
        return "Tanımsız Departman"
    
    # String'e çevir, baş/sondaki boşlukları temizle
    clean_name = str(department).strip()
    
    # Çoklu boşlukları tek boşluğa çevir
    clean_name = ' '.join(clean_name.split())
    
    return clean_name

def excel_to_dataframe(excel_file_path):
    df = pd.read_excel(excel_file_path)
    df.columns = ['name', 'title', 'department', 'phone_number']
    
    # Department sütununu temizle
    df['department'] = df['department'].apply(clean_department_name)
    
    return df

def get_mongo_collections():
    client = MongoClient(config['mongo']['uri'])
    db = client[config['mongo']['database']]
    return db['users'], db['departments']

def insert_departments(df, departments_collection):
    unique_departments = df['department'].unique()
    print(f"Bulunan departmanlar: {unique_departments}")
    
    for department in unique_departments:
        # Zaten temizlenmiş departmanlar geldiği için sadece var mı kontrol et
        if not departments_collection.find_one({"name": department}):
            departments_collection.insert_one({"name": department})
            print(f"Departman eklendi: {department}")

def insert_or_update_users(df, users_collection, departments_collection):
    for _, row in df.iterrows():
        if row['phone_number'] == 'Bulunamadı':
            continue
        department = departments_collection.find_one({"name": row['department']})
        if department:
            users_collection.update_one(
                {"phone_number": row['phone_number']},
                {
                    "$set": {
                        "name": str(row['name']).strip(),
                        "title": str(row['title']).strip(),
                        "phone_number": str(row['phone_number']),
                        "department_id": department['_id']
                    }
                },
                upsert=True
            )

if __name__ == "__main__":
    script_dir = os.path.dirname(os.path.abspath(__file__))
    excel_file_path = os.path.join(script_dir, '..', 'data/users/Otomotiv_Personel_Listesi.xlsx')
    df = excel_to_dataframe(excel_file_path)
    users_collection, departments_collection = get_mongo_collections()
    insert_departments(df, departments_collection)
    insert_or_update_users(df, users_collection, departments_collection)
    print("Data inserted successfully.")