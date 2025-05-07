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
        public async Task<IActionResult> UserRegister([FromBody] UserDto newUserData)
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
        
        [HttpPost("login")]
        public async Task<IActionResult> UserLogin([FromBody] UserDto loginData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }
            
            //Pass it to the service
            var result = await userService.UserLogin(loginData);

            if (result is OkObjectResult okObject)
            {
                var token = okObject.Value as string;
                return Ok(token);
            }

            return BadRequest("Invalid data.");
        }
        
        /*
        [Authorize]
        [HttpGet("user-data/{id}")]
        public async Task<UserDto> GetUserData(int id)
        {
            
            return new JwtToken {Token=null};
        }
        */
    }
}