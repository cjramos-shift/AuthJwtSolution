using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace AuthSolution.Ports;

public interface IAuth
{
    public string JwtAuthHandler(string nome);
}
