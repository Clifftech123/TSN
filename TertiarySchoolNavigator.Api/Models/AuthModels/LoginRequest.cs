using System.ComponentModel.DataAnnotations;

namespace TertiarySchoolNavigator.Api.Models.AuthModels
{
    public class LoginRequest
    {
     
        public string Username { get; set; } = string.Empty; 
        public string Password { get; set; } = string.Empty;
    }
}
