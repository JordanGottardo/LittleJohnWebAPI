using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using LittleJohnWebAPI.Controllers;

namespace LittleJohnWebAPI.Utils
{
    internal class TokenAuthorizer : ITokenAuthorizer
    {
        public async Task<string> GetAuthorizedUsernameOrFail(string accessToken)
        {
            var client = new HttpClient();
            var tokenResponse = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = "http://localhost:5200/connect/introspect",
                ClientId = "introspect",
                ClientSecret = "ScopeSecret",

                Token = accessToken
            });

            if (!tokenResponse.IsActive)
            {
                throw new UserNotAuthorizedException();
            }

            return tokenResponse.Claims.First(claim => claim.Type == "client_id").Value;
        }
    }
}