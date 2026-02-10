using System.Linq.Expressions;
using Cdr.Api.Context;
using Cdr.Api.Interfaces;
using MongoDB.Driver;

namespace Cdr.Api.Repositories;

public class ReadonlyMongoRepository<T> : IReadonlyMongoRepository<T> where T : class
{
    protected readonly IMongoCollection<T> _collection;

    protected readonly MongoDbContext _context;

    public ReadonlyMongoRepository(MongoDbContext context, string collectionName)
    {
        _collection = context.GetCollection<T>(collectionName);
        _context = context;
    }

    public async Task<long> CountAsync()
    {
        return await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty);
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.CountDocumentsAsync(predicate);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(predicate).AnyAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(FilterDefinition<T>.Empty).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(predicate).ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
    }

    public async Task<T> GetOneAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(predicate).FirstOrDefaultAsync();
    }
}
