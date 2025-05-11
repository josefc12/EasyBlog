using Microsoft.IdentityModel.JsonWebTokens;

namespace EasyBlog.Api.Models.Memory;

public class AuthTokens()
{
    public string? JwtToken {get;set;}
    public string? RefreshToken {get;set;} 
}