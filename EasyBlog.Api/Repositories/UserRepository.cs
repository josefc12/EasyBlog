using EasyBlog.Api.Data;
using EasyBlog.Api.Models;
using EasyBlog.Shared.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyBlog.Api.Repositories;

public interface IUserRepository
{
    Task<IActionResult> UserRegister(User newUser);
    Task<IActionResult> GetUser(string nickname);
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
    
    public async Task<IActionResult> GetUser(string nickname)
    {
        try
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Nickname == nickname);
            if (user == null)
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            return new OkObjectResult(user);
        }
        catch (Exception e) 
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}