import asyncio
import signal
import sys
from helpers.logger import main_logger as logger
from processors.cdr_processor import CDRProcessor


class ServiceState:
    """Service durumu ve graceful shutdown kontrolü"""
    def __init__(self):
        self.should_exit = False
        self.processor = None

    def request_shutdown(self):
        """Shutdown sinyali al"""
        self.should_exit = True
        if self.processor:
            self.processor.request_shutdown()


# Global service state
service_state = ServiceState()


def signal_handler(signum, frame):
    """SIGTERM/SIGINT signal handler for Windows Service"""
    signal_name = signal.Signals(signum).name if hasattr(signal, 'Signals') else str(signum)
    logger.info(f"Received signal {signal_name}, initiating graceful shutdown...")
    service_state.request_shutdown()


def setup_signal_handlers():
    """Signal handler'ları kur (Windows ve Linux için)"""
    signal.signal(signal.SIGINT, signal_handler)
    signal.signal(signal.SIGTERM, signal_handler)
    
    # Windows-specific: SIGBREAK
    if sys.platform == 'win32':
        signal.signal(signal.SIGBREAK, signal_handler)


async def main():
    """Ana fonksiyon - Yeni dosya taşıma sistemini başlat"""
    logger.info("=" * 60)
    logger.info("CDR DataIngestor Service starting...")
    logger.info("=" * 60)
    
    # Signal handler'ları kur
    setup_signal_handlers()
    
    # Test ortamı için data klasörünü kullan, production için E:\CDR
    # source_dir = os.path.join(os.path.dirname(__file__), '..', 'data')  # Test için
    source_dir = "E:\\CDR"  # Production için
    processed_dir = "E:\\CDR_Processed"
    
    processor = CDRProcessor(source_dir, processed_dir)
    service_state.processor = processor
    
    try:
        await processor.initialize()
        logger.info(f"System initialized. Source: {source_dir}, Processed: {processed_dir}")
        
        # 2 dakikada bir çalış (graceful shutdown destekli)
        await processor.run_periodic_processing(interval=120)
        
    except asyncio.CancelledError:
        logger.info("Service tasks cancelled")
    except KeyboardInterrupt:
        logger.info("Process interrupted by user (Ctrl+C)")
    except Exception as e:
        logger.error(f"Fatal error: {e}")
        raise
    finally:
        logger.info("=" * 60)
        logger.info("CDR DataIngestor Service stopped")
        logger.info("=" * 60)


if __name__ == '__main__':
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        logger.info("Service terminated by user")
    except Exception as e:
        logger.error(f"Service crashed: {e}")
        sys.exit(1)