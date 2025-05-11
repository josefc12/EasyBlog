using EasyBlog.Api.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyBlog.Api.Repositories;

public interface IAuthRepository
{
    Task<IActionResult> FindExistingRefreshToken(string? oldHashedRefreshToken = null, string? userId = null);
    Task<IActionResult> SaveExistingRefreshToken(RefreshToken existingRefreshToken, string hashedRefreshToken);
}

public class AuthRepository : IAuthRepository
{

    private EasyBlogDbContext _context;

    public AuthRepository(EasyBlogDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> FindExistingRefreshToken(string? oldHashedRefreshToken = null, string? userId = null)
    {
        var foundRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == oldHashedRefreshToken && !t.IsRevoked);
            
        if (userId != null && oldHashedRefreshToken == null)
        {
            foundRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsRevoked);
        }
        
        return new OkObjectResult(foundRefreshToken);
    }

    public async Task<IActionResult> SaveExistingRefreshToken(RefreshToken existingRefreshToken, string hashedRefreshToken)
    {
        try 
        {
            existingRefreshToken.Token = hashedRefreshToken;
            existingRefreshToken.DateCreated = DateTime.UtcNow;
            existingRefreshToken.DateExpires = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new OkResult();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        
    }

}