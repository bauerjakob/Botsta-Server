using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Botsta.Server.Extentions
{
    public static class PoliciesExtentions
    {
        public const string User = "User";
        public const string Bot = "Bot";
        public const string RefreshToken = "RefreshToken";


        public static AuthorizationPolicy UserPolicy(this AuthorizationPolicyBuilder builder)
        {
            return builder
                .RequireAuthenticatedUser()
                .RequireRole(User)
                .Build();
        }

        public static AuthorizationPolicy BotPolicy(this AuthorizationPolicyBuilder builder)
        {
            return builder
                .RequireAuthenticatedUser()
                .RequireRole(Bot)
                .Build();
        }

        public static AuthorizationPolicy RefreshTokenPolicy(this AuthorizationPolicyBuilder builder)
        {
            return builder
                .RequireAuthenticatedUser()
                .RequireRole(RefreshToken)
                .Build();
        }
    }
}
