import os
import asyncio
from helpers.logger import main_logger as logger
from processors.cdr_processor import CDRProcessor

async def main():
    """Test/Development ortamı için ana fonksiyon"""
    logger.info("CDR DataIngestor starting in DEVELOPMENT mode...")
    
    # Test ortamı için data klasörünü kullan
    script_dir = os.path.dirname(__file__)
    source_dir = os.path.join(script_dir, '..', 'data')
    processed_dir = os.path.join(script_dir, '..', 'data_processed')
    
    processor = CDRProcessor(source_dir, processed_dir)
    
    try:
        await processor.initialize()
        logger.info(f"Development system initialized.")
        logger.info(f"Source: {os.path.abspath(source_dir)}")
        logger.info(f"Processed: {os.path.abspath(processed_dir)}")
        
        # Test için 30 saniyede bir çalış
        await processor.run_periodic_processing(interval=30)
        
    except KeyboardInterrupt:
        logger.info("Development process interrupted by user")
    except Exception as e:
        logger.error(f"Fatal error in development: {e}")
        raise

if __name__ == '__main__':
    asyncio.run(main())
