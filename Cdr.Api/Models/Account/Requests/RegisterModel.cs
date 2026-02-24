namespace Cdr.Api.Models.Account
{
    public class RegisterModel
    {
        public required string UserName { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public string? Role { get; set; }
    }
}