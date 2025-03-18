using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace AuthJwtSolution.Ports
{
    public interface IAuth
    {
        public string JwtAuthHandler(string nome);
    }
}
