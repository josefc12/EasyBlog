using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EasyBlog.Api.Models.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EasyBlog.Api.Services;

public class JwtService
{

    private JwtSettings _jwtSettings;
    private readonly byte[] _secretKey;
    
    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _secretKey = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
    }

    public string GenerateJwtToken(string userId, string username, string role = "User")
    {

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Nickname, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(_secretKey),
            SecurityAlgorithms.HmacSha256
        );
        
        var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiresInMinutes),
                signingCredentials: credentials
            );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}