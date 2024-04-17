using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TertiarySchoolNavigator.Api.Interface;
using TertiarySchoolNavigator.Api.Models.AuthModels;

namespace TertiarySchoolNavigator.Api.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IAuthenticationManager authenticationManager;


        public AccountController(UserManager<User> userManager, IAuthenticationManager authenticationManager)
        {
            this.userManager = userManager;
            this.authenticationManager = authenticationManager;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginModel)
        {
            var user = await userManager.FindByEmailAsync(loginModel.LoginUserName);
            if (user == null)
            {
                throw new BadHttpRequestException($" User with email{loginModel.LoginUserName} does not excite", StatusCodes.Status400BadRequest);
            }
            if (await authenticationManager.AuthenticateUserAsync(loginModel))
            {
                return Ok(new { Message = "User logged in successfully", Token = await authenticationManager.CreateTokenAsync(), User = user });
            }
            return Unauthorized(new { Message = "Invalid username or password" });
        }




        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerModel)
        {
            var user = new User();

            user.FirstName = registerModel.RegisterFirstName;
            user.LastName = registerModel.RegisterFirstLastName;
            user.Gender = registerModel.Gender;
            user.Email = registerModel.Email;
            user.UserName = registerModel.Email;

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


        // Get all user 

        [HttpGet("users")]

        public async Task<IActionResult> GetUsers()
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

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with id {id} does not exist" });
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
