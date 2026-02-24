using Cdr.Api.Context;
using Cdr.Api.Helpers;
using Cdr.Api.Interfaces;
using Cdr.Api.Models.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cdr.Api.Repositories
{
    public class BreakRepository : ReadonlyMongoRepository<Break>, IBreakRepository
    {
        public BreakRepository(MongoDbContext context, IOptions<MongoDbSettings> mongoDbSettings)
            : base(context, mongoDbSettings.Value.BreakCollectionName)
        {
        }

        public async Task StartBreakAsync(Break breakInfo)
        {
            await _collection.InsertOneAsync(breakInfo);
        }

        public async Task EndBreakAsync(ObjectId breakId, DateTime endTime)
        {
            var filter = Builders<Break>.Filter.Eq(b => b.Id, breakId);
            var update = Builders<Break>.Update.Set(b => b.EndTime, endTime);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<Break> GetBreakByIdAsync(ObjectId breakId)
        {
            return await _collection.Find(b => b.Id == breakId).FirstOrDefaultAsync();
        }

        public async Task<bool> HasOngoingBreakAsync(string userId)
        {
            var filter = Builders<Break>.Filter.And(
                Builders<Break>.Filter.Eq(b => b.UserId, userId),
                Builders<Break>.Filter.Eq(b => b.EndTime, null),
                Builders<Break>.Filter.Ne(b => b.BreakType, "EndOfShift")
            );
            return await _collection.Find(filter).AnyAsync();
        }

        public async Task<bool> HasOngoingShiftEndAsync(string userId)
        {
            var filter = Builders<Break>.Filter.And(
                Builders<Break>.Filter.Eq(b => b.UserId, userId),
                Builders<Break>.Filter.Eq(b => b.EndTime, null),
                Builders<Break>.Filter.Eq(b => b.BreakType, "EndOfShift")
            );
            return await _collection.Find(filter).AnyAsync();
        }

        public async Task<bool> IsBreakEndedAsync(ObjectId breakId)
        {
            var breakInfo = await _collection.Find(b => b.Id == breakId).FirstOrDefaultAsync();
            return breakInfo?.EndTime != null;
        }

        public async Task<List<Break>> GetBreaksByUserIdAsync(string userId)
        {
            var filter = Builders<Break>.Filter.Eq(b => b.UserId, userId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<Break?> GetOngoingBreakAsync(string userId)
        {
            var filter = Builders<Break>.Filter.And(
                Builders<Break>.Filter.Eq(b => b.UserId, userId),
                Builders<Break>.Filter.Eq(b => b.EndTime, null)
            );
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<Break>> GetBreaksByUserIdAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            // Frontend'den gelen tarihler Turkey local time (UTC+3)
            // MongoDB UTC olarak saklar, bu yüzden dönüşüm gerekli
            var turkeyZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(startDate, DateTimeKind.Unspecified), turkeyZone);
            // endDate günün başını temsil eder, günün sonuna kadar (ertesi gün başı) almak için +1 gün ekliyoruz
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(endDate.Date.AddDays(1), DateTimeKind.Unspecified), turkeyZone);

            // StartTime bugün aralığında olan VEYA StartTime daha önce olup EndTime bugün aralığında olan molaları getir
            var filter = Builders<Break>.Filter.And(
                Builders<Break>.Filter.Eq(b => b.UserId, userId),
                Builders<Break>.Filter.Or(
                    // Molanın başlangıcı seçilen aralıkta
                    Builders<Break>.Filter.And(
                        Builders<Break>.Filter.Gte(b => b.StartTime, startUtc),
                        Builders<Break>.Filter.Lt(b => b.StartTime, endUtc)
                    ),
                    // Molanın bitişi seçilen aralıkta (gece yarısını geçen molalar)
                    Builders<Break>.Filter.And(
                        Builders<Break>.Filter.Lt(b => b.StartTime, startUtc),
                        Builders<Break>.Filter.Or(
                            Builders<Break>.Filter.And(
                                Builders<Break>.Filter.Gte(b => b.EndTime, startUtc),
                                Builders<Break>.Filter.Lt(b => b.EndTime, endUtc)
                            ),
                            Builders<Break>.Filter.Eq(b => b.EndTime, null) // Hala devam eden mola
                        )
                    )
                )
            );
            return await _collection.Find(filter).SortBy(b => b.StartTime).ToListAsync();
        }

        public async Task<List<Break>> GetAllBreaksByDateRangeAsync(DateTime startUtc, DateTime endUtc)
        {
            var filter = Builders<Break>.Filter.And(
                Builders<Break>.Filter.Gte(b => b.StartTime, startUtc),
                Builders<Break>.Filter.Lt(b => b.StartTime, endUtc)
            );
            return await _collection.Find(filter).SortBy(b => b.StartTime).ToListAsync();
        }
    }
}
