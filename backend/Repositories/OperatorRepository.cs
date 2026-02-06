using Cdr.Api.Context;
using Cdr.Api.Helpers;
using Cdr.Api.Interfaces;
using Cdr.Api.Models.Entities;
using Cdr.Api.Models.Response.UserStatistics;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cdr.Api.Repositories;

public class OperatorRepository : ReadonlyMongoRepository<Operator>, IOperatorRepository
{
    public OperatorRepository(MongoDbContext context, IOptions<MongoDbSettings> mongoDbSettings)
        : base(context, mongoDbSettings.Value.OperatorCollectionName)
    {
    }

    public async Task<OperatorInfo> GetUserInfoAsync(string phoneNumber)
    {
        var pipeline = new BsonDocument[]
        {
        new BsonDocument("$match", new BsonDocument("phone_number", phoneNumber)),
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "departments" },
            { "localField", "department_id" },
            { "foreignField", "_id" },
            { "as", "department" }
        }),
        new BsonDocument("$unwind", "$department"),
        new BsonDocument("$project", new BsonDocument
        {
            { "name", 1 },
            { "title", 1 },
            { "phone_number", 1 },
            { "department_name", "$department.name" }
        })
        };

        var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();

        if (result == null)
        {
            return null;
        }

        return new OperatorInfo
        {
            Name = result["name"].AsString,
            Department = result["department_name"].AsString,
            Position = result["title"].AsString,
            Number = result["phone_number"].AsString
        };
    }

    public async Task<List<OperatorInfo>> GetAllUsersInfoAsync()
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "departments" },
                { "localField", "department_id" },
                { "foreignField", "_id" },
                { "as", "department" }
            }),
            new BsonDocument("$unwind", "$department"),
            new BsonDocument("$project", new BsonDocument
            {
                { "name", 1 },
                { "title", 1 },
                { "phone_number", 1 },
                { "department_name", "$department.name" }
            })
        };

        var results = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();

        return results.Select(result => new OperatorInfo
        {
            Name = result["name"].AsString,
            Department = result["department_name"].AsString,
            Position = result["title"].AsString,
            Number = result["phone_number"].AsString
        }).GroupBy(info => info.Number)
          .Select(group => group.First())
          .ToList();
    }

    public async Task<Operator> GetUserByUsernameAsync(string username)
    {
        return await _collection.Find(op => op.PhoneNumber == username).FirstOrDefaultAsync();
    }
}
