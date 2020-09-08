using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TedsProject.Application;

namespace TedsProject.Configurations
{

    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // IMyScopedService is injected into Invoke
        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Items.TryAdd("user", ProcessToken(httpContext));
            await _next(httpContext);
        }
        private AuthClaims ProcessToken(HttpContext _httpContext)
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)_httpContext.User.Identity;
                var jwtHandler = new JwtSecurityTokenHandler();
                ClaimsPrincipal user = _httpContext.User;
                var claims = user.Claims.ToList();

                var id = claims.Where(x => x.Type.StartsWith("id", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()?.Value?.ToString();
                var email = claims.Where(x => x.Type.EndsWith("nameidentifier", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()?.Value?.ToString();
                var apiKey = claims.Where(x => x.Type.Equals("apiKey", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()?.Value?.ToString();
                var expiry = claims.Where(x => x.Type.Equals("exp", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()?.Value?.ToString() ?? "0";

                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expiry));

                if (dateTimeOffset.LocalDateTime > DateTime.UtcNow)
                    return new AuthClaims { Id = id, Email = email, ApiKey = apiKey };
                else
                    return new AuthClaims();

                //AuthClaims jwtPayload = JsonConvert.DeserializeObject<AuthClaims>(nameIdentifier);

                //return jwtPayload;
            }
            catch
            {
                return new AuthClaims();
            }
        }


    }
}
