namespace TertiarySchoolNavigator.Api.Models.AuthModels
{
    public class JWTSettings
    {
        public required string Audience { get; set; }
        public required string ValidIssuer { get; set; }
        public required string SecretKey { get; set; }
    }
}
