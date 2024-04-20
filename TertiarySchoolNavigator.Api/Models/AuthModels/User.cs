using Microsoft.AspNetCore.Identity;

namespace TertiarySchoolNavigator.Api.Models.AuthModels
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
    }
}
