using Cdr.Api.Models.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cdr.Api.Interfaces
{
    public interface IBreakRepository
    {
        Task StartBreakAsync(Break breakInfo);

        Task EndBreakAsync(ObjectId breakId, DateTime endTime);

        Task<Break> GetBreakByIdAsync(ObjectId breakId);

        Task<bool> HasOngoingBreakAsync(string userId);

        Task<bool> IsBreakEndedAsync(ObjectId breakId);

        Task<List<Break>> GetBreaksByUserIdAsync(string userId);

        Task<List<Break>> GetBreaksByUserIdAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate);

        Task<Break?> GetOngoingBreakAsync(string userId);

        Task<List<Break>> GetAllBreaksByDateRangeAsync(DateTime startUtc, DateTime endUtc);
    }
}
