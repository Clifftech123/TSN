namespace TertiarySchoolNavigator.Api.Contracts.Auth
{
    public class LoginUserResponse
    {
        public required string JwtToken { get; set; }
        public DateTime Expiration { get; set; }
        public required string RefreshToken { get; set; }
    }
}
