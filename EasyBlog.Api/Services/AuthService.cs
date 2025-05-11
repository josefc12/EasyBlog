using EasyBlog.Api.Models.Memory;
using EasyBlog.Api.Repositories;
using EasyBlog.Shared.Dtos;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;

namespace EasyBlog.Api.Services;

public interface IAuthService
{
    Task<IActionResult> GetNewAuthTokensFromRefreshToken(string refreshToken);
}

public class AuthService : IAuthService
{

    private IAuthRepository _authRepository;
    private JwtService _jwtService;
    private IUserService _userService;

    public AuthService(IAuthRepository authRepository, JwtService jwtService, IUserService userService)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
        _userService = userService;
    }

    //Find existing refresh token
    public async Task<IActionResult> GetNewAuthTokensFromRefreshToken(string refreshToken)
    {

        var storedToken = await _authRepository.FindExistingRefreshToken(oldHashedRefreshToken:refreshToken);
        
        if (storedToken is OkObjectResult tokenResult)
        {
            
            var tokenRecord = tokenResult.Value as RefreshToken;
            if (tokenRecord == null || tokenRecord.DateExpires < DateTime.UtcNow)
            {
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }

            int userId = int.Parse(tokenRecord.UserId);

            //Find the user this token belongs to.
            var user = await _userService.GetUserById(userId);

            if (user is OkObjectResult userResult)
            {
                UserDto userRecord = userResult.Value as UserDto;

                if (userRecord == null)
                {
                    return new StatusCodeResult(StatusCodes.Status401Unauthorized);
                }

                if (userRecord.Id != null && userRecord.Nickname != null)
                {
                    
                    // Generate new access token
                    var token = _jwtService.GenerateJwtToken(userRecord.Id.ToString(), userRecord.Nickname);

                    // Optionally rotate refresh token
                    var newRefreshToken = _jwtService.GenerateRefreshToken();
                    var newRefreshTokenHash = Argon2.Hash(newRefreshToken);

                    //Save the new refresh token
                    var result = await _authRepository.SaveExistingRefreshToken(tokenRecord,newRefreshTokenHash);

                    if (result is OkResult)
                    {
                        return new OkObjectResult(new AuthTokens(){ JwtToken = token, RefreshToken = newRefreshTokenHash });
                    }

                    //If anything went wrong just drop this for now:
                    return new StatusCodeResult(StatusCodes.Status401Unauthorized);

                }
            }

            //If anything went wrong just drop this for now:
            return new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }

        //If anything went wrong just drop this for now:
        return new StatusCodeResult(StatusCodes.Status401Unauthorized);
    }
}