namespace TertiarySchoolNavigator.Api.Models.AuthModels
{
    public class RegisterRequest
    {



        public required string RegisterFirstName { get; set; }
        public required string RegisterFirstLastName { get; set; }
        public required string Gender { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

    }
}
