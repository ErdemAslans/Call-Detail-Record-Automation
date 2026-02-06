from typing import List
from helpers.logger import main_logger as logger
# from models.incomingCalls import IncomingCalls
from models.cdrModel import CdrModel
from pymongo import MongoClient
from pydantic import ValidationError
from helpers.config import load_config
from helpers.converters import parse_csv_to_model

config = load_config()

# Singleton MongoDB client (connection pooling)
_mongo_client = None


def get_mongo_client():
    """Get or create MongoDB client with connection pooling"""
    global _mongo_client
    if _mongo_client is None:
        _mongo_client = MongoClient(
            config['mongo']['uri'],
            maxPoolSize=50,
            minPoolSize=10,
            maxIdleTimeMS=30000,
            serverSelectionTimeoutMS=5000,
            connectTimeoutMS=10000,
            retryWrites=True
        )
        logger.info("MongoDB client initialized with connection pool (maxPoolSize=50)")
    return _mongo_client


def close_mongo_client():
    """Close MongoDB client (graceful shutdown için)"""
    global _mongo_client
    if _mongo_client is not None:
        _mongo_client.close()
        _mongo_client = None
        logger.info("MongoDB client closed")


def get_mongo_collection():
    """Get the MongoDB collection for incoming calls."""
    client = get_mongo_client()
    db = client[config['mongo']['database']]
    return db[config['mongo']['collection']]


async def insert_to_mongo(collection, row, users_collection): 
    try:
        record = parse_csv_to_model(row, users_collection)
        collection.insert_one(record.model_dump())
    except ValidationError as e:
        logger.error(f"Validation error for row {row}: {e}")
    except Exception as e:
        logger.error(f"Error inserting record for row {row}: {e}")