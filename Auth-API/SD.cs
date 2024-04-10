using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Auth_API
{
    public static class SD
    {
        public const string AdminRole = "Admin";
        public const string ManagerRole = "Manager";
        public const string PlayerRole = "Player";

        public static bool VIPPolicy(AuthorizationHandlerContext authorizationHandlerContext)
        {
            if( authorizationHandlerContext.User.IsInRole(PlayerRole) && 
                authorizationHandlerContext.User.HasClaim(c => c.Type == ClaimTypes.Email && c.Value.Contains("vip"))) 
            {
                return true;
            }
            return false;
        }
    }
}
