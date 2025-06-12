using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Models;


namespace TodoApp.Utils;

public class JwtToken(IConfiguration configuration)
{
    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Email)
        };
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));

        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDesctiption = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppSettings:Issuer"),
            audience: configuration.GetValue<string>("AppSettings:Audience"),
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: cred
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDesctiption);

    }
}