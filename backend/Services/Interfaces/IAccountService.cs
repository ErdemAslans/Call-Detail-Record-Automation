using Cdr.Api.Models.Account;
using Cdr.Api.Models.Account.Responses;

namespace Cdr.Api.Services.Interfaces;

public interface IAccountService
{
    Task<LoginResponseModel> LoginAsync(LoginModel model);
}
