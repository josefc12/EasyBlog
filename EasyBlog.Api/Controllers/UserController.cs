using System.Threading.Tasks;
using EasyBlog.Api.Models.Memory;
using EasyBlog.Api.Services;
using EasyBlog.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyBlog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> UserRegister(UserDto newUserData)
        {

            //Validate the DTO
            if (newUserData.Nickname == null || newUserData.PasswordHash == null)
            {
                return ValidationProblem(new ValidationProblemDetails()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation Error",
                    Detail = "Nickname and Password must be provided."
                });
            }

            var result = await userService.UserRegister(newUserData);

            if (result is OkResult)
            {
                // Handle success case
                return Ok(new { message = "User registered successfully." });
            }

            return Ok();
        }

        //TODO: User login and Get user data

        /*
        [HttpPost("login")]
        public async Task<JwtToken> UserLogin(UserDto loginData)
        {
            
            return new JwtToken {Token=null};
        }
        
        [Authorize]
        [HttpGet("user-data/{id}")]
        public async Task<UserDto> GetUserData(int id)
        {
            
            return new JwtToken {Token=null};
        }
        */
    }
}