using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityModel.Client;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace asp.net_core_api_gateway.Controllers
{
    [Route("data")]
    [Authorize]
    public class DataController : Controller
    {
        private const string AuthorizationRequestHeader = "Authorization";

        public async Task<IActionResult> Get()
        {
            var userToken = "";
            var tokenResponse = await DelegateAsync(userToken);

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            var content = await client.GetStringAsync("http://localhost:5001/identity");

            return Json(content);
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

            return await client.RequestCustomGrantAsync("delegation", "administrationApi", payload);
        }
    }
}