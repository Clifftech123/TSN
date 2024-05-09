using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TertiarySchoolNavigator.Api.Contracts.Auth;
using TertiarySchoolNavigator.Api.Extensions;
using TertiarySchoolNavigator.Api.Interface;
using TertiarySchoolNavigator.Api.Models.AuthModels;
using TertiarySchoolNavigator.Api.Validators;

namespace TertiarySchoolNavigator.Api.Controllers
{


    [ApiController]
    [Route("api/account/v1")]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IMapper _mapper;
        private readonly RegisterRequestValidator registerRequestValidator;
        private readonly LoginRequestValidator loginRequestValidator;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;



        public AccountController(UserManager<User> userManager,
            IAuthenticationManager authenticationManager, IMapper mapper, ILogger<AccountController> logger,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.authenticationManager = authenticationManager;
            _mapper = mapper;

            registerRequestValidator = new RegisterRequestValidator();
            loginRequestValidator = new LoginRequestValidator();
            _logger = logger;
            _configuration = configuration;
        }



        // Login a user  

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequset loginModel)
        {
            try
            {
                // Validate the login model
                FluentValidation.Results.ValidationResult validationResult = loginRequestValidator.Validate(loginModel);
                if (validationResult.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(loginModel.username);
                    var isPasswordValid = user != null && await userManager.CheckPasswordAsync(user, loginModel.Password);
                    if (user == null || !isPasswordValid)
                    {
                        return BadRequest(new { Message = "Invalid username or password" });
                    }

                    if (await authenticationManager.AuthenticateUserAsync(loginModel))
                    {
                        var token = await authenticationManager.CreateTokenAsync();
                        var refreshToken = authenticationManager.GenerateRefreshToken();
                        user.RefreshTokenExpiry = DateTime.Now.AddDays(1);

                        await userManager.UpdateAsync(user);

                        var userRole = await userManager.GetRolesAsync(user);
                        return Ok(new { User = new { user.Id, user.UserName, user.FirstName, user.LastName, user.Email, Roles = userRole, Token = token, refreshToken } });

                    }

                    return Unauthorized(new { Message = "Invalid username or password" });
                }

                return UnprocessableEntity(validationResult.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in a user");
                throw new BadHttpRequestException(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }




        // Register a new user

        [HttpPost("register-admin")]

        public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerModel)
        {
            _logger.LogInformation("Registering a new user");
            // Validate the register model
            var validationResult = registerRequestValidator.Validate(registerModel);
            if (!validationResult.IsValid)
            {
                throw new BadHttpRequestException("Invalid input", StatusCodes.Status400BadRequest);
            }

            // Check if a user with the same email already exists
            var existingUser = await userManager.FindByEmailAsync(registerModel.Email);
            if (existingUser != null)
            {
                throw new BadHttpRequestException("User with the same email already exists", StatusCodes.Status400BadRequest);
            }

            // Create a new user
            var user = _mapper.Map<User>(registerModel);
            var result = await userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                _logger.LogError("Error occured while registering a new user");

                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            await userManager.AddToRoleAsync(user, "Administrator");

            _logger.LogInformation("User registered successfully");

            return StatusCode(201, new { Message = "User registered successfully", User = user });
        }




        // Register a new user  for   user role


        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest registerModel)
        {
            _logger.LogInformation("Registering a new user");
            // Validate the register model
            var validationResult = registerRequestValidator.Validate(registerModel);
            if (!validationResult.IsValid)
            {
                throw new BadHttpRequestException("Invalid input", StatusCodes.Status400BadRequest);
            }

            // Check if a user with the same email already exists
            var existingUser = await userManager.FindByEmailAsync(registerModel.Email);
            if (existingUser != null)
            {
                throw new BadHttpRequestException("User with the same email already exists", StatusCodes.Status400BadRequest);
            }

            // Create a new user
            var user = _mapper.Map<User>(registerModel);

            var result = await userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                _logger.LogError("Error occured while registering a new user");
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            // Add the user to the "User" role
            await userManager.AddToRoleAsync(user, "User");

            _logger.LogInformation("User registered successfully");

            return StatusCode(201, new { Message = "User registered successfully", User = user });
        }



        // Get all user 

        [HttpGet("users")]
        // [Authorize(Roles = "Administrator")]
        public IActionResult GetUsers()
        {
            var users = userManager.Users.ToList();
            if (users.Count == 0)
            {
                throw new BadHttpRequestException("No user found", StatusCodes.Status404NotFound);
            }
            return Ok(users);
        }


        // Get user by id

        [HttpGet("users/{id}")]
        // [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with id {id} does not exist" });
            }
            return Ok(user);
        }


        // delete user by id


        [HttpDelete("users/{id}")]
        /// [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with id {id} does not exist" });
            }

            // Get the current user
            var currentUser = await userManager.GetUserAsync(User);

            // Check if the current user is the same as the user to be deleted or if the current user is an admin
            if (currentUser.Id != user.Id && !await userManager.IsInRoleAsync(currentUser, "Administrator"))
            {
                return Forbid();
            }

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return Ok(new { Message = "User deleted successfully" });
        }





        // Update User 
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest updateUserModel)
        {
            // Find the user by id
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with id {id} does not exist" });
            }

            // Update the user properties
            user.FirstName = updateUserModel.FirstName;
            user.LastName = updateUserModel.LastName;
            user.Email = updateUserModel.Email;

            // Save the changes
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return Ok(new { Message = "User updated successfully", User = user });
        }




        // User logout both Admin and User

        [HttpPost("logout")]

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User logging out");

            var username = HttpContext.User.Identity?.Name;

            if (username is null)
                return Unauthorized();

            var user = await userManager.FindByNameAsync(username);

            if (user is null)
                return Unauthorized();

            user.RefreshToken = null;

            await userManager.UpdateAsync(user);

            _logger.LogInformation("User logged out successfully");

            return Ok();
        }


        // Refresh token
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshModel refreshModel)
        {
            var principal = AuthServiceExtensions.GetPrincipalFromExpiredToken(refreshModel.AccessToken, _configuration);
            var username = principal.Identity.Name;

            var user = await userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != refreshModel.RefreshToken || user.RefreshTokenExpiry <= DateTime.Now)
            {
                return BadRequest(new { Message = "Invalid client request" });
            }

            var newAccessToken = await authenticationManager.CreateTokenAsync();
            var newRefreshToken = authenticationManager.GenerateRefreshToken();

            user.RefreshToken = (string)newRefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(1);

            await userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }


        //revoke token

        [HttpPost("Revoke")]
        public async Task<IActionResult> Revoke()
        {
            _logger.LogInformation("Revoke called");

            var username = HttpContext.User.Identity?.Name;

            if (username is null)
                return Unauthorized();

            var user = await userManager.FindByNameAsync(username);

            if (user is null)
                return Unauthorized();

            user.RefreshToken = null;

            await userManager.UpdateAsync(user);

            _logger.LogInformation("Revoke succeeded");

            return Ok();
        }








    }
}
