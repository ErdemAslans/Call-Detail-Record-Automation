using Cdr.Api.Models.Entities;
using Cdr.Api.Models.Response.UserStatistics;

namespace Cdr.Api.Interfaces
{
    public interface IOperatorRepository: IReadonlyMongoRepository<Operator>
    {
        Task<OperatorInfo> GetUserInfoAsync(string number);

        Task<List<OperatorInfo>> GetAllUsersInfoAsync();

        Task<Operator> GetUserByUsernameAsync(string username);
    }
}
