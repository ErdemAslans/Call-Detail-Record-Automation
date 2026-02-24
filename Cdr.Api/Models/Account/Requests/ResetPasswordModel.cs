namespace Cdr.Api.Models.Account
{
    public class ResetPasswordModel
    {
        public required string Email { get; set; }
        public required string NewPassword { get; set; }
    }
}
