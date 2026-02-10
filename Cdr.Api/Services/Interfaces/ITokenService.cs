using System.Security.Claims;
using Cdr.Api.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Cdr.Api.Services.Interfaces;

/// <summary>
/// Service for JWT token generation and validation operations
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a new JWT access token for the specified user
    /// </summary>
    /// <param name="user">The user for whom to generate the token</param>
    /// <returns>A JWT access token string</returns>
    Task<string> GenerateAccessToken(User user);

    /// <summary>
    /// Generates a new refresh token
    /// </summary>
    /// <returns>A refresh token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Extracts and validates the claims principal from an expired token
    /// </summary>
    /// <param name="token">The expired JWT token</param>
    /// <returns>The claims principal contained in the token</returns>
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    /// <summary>
    /// Generates a new access token using a refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token to use</param>
    /// <returns>A new JWT access token string</returns>
    Task<string> GenerateAccessTokenFromRefreshTokenAsync(string refreshToken);
}