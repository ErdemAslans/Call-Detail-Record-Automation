using MongoDB.Driver;
using Cdr.Api.Entities.Cdr;

namespace Cdr.Api.Context;

public class MongoDbContext
{
    private readonly IConfiguration _configuration;
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        var client = new MongoClient(configuration.GetConnectionString("MongoDbConnection"));
        _database = client.GetDatabase(configuration["MongoDb:DatabaseName"]);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}