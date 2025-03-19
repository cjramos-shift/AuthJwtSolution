using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthSolution.Model;
using AuthSolution.Ports;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthSolution.Services;

public class AuthService : IAuth
{
    private readonly IConfiguration _config;
    public AuthService(IConfiguration config)
    {
        _config = config;
    }


    public string JwtAuthHandler(string userName)
    {
        var handler = new JwtSecurityTokenHandler();

        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AuthSecretKey"])), SecurityAlgorithms.HmacSha256);

        var user = new User() { Name = userName, Role = new string[] { "admin" } };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddMinutes(5),
            Subject = CreateClaims(user)
        };

        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }

    internal static ClaimsIdentity CreateClaims(User user)
    {
        var ci = new ClaimsIdentity();

        ci.AddClaim(new Claim(type: "id", user.Id.ToString()));
        ci.AddClaim(new Claim(ClaimTypes.Name, user.Name));
        ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        ci.AddClaim(new Claim(type: "password", user.Password));

        foreach (var role in user.Role)
            ci.AddClaim(new Claim(ClaimTypes.Role, role));

        return ci;
    }
}
