namespace TertiarySchoolNavigator.Api.Contracts.Auth
{
    public class LoginUserRequset
    {

        public required string username { get; set; }
        public required string Password { get; set; }
    }
}
