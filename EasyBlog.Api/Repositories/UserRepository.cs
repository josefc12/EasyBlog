using EasyBlog.Api.Data;
using EasyBlog.Api.Models;
using EasyBlog.Shared.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EasyBlog.Api.Repositories;

public interface IUserRepository
{
    Task<IActionResult> UserRegister(User newUser);
}

public class UserRepository(EasyBlogDbContext context) : IUserRepository
{
    public async Task<IActionResult> UserRegister(User newUser)
    {
        try
        {
            context.Users.Add(newUser);
            await context.SaveChangesAsync();
            return new OkResult();
        }
        catch (Exception e) 
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        

    }
}