using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TedsProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace TedsProject.Auth
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        public async Task<string> GenerateEncodedToken(UserModel userInfo)
        {
           
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: GenerateClaimsIdentity(userInfo).Claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return await Task.FromResult(encodedJwt);
        }

        public ClaimsIdentity GenerateClaimsIdentity(UserModel userInfo)
        {
  
            var claims = new Claim[]
            {

                new Claim(ClaimTypes.Name, userInfo.Id),
                new Claim(ClaimTypes.Email, userInfo.Id),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
                new Claim(JwtRegisteredClaimNames.NameId, userInfo.Id),
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Id, userInfo.Id),
               // new Claim("user_info", JsonConvert.SerializeObject(userInfo)),
               
            };
            return new ClaimsIdentity(new GenericIdentity(userInfo.Id, "Token"), claims);
        }

        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}
