namespace Cdr.Api.Models.Account.Responses
{
    public class LoginResponseModel
    {
        public required string Token { get; set; }

        public required string RefreshToken { get; set; }
    }
}