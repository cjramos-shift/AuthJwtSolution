using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AuthJwtSolution.Ports;
using Microsoft.IdentityModel.Tokens;

namespace AuthJwtSolution.Services
{
    public class AuthService : IAuth
    {
        private readonly IConfiguration _config;
        public AuthService(IConfiguration config)
        {
            _config = config;
        }
        public string JwtAuthHandler(string nome)
        {
            var handler = new JwtSecurityTokenHandler();
            
            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AuthSecretKey"])), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddMinutes(5),
                Claims = new Dictionary<string, object>
                {
                    { "nome", nome }
                }
            };

            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }
    }
}
