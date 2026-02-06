import yaml
import os

def load_config(config_path=None):
    if config_path is None:
        script_dir = os.path.dirname(os.path.abspath(__file__))
        config_path = os.path.join(script_dir, '..', 'config.yaml')
    
    with open(config_path, 'r') as file:
        config = yaml.safe_load(file)
    
    # Environment variable override (future flexibility için)
    # MONGO_URI environment variable set edilmişse config.yaml değerini override et
    mongo_uri_env = os.environ.get('MONGO_URI')
    if mongo_uri_env:
        config['mongo']['uri'] = mongo_uri_env
    
    # Database adı override
    mongo_db_env = os.environ.get('MONGO_DATABASE')
    if mongo_db_env:
        config['mongo']['database'] = mongo_db_env
    
    return config