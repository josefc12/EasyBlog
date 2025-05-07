using EasyBlog.Api.Models;
using EasyBlog.Api.Repositories;
using EasyBlog.Shared.Dtos;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;

namespace EasyBlog.Api.Services;

public interface IUserService
{
    Task<IActionResult> UserRegister(UserDto newUserData);
    Task<IActionResult> UserLogin(UserDto loginData);
}

public class UserService(IUserRepository userRepository, JwtService jwtService) : IUserService
{
    public async Task<IActionResult> UserRegister(UserDto newUserData)
    {

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
        var result = await userRepository.UserRegister(newUser);

        return result;
    }
    
    public async Task<IActionResult> UserLogin(UserDto loginData)
    {
        //Try finding the user, if they exist
        var result = await userRepository.GetUser(loginData.Nickname);

        if (result is OkObjectResult okResult)
        {
            
            var user = okResult.Value as User; 
            
            if (Argon2.Verify(user.PasswordHash , loginData.PasswordHash))
            {
                //Password correct, return the token
                var token = jwtService.GenerateJwtToken(user.Id.ToString(), user.Nickname);
                return new OkObjectResult(token);
            }
            //Wrong password
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        return result;
    }
}
