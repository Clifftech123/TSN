namespace TertiarySchoolNavigator.Api.Contracts.Auth
{
    public class RegisterUserRequest
    {

        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
       




    }
}
