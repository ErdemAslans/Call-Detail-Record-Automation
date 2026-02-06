import os
import pandas as pd
from pymongo import MongoClient
from helpers.config import load_config

config = load_config()

def excel_to_json(excel_file_path):
    df = pd.read_excel(excel_file_path)
    df.columns = [col.lower() for col in df.columns]  # Convert column names to lowercase
    return df.to_dict(orient='records')

def get_mongo_users_collection():
    """Get the MongoDB collection for users."""
    client = MongoClient(config['mongo']['uri'])
    db = client[config['mongo']['database']]
    return db['users']

def insert_json_to_mongo(json_data):
    collection = get_mongo_users_collection()
    collection.insert_many(json_data)

def get_unique_phone_numbers():
    """Return a list of unique phone numbers from the MongoDB collection."""
    collection = get_mongo_users_collection()
    phone_numbers = collection.distinct("phone_number")
    return phone_numbers

if __name__ == "__main__":
    import sys
    
    script_dir = os.path.dirname(os.path.abspath(__file__))
    excel_file_path = os.path.join(script_dir, '..', 'data/users/Otomotiv Dahili Listesi.xlsx')
    # excel_file_path = '/home/anarion/Projects/CDR.DataIngestor/src/models/Otomotiv Dahili Listesi.xlsx' if len(sys.argv) == 1 else sys.argv[1]
    
    if not os.path.exists(excel_file_path):
        print(f"Error: The file '{excel_file_path}' does not exist.")
        sys.exit(1)
    
    json_data = excel_to_json(excel_file_path)
    insert_json_to_mongo(json_data)
    print("Data inserted successfully.")
    