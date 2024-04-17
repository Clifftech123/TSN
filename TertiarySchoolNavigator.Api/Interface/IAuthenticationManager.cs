using System.Security.Claims;
using TertiarySchoolNavigator.Api.Models.AuthModels;

namespace TertiarySchoolNavigator.Api.Interface
{
    public interface IAuthenticationManager
    {
        Task<bool> AuthenticateUserAsync(LoginRequest loginRequest);
        Task<string> CreateTokenAsync();
        Task<List<Claim>> GetClaimsAsync();

    }
}
