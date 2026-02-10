using Cdr.Api.Interfaces;

namespace Cdr.Api.Models;
public class JwtConfigModel: IJwtConfig
{
    public required string Key { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ExpiresInMinutes { get; set; }
}