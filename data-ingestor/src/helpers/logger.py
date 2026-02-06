# src/logger.py
import logging
import os
from pymongo import MongoClient
import traceback
from helpers.config import load_config

config = load_config()

# Log dizini (Windows Service için)
LOG_DIR = os.environ.get('CDR_LOG_DIR', 'E:\\CDR_Logs')


class MongoHandler(logging.Handler):
    def __init__(self, db_name, collection_name, level=logging.NOTSET):
        super().__init__(level)
        self.client = MongoClient(config['mongo']['uri'])
        self.db = self.client[db_name]
        self.collection = self.db[collection_name]

    def emit(self, record):
        try:
            log_entry = self.format(record)
            log_data = {
                "timestamp": record.asctime,
                "level": record.levelname,
                "message": record.message,
                "filename": record.pathname,
                "line": record.lineno,
                "function": record.funcName,
                "exception": None
            }
            if record.exc_info:
                log_data["exception"] = ''.join(traceback.format_exception(*record.exc_info))
            self.collection.insert_one(log_data)
        except Exception:
            # MongoDB bağlantı hatası olsa bile service crash etmesin
            pass


class SafeFileHandler(logging.FileHandler):
    """Dizin yoksa oluşturan güvenli file handler"""
    def __init__(self, filename, mode='a', encoding='utf-8', delay=False):
        # Dizin yoksa oluştur
        log_dir = os.path.dirname(filename)
        if log_dir and not os.path.exists(log_dir):
            try:
                os.makedirs(log_dir, exist_ok=True)
            except Exception:
                pass
        super().__init__(filename, mode, encoding, delay)


def setup_logger(name, level=logging.INFO):
    """Function to setup a logger; can be called from any module."""
    formatter = logging.Formatter(
        '%(asctime)s - %(levelname)s - [%(filename)s:%(lineno)d] - %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S'
    )
    
    logger = logging.getLogger(name)
    logger.setLevel(level)
    
    # Duplicate handler eklemeyi önle
    if logger.handlers:
        return logger
    
    # 1. MongoHandler - Veritabanına log
    try:
        mongo_handler = MongoHandler(
            db_name=config['mongo']['database'], 
            collection_name=config['mongo']['log_collection']
        )
        mongo_handler.setFormatter(formatter)
        mongo_handler.setLevel(logging.INFO)
        logger.addHandler(mongo_handler)
    except Exception as e:
        print(f"Warning: Could not setup MongoDB logging: {e}")
    
    # 2. FileHandler - Dosyaya log (Windows Service debugging için)
    try:
        log_file = os.path.join(LOG_DIR, 'cdr_ingestor.log')
        file_handler = SafeFileHandler(log_file)
        file_handler.setFormatter(formatter)
        file_handler.setLevel(logging.DEBUG)
        logger.addHandler(file_handler)
    except Exception as e:
        print(f"Warning: Could not setup file logging: {e}")
    
    # 3. ConsoleHandler - Terminal/stdout log (NSSM stdout capture için)
    console_handler = logging.StreamHandler()
    console_handler.setFormatter(formatter)
    console_handler.setLevel(logging.INFO)
    logger.addHandler(console_handler)
    
    return logger

# Ana logger'ı oluşturun
main_logger = setup_logger('main_logger')