using System.Security.Claims;
using System.Threading.Tasks;
using EasyBlog.Api.Data;
using EasyBlog.Api.Models.Memory;
using EasyBlog.Api.Services;
using EasyBlog.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyBlog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService, EasyBlogDbContext context) : ControllerBase
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
                var tokens = okObject.Value as AuthTokens;

                // Set refresh token in HTTP-only cookie
                Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                return Ok(tokens.JwtToken);
            }

            return BadRequest("Invalid data.");
        }
    
        
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("set-admin-role/{nickname}")]
        public async Task<IActionResult> SetAdminRole(string nickname)
        {

            if (string.IsNullOrWhiteSpace(nickname))
            {
                return BadRequest("Invalid nickname.");
            }

            var result = await userService.SetAdminRole(nickname);

            if (result is OkObjectResult okObject)
            {
                return Ok("User was assigned the Admin role.");
            }
            
            return BadRequest("User not found or invalid data.");
        }
        
    }
}