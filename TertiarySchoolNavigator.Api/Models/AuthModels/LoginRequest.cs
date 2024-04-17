namespace TertiarySchoolNavigator.Api.Models.AuthModels
{
    public class LoginRequest
    {

        public string LoginUserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
