import os
import csv
import asyncio
import time
from datetime import datetime
from helpers.logger import main_logger as logger
from users import get_unique_phone_numbers
from utils import get_mongo_collection, insert_to_mongo
from create_collection import create_collection_if_not_exists
from helpers.file_manager import FileManager

class CDRProcessor:
    def __init__(self, source_dir: str = "E:\\CDR", processed_dir: str = "E:\\CDR_Processed"):
        self.file_manager = FileManager(source_dir, processed_dir)
        self.collection = None
        self.users_collection = None
        self._should_exit = False  # Graceful shutdown flag
        
    def request_shutdown(self):
        """Service tarafından shutdown isteği"""
        logger.info("CDRProcessor received shutdown request")
        self._should_exit = True
        
    async def initialize(self):
        """Sistem başlatma"""
        logger.info("CDR Processor initializing...")
        
        await create_collection_if_not_exists()
        self.collection = get_mongo_collection()
        self.users_collection = get_unique_phone_numbers()
        
        # Başlangıç istatistikleri
        stats = self.file_manager.get_processing_stats()
        logger.info(f"System initialized. Stats: {stats}")
        
    async def process_single_file(self, file_path: str) -> dict:
        """Tek dosyayı işle"""
        start_time = time.time()
        result = {
            'success': False,
            'file_path': file_path,
            'filename': os.path.basename(file_path),
            'records_processed': 0,
            'processing_time': 0,
            'error': None
        }
        
        locked_file_path = None
        
        try:
            # Dosyayı işleme için kilitle
            locked_file_path = self.file_manager.lock_file_for_processing(file_path)
            
            # CSV dosyasını oku ve işle
            records_count = 0
            batch_tasks = []
            
            with open(locked_file_path, mode='r', encoding='utf-8-sig') as file:
                csv_reader = csv.DictReader(file)

                # NOT: DictReader zaten ilk satırı header olarak kullanır
                # next() çağrısı KALDIRILDI - aksi halde ilk veri satırı kaybolur!

                # Her satırı işle
                for row_num, row in enumerate(csv_reader, 1):
                    try:
                        batch_tasks.append(insert_to_mongo(self.collection, row, self.users_collection))
                        records_count += 1
                        
                        # Her 1000 kayıtta bir batch işle (memory management)
                        if len(batch_tasks) >= 1000:
                            await asyncio.gather(*batch_tasks)
                            batch_tasks = []
                            
                    except Exception as row_error:
                        logger.error(f"Error processing row {row_num} in {result['filename']}: {row_error}")
                        # Tek satır hatası tüm dosyayı durdurmaz
                
                # Son batch'i işle
                if batch_tasks:
                    await asyncio.gather(*batch_tasks)
            
            # Başarılı tamamlama
            if records_count > 0:
                success = self.file_manager.move_to_processed(locked_file_path, records_count)
                if success:
                    result['success'] = True
                    result['records_processed'] = records_count
                    result['processing_time'] = time.time() - start_time
                else:
                    result['error'] = "Failed to move file to processed"
            else:
                result['error'] = "No records processed"
                self.file_manager.unlock_file_on_failure(locked_file_path)
            
        except Exception as e:
            result['error'] = str(e)
            logger.error(f"Error processing file {result['filename']}: {e}")
            
            # Hata durumunda dosyayı unlock et
            if locked_file_path:
                self.file_manager.unlock_file_on_failure(locked_file_path)
        
        result['processing_time'] = time.time() - start_time
        return result
    
    async def process_available_files(self):
        """Mevcut tüm dosyaları işle"""
        start_time = time.time()
        
        # İşlenmeye hazır dosyaları al
        available_files = self.file_manager.get_available_files()
        
        if not available_files:
            logger.debug("No files available for processing")
            return
        
        logger.info(f"Processing {len(available_files)} files...")
        
        # İstatistikler
        total_files = len(available_files)
        successful_files = 0
        failed_files = 0
        total_records = 0
        
        # Her dosyayı sırayla işle
        for file_path in available_files:
            result = await self.process_single_file(file_path)
            
            if result['success']:
                successful_files += 1
                total_records += result['records_processed']
                logger.info(f"✓ Processed: {result['filename']} ({result['records_processed']} records, {result['processing_time']:.2f}s)")
            else:
                failed_files += 1
                logger.error(f"✗ Failed: {result['filename']} - {result['error']}")
        
        # Özet
        total_time = time.time() - start_time
        logger.info(f"Batch completed: {successful_files}/{total_files} files successful, {total_records} records, {total_time:.2f}s")
        
        # İstatistikleri güncelle
        stats = self.file_manager.get_processing_stats()
        logger.info(f"Current stats: {stats}")
    
    async def run_periodic_processing(self, interval: int = 120):
        """2 dakikada bir dosyaları kontrol et ve işle (graceful shutdown destekli)"""
        logger.info(f"Starting periodic processing every {interval} seconds")
        
        while not self._should_exit:
            try:
                logger.info("=== Starting periodic file processing ===")
                await self.process_available_files()
                logger.info(f"=== Processing completed. Next run in {interval} seconds ===")
                
            except Exception as e:
                logger.error(f"Error in periodic processing: {e}")
            
            # Bir sonraki çalışmaya kadar bekle (shutdown sinyalini hızlı yakala)
            remaining = interval
            while remaining > 0 and not self._should_exit:
                await asyncio.sleep(min(5, remaining))  # 5 saniyede bir kontrol
                remaining -= 5
        
        logger.info("Periodic processing stopped due to shutdown request")
