using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace Piedone.Facebook.Suite
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public static class AuthenticationServiceExtensions
    {
        public static bool IsAuthenticated(this IAuthenticationService service)
        {
            return service.GetAuthenticatedUser() != null;
        }
    }
}