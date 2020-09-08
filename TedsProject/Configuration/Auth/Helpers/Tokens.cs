using System.Threading.Tasks;
using Newtonsoft.Json;
using TedsProject.Auth;
using TedsProject.Models;

namespace TedsProject.Helpers
{
    public class Tokens
    {
      public static async Task<string> GenerateJwt(IJwtFactory jwtFactory, UserModel userInfo, string apiKey, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
      {
        var response = new
        {
          id = userInfo.Id,
          accessToken = await jwtFactory.GenerateEncodedToken(userInfo, apiKey),
          apiKey = apiKey,
          expiresIn = (int)jwtOptions.ValidFor.TotalSeconds
        };

        return JsonConvert.SerializeObject(response, serializerSettings);
      }
    }
}
