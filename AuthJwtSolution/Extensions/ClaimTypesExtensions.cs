using System.Security.Claims;

namespace AuthJwtSolution.Extensions
{
    public static class ClaimTypesExtensions
    {
        public static string GetId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue("id");
        }

        public static string GetName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Name);
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Email);
        }

        public static string GetPassword(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue("password");
        }

        public static string[] GetRoles(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
        }
    }
}
