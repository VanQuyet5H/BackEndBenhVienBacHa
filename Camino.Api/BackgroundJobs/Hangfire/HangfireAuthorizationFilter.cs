using System;
using System.Linq;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Helpers;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace Camino.Api.BackgroundJobs.Hangfire
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private static readonly string HangFireCookieName = "HangFireCookie";
        private static readonly int CookieExpirationMinutes = 60;
        private readonly string _signingKey;

        public HangfireAuthorizationFilter(string signingKey)
        {
            _signingKey = signingKey;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            string accessToken;
            var setCookie = false;

            // try to get token from query string
            if (httpContext.Request.Query.ContainsKey("access_token"))
            {
                accessToken = httpContext.Request.Query["access_token"].FirstOrDefault();
                setCookie = true;
            }
            else
            {
                accessToken = httpContext.Request.Cookies[HangFireCookieName];
            }

            if (String.IsNullOrEmpty(accessToken))
            {
                return false;
            }

            JwtTokenValidator jwtTokenValidator = new JwtTokenValidator(new JwtTokenHandler(null));
            var claims = jwtTokenValidator.GetPrincipalFromToken(accessToken, _signingKey, true);
            var userIdClaim = claims?.Claims.FirstOrDefault(o => o.Type == Constants.JwtClaimTypes.Id);
            if (userIdClaim == null || userIdClaim.Value.ParseToInt() != 1)
            {
                return false;
            }

            if (setCookie)
            {
                httpContext.Response.Cookies.Append(HangFireCookieName, accessToken,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(CookieExpirationMinutes)
                });
            }
            return true;
        }
    }
}
