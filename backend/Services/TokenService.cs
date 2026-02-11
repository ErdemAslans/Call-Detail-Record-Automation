using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cdr.Api.Interfaces;
using Cdr.Api.Models.Entities;
using Cdr.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Cdr.Api.Services;
public class TokenService : ITokenService
{
    private readonly IJwtConfig _jwtConfig;
    private readonly UserManager<User> _userManager;

    public TokenService(IJwtConfig jwtConfig, UserManager<User> userManager)
    {
        _userManager = userManager;
        _jwtConfig = jwtConfig;
    }

    public async Task<string> GenerateAccessToken(User user)
    {
        var key = GetSecurityKey();
        var credentials = GetSigningCredentials(key);
        var claims = await GetClaimsAsync(user);

        var token = CreateJwtToken(claims, credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateAccessTokenFromRefreshTokenAsync(string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(refreshToken);
        var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }
        return await GenerateAccessToken(user);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }


    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // Süresi dolmuş token'ları doğrulamak için
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtConfig.Issuer,
            ValidAudience = _jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    private SymmetricSecurityKey GetSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
    }

    private SigningCredentials GetSigningCredentials(SymmetricSecurityKey key)
    {
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaimsAsync(User user)
    {
        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.UniqueName, user?.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private JwtSecurityToken CreateJwtToken(IEnumerable<Claim> claims, SigningCredentials credentials)
    {
        return new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiresInMinutes),
            signingCredentials: credentials
        );
    }
}