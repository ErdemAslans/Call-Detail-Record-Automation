#!/usr/bin/env python3
"""
CDR DataIngestor Test Processor - Sistem Python
"""

import sys
import os
from pathlib import Path

print(f"🐍 Python Yolu: {sys.executable}")
print(f"🐍 Python Sürümü: {sys.version.split()[0]}")

# src klasörünü path'e ekle
project_root = Path(__file__).parent
src_path = project_root / "src"
sys.path.insert(0, str(src_path))

from processors.cdr_processor import CDRProcessor
import asyncio

async def test():
    print("CDR Processor Test Başlıyor...")
    try:
        processor = CDRProcessor('E:/CDR', 'E:/CDR_Processed')
        await processor.initialize()
        
        stats = processor.file_manager.get_processing_stats()
        print('✓ Processor başarıyla başlatıldı')
        print(f'Kaynak klasör: {stats["source_dir"]}')
        print(f'İşlenmiş klasör: {stats["processed_dir"]}')
        print(f'Bekleyen dosyalar: {stats["pending_files"]}')
        print(f'İşlenen dosyalar: {stats["processing_files"]}')
        print(f'Tamamlanmış dosyalar: {stats["processed_files"]}')
        
        # Bir test dosya işleme döngüsü
        print("Test dosya taraması yapılıyor...")
        await processor.process_available_files()
        print("✓ Test tamamlandı!")
        
    except Exception as e:
        print(f"✗ Test hatası: {e}")

if __name__ == "__main__":
    asyncio.run(test())
