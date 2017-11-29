using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace asp.net_core_api_gateway.Controllers
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : Controller
    {
        private const string AuthorizationRequestHeader = "Authorization";

        public async Task<IActionResult> Get()
        {
            var userToken = ExtractTokenFromAuthenticationHeader(HttpContext.Request.Headers);
            var tokenResponse = await DelegateAsync(userToken);

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            var content = await client.GetStringAsync("http://localhost:5000/api/usermanagement");

            return Json(content);
        }

        private string ExtractTokenFromAuthenticationHeader(IDictionary<string, StringValues> receivedHeaders)
        {
            if (receivedHeaders.ContainsKey(AuthorizationRequestHeader))
            {
                return receivedHeaders[AuthorizationRequestHeader][0].Split(' ').Last();
            }
            return String.Empty;
        }

        private async Task<TokenResponse> DelegateAsync(string userToken)
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                return null;
            }

            var payload = new
            {
                token = userToken
            };

            var client = new TokenClient(disco.TokenEndpoint, "apiGateway.client", "secret");

            return await client.RequestCustomGrantAsync("delegation", "identityApi", payload);
        }
    }
}