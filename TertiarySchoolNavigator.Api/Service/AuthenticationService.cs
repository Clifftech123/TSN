using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TertiarySchoolNavigator.Api.Contracts.Auth;
using TertiarySchoolNavigator.Api.Interface;
using TertiarySchoolNavigator.Api.Models.AuthModels;

namespace TertiarySchoolNavigator.Api.Service
{
    public class AuthenticationService : IAuthenticationManager
    {

        // Inject the UserManager and SignInManager services
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private User? _user;


        public AuthenticationService(UserManager<User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }


        // Authenticate the user with the provided credentials

        public async Task<bool> AuthenticateUserAsync(LoginUserRequset loginRequest)
        {
            _user = await userManager.FindByEmailAsync(loginRequest.username);
            //if (user == null)
            //{
            // Handle the null user case here. For example, you could throw an exception:
            //  throw new ArgumentException("No user found with the provided email.");
            // }
            var result = (_user is not null && await userManager.CheckPasswordAsync(_user, loginRequest.Password));
            return result;
        }



        // Create a JWT token for the authenticated user
        public async Task<string> CreateTokenAsync()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaimsAsync();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);


            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            var role = claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            return token;
        }



        //  Get claims for the authenticated user
        public async Task<List<Claim>> GetClaimsAsync()
        {
            var claims = new List<Claim>();

            if (_user != null)
            {
                claims.Add(new Claim(ClaimTypes.Sid, _user.Id.ToString()));
            }

            var roles = await userManager.GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }


        // Generate the token options with the provided signing credentials and claim
        private SigningCredentials GetSigningCredentials()
        {
            var jwtSettings = configuration.GetSection("JWTSettings");
            var secretKey = jwtSettings.GetSection("secretKey").Value;

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException("secretKey", "The secret key must not be null or empty.");
            }

            var key = Encoding.UTF8.GetBytes(secretKey);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }


        // Generate the token options with the provided signing credentials and claim
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = configuration.GetSection("JWTSettings");
            var tokenOptions = new JwtSecurityToken(
            issuer: jwtSettings.GetSection("validIssuer").Value,
            audience: jwtSettings.GetSection("audience").Value,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("expiryInMinutes").Value)),
            signingCredentials: signingCredentials);
            return tokenOptions;
        }



        public object GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var generate = RandomNumberGenerator.Create();
            generate.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }






    }
}
