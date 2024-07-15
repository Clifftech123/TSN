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
                var validationResult = loginRequestValidator.Validate(loginModel);
                if (!validationResult.IsValid) return UnprocessableEntity(validationResult.Errors);
                var user = await userManager.FindByEmailAsync(loginModel.username);
                var isPasswordValid = user != null && await userManager.CheckPasswordAsync(user, loginModel.Password);
                if (user == null || !isPasswordValid)
                {
                    return BadRequest(new { Message = "Invalid username or password" });
                }

                if (!await authenticationManager.AuthenticateUserAsync(loginModel))
                    return Unauthorized(new { Message = "Invalid username or password" });
                var token = await authenticationManager.CreateTokenAsync();
                var refreshToken = authenticationManager.GenerateRefreshToken();
                user.RefreshTokenExpiry = DateTime.Now.AddDays(1);

                await userManager.UpdateAsync(user);

                var userRole = await userManager.GetRolesAsync(user);
                return Ok(new { User = new { user.Id, user.UserName, user.FirstName, user.LastName, user.Email, Roles = userRole, Token = token, refreshToken } });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in a user");
                throw new BadHttpRequestException(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }


        
        // Register a new user  for   admin role

        [HttpPost("register-admin")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerModel)
        {
            _logger.LogInformation("Registering a new user");

            // Validate the register model using injected validators
            var validationResult = registerRequestValidator.Validate(registerModel);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            // Check if a user with the same email already exists
            var existingUser = await userManager.FindByEmailAsync(registerModel.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "User with the same email already exists");
                return BadRequest(ModelState);
            }

            // Create a new user
            var user = _mapper.Map<User>(registerModel);
            var result = await userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
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

            // Validate the register model using injected validators
            var validationResult = registerRequestValidator.Validate(registerModel);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            // Check if a user with the same email already exists
            var existingUser = await userManager.FindByEmailAsync(registerModel.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "User with the same email already exists");
                return BadRequest(ModelState);
            }

            // Create a new user
            var user = _mapper.Map<User>(registerModel);
            var result = await userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("Error occurred while registering a new user");
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
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
        [Authorize(Roles = "Administrator")]
        public IActionResult GetUsers()
        {
            var users = userManager.Users.ToList();
            if (users.Count == 0)
            {
                _logger.LogInformation("No users found");
                // Returning an empty list to indicate no users found
                return Ok(new List<User>());
            }
            _logger.LogInformation($"Retrieved {users.Count} users");
            return Ok(users);
        }


        // Get user by id

    
        [HttpGet("users/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {id} does not exist." });
            }

            var userDto = new 
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.RefreshToken,
                user.RefreshTokenExpiry
                
            };

            return Ok(userDto);
        }


        // delete user by id

        [HttpDelete("users/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            _logger.LogInformation($"Attempting to delete user with ID: {id}");

            // Find the user to delete
            var userToDelete = await userManager.FindByIdAsync(id);
            if (userToDelete == null)
            {
                _logger.LogWarning($"User with ID: {id} not found for deletion.");
                return NotFound(new { Message = $"User with ID: {id} does not exist." });
            }
            
            // Check if the requesting user is an admin or the user to be deleted

            var requestingUser = await userManager.GetUserAsync(User);
            var isRequestingUserAdmin = await userManager.IsInRoleAsync(requestingUser, "Administrator");

            if (requestingUser.Id != userToDelete.Id && !isRequestingUserAdmin)
            {
                _logger.LogWarning($"User with ID: {requestingUser.Id} attempted to delete user with ID: {id} without sufficient permissions.");
                return Forbid();
            }

            // Delete the user
            var deletionResult = await userManager.DeleteAsync(userToDelete);
            if (!deletionResult.Succeeded)
            {
                _logger.LogError($"Failed to delete user with ID: {id}. Errors: {string.Join(", ", deletionResult.Errors.Select(e => e.Description))}");
                foreach (var error in deletionResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"User with ID: {id} deleted successfully.");
            return Ok(new { Message = "User deleted successfully." });
        }



        // Update User 
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest updateUserModel)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var isCurrentUserAdmin = await userManager.IsInRoleAsync(currentUser, "Administrator");

            // Find the user by id
            var userToUpdate = await userManager.FindByIdAsync(id);
            if (userToUpdate == null)
            {
                return NotFound(new { Message = $"User with id {id} does not exist" });
            }

            // Check if the current user is allowed to update the target user
            if (!isCurrentUserAdmin && currentUser.Id != userToUpdate.Id)
            {
                return Forbid("You do not have permission to update this user");
            }

            // Update the user properties
            userToUpdate.FirstName = updateUserModel.FirstName;
            userToUpdate.LastName = updateUserModel.LastName;
            userToUpdate.Email = updateUserModel.Email;

            // Save the changes
            var result = await userManager.UpdateAsync(userToUpdate);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return Ok(new { Message = "User updated successfully", User = userToUpdate });
        }




        // User logout both Admin and User
        /// <summary>
        /// Logs out the current user by clearing their refresh token.
        /// </summary>
        /// <returns>An IActionResult indicating the outcome of the logout operation.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Initiating user logout process.");

            var username = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Logout attempt failed: Username is null or empty.");
                return Unauthorized(new { Message = "You must be logged in to log out." });
            }

            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning($"Logout attempt failed: User '{username}' not found.");
                return Unauthorized(new { Message = "User not found." });
            }

            user.RefreshToken = null;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to log out user '{username}'.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while logging out." });
            }

            _logger.LogInformation($"User '{username}' logged out successfully.");
            return Ok(new { Message = "Logged out successfully." });
        }


        // Refresh token
     [HttpPost("Refresh")]
public async Task<IActionResult> Refresh([FromBody] RefreshModel refreshModel)
{
    // Extract username from the expired access token
    var principal = AuthServiceExtensions.GetPrincipalFromExpiredToken(refreshModel.AccessToken, _configuration);
    var username = principal.Identity.Name;

    // Retrieve the user from the database
    var user = await userManager.FindByNameAsync(username);
    if (user == null)
    {
        return BadRequest(new { Message = "User not found." });
    }

    // Validate the refresh token
    if (user.RefreshToken != refreshModel.RefreshToken || user.RefreshTokenExpiry <= DateTime.Now)
    {
        return BadRequest(new { Message = "Invalid or expired refresh token." });
    }

    // Generate new tokens
    var newAccessToken = await authenticationManager.CreateTokenAsync();
    var newRefreshToken = authenticationManager.GenerateRefreshToken();

    // Update user's refresh token and expiry
    user.RefreshToken =(string) newRefreshToken;
    user.RefreshTokenExpiry = DateTime.Now.AddDays(1);
    await userManager.UpdateAsync(user);

    // Return the new tokens
    return Ok(new
    {
        AccessToken = newAccessToken,
        RefreshToken = newRefreshToken
    });
}

        //revoke token

        /// <summary>
        /// Revokes the refresh token for the current user, effectively logging them out from all devices.
        /// </summary>
        /// <returns>An IActionResult indicating the outcome of the revoke operation.</returns>
        [HttpPost("Revoke")]
        public async Task<IActionResult> Revoke()
        {
            _logger.LogInformation("Revoke token process initiated.");

            var username = HttpContext.User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("Revoke token attempt failed: Username is null or empty.");
                return Unauthorized(new { Message = "Invalid request." });
            }

            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning($"Revoke token attempt failed: User '{username}' not found.");
                return Unauthorized(new { Message = "User not found." });
            }

            if (user.RefreshToken == null)
            {
                _logger.LogInformation($"Revoke token attempt aborted: No refresh token found for user '{username}'.");
                return Ok(new { Message = "No refresh token to revoke." });
            }

            user.RefreshToken = null;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to revoke refresh token for user '{username}'.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while revoking the token." });
            }

            _logger.LogInformation($"Refresh token for user '{username}' revoked successfully.");
            return Ok(new { Message = "Token revoked successfully." });
        }







    }
}
