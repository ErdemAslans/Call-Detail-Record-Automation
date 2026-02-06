import win32serviceutil
import win32service
import win32event
import servicemanager
import sys
import os
import asyncio
from pathlib import Path

# Project path'ini ekle
PROJECT_DIR = Path(__file__).parent.parent
sys.path.insert(0, str(PROJECT_DIR / 'src'))

from processors.cdr_processor import CDRProcessor

class CDRService(win32serviceutil.ServiceFramework):
    _svc_name_ = 'CDRDataIngestor'
    _svc_display_name_ = 'CDR Data Ingestor Service'
    _svc_description_ = 'CDR dosyalarını 2 dakikada bir işleyen servis'

    def __init__(self, args):
        win32serviceutil.ServiceFramework.__init__(self, args)
        self.hWaitStop = win32event.CreateEvent(None, 0, 0, None)
        self.stop_requested = False

    def SvcStop(self):
        servicemanager.LogInfoMsg('CDR Service durduruluyor...')
        self.ReportServiceStatus(win32service.SERVICE_STOP_PENDING)
        self.stop_requested = True
        win32event.SetEvent(self.hWaitStop)

    def SvcDoRun(self):
        servicemanager.LogInfoMsg('CDR Service başlatıldı')
        try:
            asyncio.run(self.main_loop())
        except Exception as e:
            servicemanager.LogErrorMsg(f'Service hatası: {e}')

    async def main_loop(self):
        processor = CDRProcessor('E:\\CDR', 'E:\\CDR_Processed')
        await processor.initialize()
        servicemanager.LogInfoMsg('CDR Processor hazır')
        
        while not self.stop_requested:
            try:
                await processor.process_available_files()
                # 2 dakika bekle
                for i in range(24):  # 24 x 5 = 120 saniye
                    if self.stop_requested:
                        break
                    await asyncio.sleep(5)
            except Exception as e:
                servicemanager.LogErrorMsg(f'İşlem hatası: {e}')
                await asyncio.sleep(30)

if __name__ == '__main__':
    win32serviceutil.HandleCommandLine(CDRService)
