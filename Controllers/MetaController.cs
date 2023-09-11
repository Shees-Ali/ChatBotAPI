using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;

namespace ChatBotAPI.Controllers
{
    [Route("api/webhook")]
    [ApiController]
    public class MetaController : ControllerBase
    {

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> WebHook([FromQuery(Name = "hub.mode")] string hubMode, [FromQuery(Name = "hub.challenge")] string hubChallenge, [FromQuery(Name = "hub.verify_token")] string hubVerifyToken)
        {
            // Check if a token and mode are in the query string of the request
            if (!string.IsNullOrEmpty(hubMode) && !string.IsNullOrEmpty(hubVerifyToken))
            {
                // Check the mode and token sent are correct
                if (hubMode == "subscribe" && hubVerifyToken == "mytoken@6")
                {
                    // Respond with the challenge token from the request
                    System.Diagnostics.Debug.WriteLine("WEBHOOK_VERIFIED");
                    return Ok(hubChallenge);
                }
            }

            // Return a 403 Forbidden response if verify tokens do not match
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Forbidden")
            };
            return NotFound(response);
        }
    }
}
