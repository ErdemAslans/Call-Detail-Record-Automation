import os
import shutil
import time
from datetime import datetime
from helpers.logger import main_logger as logger
from typing import List

class FileManager:
    def __init__(self, source_dir: str = "E:\\CDR", processed_dir: str = "E:\\CDR_Processed"):
        self.source_dir = source_dir
        self.processed_dir = processed_dir
        self.processing_extension = ".processing"
        
        # Klasörleri oluştur
        self._ensure_directories()
    
    def _ensure_directories(self):
        """Gerekli klasörleri oluştur"""
        try:
            os.makedirs(self.source_dir, exist_ok=True)
            os.makedirs(self.processed_dir, exist_ok=True)
            
            # Tarih bazlı alt klasörler
            today = datetime.now()
            processed_today = os.path.join(self.processed_dir, today.strftime("%Y"), today.strftime("%m"))
            os.makedirs(processed_today, exist_ok=True)
            
            logger.info(f"Directories ensured: {self.source_dir}, {processed_today}")
        except Exception as e:
            logger.error(f"Error creating directories: {e}")
            raise
    
    def get_available_files(self) -> List[str]:
        """İşlenmeye hazır dosyaları getir - CDR dosyaları uzantısız"""
        available_files = []
        
        if not os.path.exists(self.source_dir):
            logger.warning(f"Source directory does not exist: {self.source_dir}")
            return available_files
        
        try:
            # CDR dosyaları uzantısız - sadece .processing ve gizli dosyaları hariç tut
            all_files = [f for f in os.listdir(self.source_dir) 
                        if not f.endswith(self.processing_extension) 
                        and not f.startswith('.') 
                        and os.path.isfile(os.path.join(self.source_dir, f))
                        and f.startswith('cdr_')]  # CDR dosyalarını filtrele
            
            logger.info(f"Found {len(all_files)} CDR files in source directory")
            
            for filename in all_files:
                file_path = os.path.join(self.source_dir, filename)
                
                # Dosya concurrent access kontrolü
                if self._is_file_ready_for_processing(file_path):
                    available_files.append(file_path)
                else:
                    logger.debug(f"File not ready for processing: {filename}")
            
            logger.info(f"Found {len(available_files)} files ready for processing")
            
        except Exception as e:
            logger.error(f"Error scanning source directory: {e}")
        
        return available_files
    
    def _is_file_ready_for_processing(self, file_path: str, stable_duration: int = 60) -> bool:
        """
        Dosyanın işlenmeye hazır olup olmadığını kontrol et
        stable_duration saniye boyunca dosya boyutu değişmemişse hazır kabul et
        """
        try:
            if not os.path.exists(file_path):
                return False
            
            # İlk boyutu al
            initial_stat = os.stat(file_path)
            initial_size = initial_stat.st_size
            initial_mtime = initial_stat.st_mtime
            
            # Dosya çok küçükse henüz yazılıyor olabilir
            if initial_size < 100:  # 100 byte altı
                return False
            
            # Son değiştirilme zamanından şu ana kadar geçen süre
            current_time = time.time()
            time_since_modification = current_time - initial_mtime
            
            # 1 dakikadan az önce değiştirilmişse bekle
            if time_since_modification < stable_duration:
                logger.debug(f"File modified recently, waiting: {os.path.basename(file_path)}")
                return False
            
            # Dosya stabil görünüyor
            return True
            
        except Exception as e:
            logger.warning(f"Error checking file stability {file_path}: {e}")
            return False
    
    def lock_file_for_processing(self, file_path: str) -> str:
        """
        Dosyayı işleme için kilitle (.processing uzantısı ekle)
        Returns: Yeni dosya yolu
        """
        try:
            locked_file_path = file_path + self.processing_extension
            
            # Dosyayı yeniden adlandır (atomic operation)
            os.rename(file_path, locked_file_path)
            
            logger.info(f"File locked for processing: {os.path.basename(file_path)}")
            return locked_file_path
            
        except Exception as e:
            logger.error(f"Error locking file {file_path}: {e}")
            raise
    
    def unlock_file_on_failure(self, locked_file_path: str):
        """
        İşlem başarısız olursa dosyayı unlock et (orijinal adına döndür)
        """
        try:
            if not locked_file_path.endswith(self.processing_extension):
                logger.warning(f"File is not locked: {locked_file_path}")
                return
            
            original_file_path = locked_file_path[:-len(self.processing_extension)]
            os.rename(locked_file_path, original_file_path)
            
            logger.warning(f"File unlocked due to processing failure: {os.path.basename(original_file_path)}")
            
        except Exception as e:
            logger.error(f"Error unlocking file {locked_file_path}: {e}")
    
    def move_to_processed(self, locked_file_path: str, records_count: int = 0) -> bool:
        """
        İşlem başarılı olduktan sonra dosyayı processed klasörüne taşı
        """
        try:
            if not locked_file_path.endswith(self.processing_extension):
                logger.error(f"File is not locked: {locked_file_path}")
                return False
            
            # Orijinal dosya adını al
            original_filename = os.path.basename(locked_file_path[:-len(self.processing_extension)])
            
            # Hedef klasörü oluştur (tarih bazlı)
            today = datetime.now()
            target_dir = os.path.join(self.processed_dir, today.strftime("%Y"), today.strftime("%m"))
            os.makedirs(target_dir, exist_ok=True)
            
            # Hedef dosya yolu
            target_file_path = os.path.join(target_dir, original_filename)
            
            # Aynı isimde dosya varsa timestamp ekle
            if os.path.exists(target_file_path):
                timestamp = datetime.now().strftime("_%H%M%S")
                name, ext = os.path.splitext(original_filename)
                target_file_path = os.path.join(target_dir, f"{name}{timestamp}{ext}")
            
            # Dosyayı taşı
            shutil.move(locked_file_path, target_file_path)
            
            logger.info(f"File moved to processed: {original_filename} ({records_count} records)")
            return True
            
        except Exception as e:
            logger.error(f"Error moving file to processed {locked_file_path}: {e}")
            # Hata durumunda unlock et
            self.unlock_file_on_failure(locked_file_path)
            return False
    
    def get_processing_stats(self) -> dict:
        """İşleme istatistiklerini döndür"""
        try:
            source_count = len([f for f in os.listdir(self.source_dir) 
                              if f.lower().endswith('.csv') and not f.endswith(self.processing_extension)]) \
                              if os.path.exists(self.source_dir) else 0
            
            processing_count = len([f for f in os.listdir(self.source_dir) 
                                  if f.endswith(self.processing_extension)]) \
                                  if os.path.exists(self.source_dir) else 0
            
            # Processed klasörü toplam dosya sayısı
            processed_count = 0
            if os.path.exists(self.processed_dir):
                for root, dirs, files in os.walk(self.processed_dir):
                    processed_count += len([f for f in files if f.lower().endswith('.csv')])
            
            return {
                'pending_files': source_count,
                'processing_files': processing_count,
                'processed_files': processed_count,
                'source_dir': self.source_dir,
                'processed_dir': self.processed_dir
            }
            
        except Exception as e:
            logger.error(f"Error getting processing stats: {e}")
            return {}
