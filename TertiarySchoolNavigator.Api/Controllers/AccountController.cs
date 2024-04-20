using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TertiarySchoolNavigator.Api.Interface;
using TertiarySchoolNavigator.Api.Models.AuthModels;
using TertiarySchoolNavigator.Api.Validators;

namespace TertiarySchoolNavigator.Api.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IAuthenticationManager authenticationManager;
        private readonly RegisterRequestValidator registerRequestValidator;
        private readonly LoginRequestValidator loginRequestValidator;


        public AccountController(UserManager<User> userManager, IAuthenticationManager authenticationManager)
        {
            this.userManager = userManager;
            this.authenticationManager = authenticationManager;
            registerRequestValidator = new RegisterRequestValidator();
            loginRequestValidator = new LoginRequestValidator();
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequset loginModel)
        {
            // Validate the login model
            var validationResult = loginRequestValidator.Validate(loginModel);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // Find the user
            var user = await userManager.FindByEmailAsync(loginModel.username);
            if (user == null)
            {
                return BadRequest(new { Message = $"User with email {loginModel.username} does not exist" });
            }

            // Authenticate the user
            if (await authenticationManager.AuthenticateUserAsync(loginModel))
            {
                var token = await authenticationManager.CreateTokenAsync();
                return Ok(new { Message = "User logged in successfully", Token = token, User = user });
            }

            return Unauthorized(new { Message = "Invalid username or password" });
        }



        // Register a new user

        [HttpPost("register-admin")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerModel)
        {
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
            var user = new User
            {
                FirstName = registerModel.Firstname,
                LastName = registerModel.Lastname,
                Gender = registerModel.Gender,
                Email = registerModel.Email,
                UserName = registerModel.Email
            };

            var result = await userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            await userManager.AddToRoleAsync(user, "Administrator");

            return StatusCode(201, new { Message = "User registered successfully", User = user });
        }




        // Register a new user  for   user role
        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest registerModel)
        {
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
            var user = new User
            {
                FirstName = registerModel.Firstname,
                LastName = registerModel.Lastname,
                Gender = registerModel.Gender,
                Email = registerModel.Email,
                UserName = registerModel.Email
            };

            var result = await userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            // Add the user to the "User" role
            await userManager.AddToRoleAsync(user, "User");

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
                throw new BadHttpRequestException("No user found", StatusCodes.Status404NotFound);
            }
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
                return NotFound(new { Message = $"User with id {id} does not exist" });
            }
            return Ok(user);
        }


        // delete user by id


        [HttpDelete("users/{id}")]
        [Authorize]
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

    }
}
