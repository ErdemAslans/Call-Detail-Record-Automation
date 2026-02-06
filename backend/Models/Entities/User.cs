using Microsoft.AspNetCore.Identity;

namespace Cdr.Api.Models.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}