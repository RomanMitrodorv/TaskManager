using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Habit.FunctionalTest
{
    public class AutoAuthorizeMiddleware
    {
        public const string IDENTITY_ID = "c5114168-ad5f-4745-b1dd-5a424cec08aa";

        private readonly RequestDelegate _next;

        public AutoAuthorizeMiddleware(RequestDelegate rd)
        {
            _next = rd;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var identity = new ClaimsIdentity("cookies");

            identity.AddClaim(new Claim("sub", IDENTITY_ID));
            identity.AddClaim(new Claim("unique_name", IDENTITY_ID));
            identity.AddClaim(new Claim(ClaimTypes.Name, IDENTITY_ID));

            httpContext.User.AddIdentity(identity);

            await _next.Invoke(httpContext);
        }
    }
}
