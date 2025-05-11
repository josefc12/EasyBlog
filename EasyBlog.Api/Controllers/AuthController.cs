using EasyBlog.Api.Data;
using EasyBlog.Api.Models.Memory;
using EasyBlog.Api.Services;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyBlog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> GetRefreshToken()
    {
        
        // Get refresh token from cookie
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized("No refresh token provided.");
        }

        var result = await _authService.GetNewAuthTokensFromRefreshToken(refreshToken);

        if (result is OkObjectResult authTokensResult)
        {
            var authTokens = authTokensResult.Value as AuthTokens;

            if (authTokens == null || authTokens.JwtToken == null || authTokens.RefreshToken == null)
            {
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }

            Response.Cookies.Append("refreshToken", authTokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(authTokens.JwtToken);
        }
        //If anything went wrong just return unauthorized for now
        return Unauthorized("Something went wrong along the way, try later.");
    }
}