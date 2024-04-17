namespace TertiarySchoolNavigator.Api.Models.AuthModels
{
    public class JWTSettings
    {
        public string Audience { get; set; }
        public string ValidIssuer { get; set; }
        public string SecretKey { get; set; }
    }
}
