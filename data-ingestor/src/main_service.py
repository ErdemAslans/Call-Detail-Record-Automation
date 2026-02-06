import asyncio
import signal
import sys
from helpers.logger import main_logger as logger
from processors.cdr_processor import CDRProcessor

class CDRService:
    def __init__(self):
        self.processor = None
        self.running = True
        
    def signal_handler(self, signum, frame):
        """Graceful shutdown için signal handler"""
        logger.info(f"Received signal {signum}, shutting down gracefully...")
        self.running = False
        
    async def run(self):
        """Ana servis çalışma fonksiyonu"""
        logger.info("CDR DataIngestor Service starting...")
        
        # Production paths
        source_dir = "E:\\CDR"
        processed_dir = "E:\\CDR_Processed"
        
        self.processor = CDRProcessor(source_dir, processed_dir)
        
        try:
            # System'i initialize et
            await self.processor.initialize()
            logger.info(f"Service initialized successfully")
            logger.info(f"Source Directory: {source_dir}")
            logger.info(f"Processed Directory: {processed_dir}")
            
            # Ana loop - 2 dakikada bir çalış
            while self.running:
                try:
                    logger.info("=== Starting periodic file processing ===")
                    await self.processor.process_available_files()
                    logger.info("=== Processing completed ===")
                    
                    # 2 dakika bekle (120 saniye)
                    for i in range(120):
                        if not self.running:
                            break
                        await asyncio.sleep(1)
                        
                except Exception as e:
                    logger.error(f"Error in processing cycle: {e}")
                    # Hata durumunda 30 saniye bekle ve devam et
                    for i in range(30):
                        if not self.running:
                            break
                        await asyncio.sleep(1)
                    
        except Exception as e:
            logger.error(f"Fatal error in service: {e}")
            raise
        finally:
            logger.info("CDR DataIngestor Service stopped")

async def main():
    """Ana fonksiyon"""
    service = CDRService()
    
    # Signal handlers (Ctrl+C için)
    signal.signal(signal.SIGINT, service.signal_handler)
    signal.signal(signal.SIGTERM, service.signal_handler)
    
    try:
        await service.run()
    except KeyboardInterrupt:
        logger.info("Service interrupted by user")
    except Exception as e:
        logger.error(f"Service failed with error: {e}")
        sys.exit(1)

if __name__ == '__main__':
    asyncio.run(main())
