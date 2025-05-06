using System.Threading.Tasks;
using EasyBlog.Api.Models;
using EasyBlog.Api.Repositories;
using EasyBlog.Shared.Dtos;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EasyBlog.Api.Services;

public interface IUserService
{
    Task<IActionResult> UserRegister(UserDto newUserData);
}

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<IActionResult> UserRegister(UserDto newUserData)
    {

        //Encrypt the password
        string encryptedPassword = Argon2.Hash(newUserData.PasswordHash);
        newUserData.PasswordHash = encryptedPassword;

        //Create the actual entity
        User newUser = new()
        {
            Nickname = newUserData.Nickname ?? "<no-nickname>",
            PasswordHash = newUserData.PasswordHash ?? "<no-password>",
            Email = null,
            DateCreated = DateTime.Now,
            DateDeleted = null
        };

        //Write the new user into the database
        var result = await userRepository.UserRegister(newUser);

        return result;

    }
}