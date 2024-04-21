using System.Security.Claims;
using TertiarySchoolNavigator.Api.Models.AuthModels;

namespace TertiarySchoolNavigator.Api.Interface
{
    public interface IAuthenticationManager
    {
        Task<bool> AuthenticateUserAsync(LoginRequset loginRequest);
        Task<string> CreateTokenAsync();
        object GenerateRefreshToken();
        Task<List<Claim>> GetClaimsAsync();
     

    }
     

    }
