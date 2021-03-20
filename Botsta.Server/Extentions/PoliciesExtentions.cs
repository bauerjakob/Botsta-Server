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


        public static AuthorizationPolicy UserPolicy(this AuthorizationPolicyBuilder builder)
        {
            return builder
                .RequireAuthenticatedUser()
                .RequireRole(User)
                .Build();
        }
    }
}
