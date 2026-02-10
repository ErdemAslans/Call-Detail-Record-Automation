namespace Cdr.Api.Interfaces;

public interface IJwtConfig
{
    string Key { get; set; }

    string Issuer { get; set; }

    string Audience { get; set; }

    int ExpiresInMinutes { get; set; }
}