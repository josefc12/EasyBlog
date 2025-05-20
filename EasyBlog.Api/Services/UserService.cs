using EasyBlog.Api.Data;
using EasyBlog.Api.Models;
using EasyBlog.Api.Models.Memory;
using EasyBlog.Api.Repositories;
using EasyBlog.Shared.Dtos;
using EasyBlog.Shared.Enums;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace EasyBlog.Api.Services;

public interface IUserService
{
    Task<IActionResult> UserRegister(UserDto newUserData);
    Task<IActionResult> UserLogin(UserDto loginData);
    Task<IActionResult> GetUserById(int userId);
    Task<IActionResult> SetAdminRole(string nickname);
}

public class UserService(IUserRepository userRepository, IAuthRepository authRepository, JwtService jwtService, EasyBlogDbContext context) : IUserService
{
    public async Task<IActionResult> UserRegister(UserDto newUserData)
    {

        //If the user of the same nickname already exists, don't allow creation.
        var result = await userRepository.GetUser(newUserData.Nickname);
        if (result is OkObjectResult okResult)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        //Encrypt the password
        string encryptedPassword = Argon2.Hash(newUserData.PasswordHash);
        newUserData.PasswordHash = encryptedPassword;

        //TODO nickname must be unique - do a check

        //Create the actual entity
        User newUser = new()
        {
            Nickname = newUserData.Nickname ?? "<no-nickname>",
            PasswordHash = newUserData.PasswordHash ?? "<no-password>",
            Email = null,
            DateCreated = DateTime.UtcNow,
            DateDeleted = null
        };

        //Write the new user into the database
        var sucResult = await userRepository.UserRegister(newUser);

        return sucResult;
    }

    public async Task<IActionResult> UserLogin(UserDto loginData)
    {
        //Try finding the user, if they exist
        var result = await userRepository.GetUser(loginData.Nickname);

        if (result is OkObjectResult okResult)
        {

            var user = okResult.Value as User;

            if (Argon2.Verify(user.PasswordHash, loginData.PasswordHash))
            {
                //Password correct, return the token
                var token = jwtService.GenerateJwtToken(user.Id.ToString(), user.Nickname, user.Role.ToString());

                //Find their refresh token and see if it's valid.
                var exitingRefreshTokenResult = await authRepository.FindExistingRefreshToken(userId: user.Id.ToString());

                if (exitingRefreshTokenResult is OkObjectResult existingRefreshTokenRestult)
                {
                    var refreshTokenRecord = existingRefreshTokenRestult.Value as RefreshToken;

                    if (refreshTokenRecord != null && refreshTokenRecord.DateExpires > DateTime.UtcNow)
                    {
                        return new OkObjectResult(new AuthTokens { JwtToken = token, RefreshToken = refreshTokenRecord.Token });
                    }
                    else if (refreshTokenRecord == null || refreshTokenRecord.IsRevoked == true)
                    {
                        //Generate a new token
                        var refreshToken = jwtService.GenerateRefreshToken();
                        var refreshTokenHash = Argon2.Hash(refreshToken);
                        // Store refresh token in the database
                        await context.RefreshTokens.AddAsync(new RefreshToken
                        {
                            Token = refreshTokenHash,
                            UserId = user.Id.ToString(),
                            DateCreated = DateTime.UtcNow,
                            DateExpires = DateTime.UtcNow.AddDays(7),
                            IsRevoked = false
                        });
                        await context.SaveChangesAsync();

                        return new OkObjectResult(new AuthTokens { JwtToken = token, RefreshToken = refreshTokenHash });
                    }
                }

                //Something went wrong along the way
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }

            //Wrong password
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        return result;
    }

    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await userRepository.GetUserById(userId);

        if (user is OkObjectResult userResult)
        {
            var userRecord = userResult.Value as User;

            if (userRecord != null)
            {
                return new OkObjectResult(userRecord);
            }

            //Something went wrong along the way
            return new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }

        //Something went wrong along the way
        return new StatusCodeResult(StatusCodes.Status401Unauthorized);
    }

    public async Task<IActionResult> SetAdminRole(string nickname)
    {
        var user = await userRepository.GetUser(nickname);
        if (user is OkObjectResult userResult)
        {
            var userRecord = userResult.Value as User;

            if (userRecord != null)
            {
                userRecord.Role = Roles.Admin;
                await userRepository.SetAdminRole(userRecord);
                return new OkObjectResult(userRecord);
            }
            //TODO probably best to return something explicit
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        //TODO probably best to return something explicit
        return new StatusCodeResult(StatusCodes.Status400BadRequest);
    }
}
