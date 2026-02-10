using Cdr.Api.Context;
using Cdr.Api.Entities;
using Cdr.Api.Helpers;
using Cdr.Api.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Cdr.Api.Repositories;

public class DepartmentRepository : ReadonlyMongoRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(MongoDbContext context, IOptions<MongoDbSettings> mongoDbSettings)
        : base(context, mongoDbSettings.Value.DepartmentCollectionName)
    {
    }
}
