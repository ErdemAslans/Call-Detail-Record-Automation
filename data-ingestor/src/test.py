import pyodbc
from helpers.config import load_config
from helpers.logger import main_logger as logger

config = load_config()

connection_string = (
    f"DRIVER={{ODBC Driver 18 for SQL Server}};"
    f"SERVER={config['mssql']['server']};"
    f"DATABASE={config['mssql']['database']};"
    f"UID={config['mssql']['user']};"
    f"PWD={config['mssql']['password']};"
    f"TrustServerCertificate=yes;"
)

try:
    connection = pyodbc.connect(connection_string)
    print("Connection successful!")
    connection.close()
except pyodbc.Error as ex:
    sqlstate = ex.args[1]
    print(f"Connection failed: {sqlstate}")