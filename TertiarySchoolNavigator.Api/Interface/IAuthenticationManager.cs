using System.Security.Claims;
using TertiarySchoolNavigator.Api.Contracts.Auth;

namespace TertiarySchoolNavigator.Api.Interface
{
    public interface IAuthenticationManager
    {
        Task<bool> AuthenticateUserAsync(LoginUserRequset loginRequest);
        Task<string> CreateTokenAsync();
        object GenerateRefreshToken();
        Task<List<Claim>> GetClaimsAsync();


    }


}
