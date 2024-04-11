using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Auth_API
{
    public static class SD
    {
        public const string AdminRole = "Admin";
        public const string ManagerRole = "Manager";
        public const string PlayerRole = "Player";

        public const string AdminUserName = "admin@example.com";
        public const string SuperAdminChangeNotAllowed = "Super Admin change is not allowed";

        public const int MaximumLoginAttempts = 3;

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
