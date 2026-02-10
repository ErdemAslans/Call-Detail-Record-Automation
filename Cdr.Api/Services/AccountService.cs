using Cdr.Api.Models.Account;
using Cdr.Api.Models.Account.Responses;
using Cdr.Api.Models.Entities;
using Cdr.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Cdr.Api.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<AccountService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;

    public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountService> logger, IConfiguration configuration, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponseModel> LoginAsync(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var result = await _userManager.CheckPasswordAsync(user, model.Password);

        if (!result)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = await _tokenService.GenerateAccessToken(user);

        return new LoginResponseModel
        {
            Token = token,
            RefreshToken = _tokenService.GenerateRefreshToken()
        };
    }
}