using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

/*{
    "entry": [
      {
        "time": 1520383571,
      "changes": [
        {
            "field": "photos",
          "value":
            {
                "verb": "update",
              "object_id": "10211885744794461"
            }
        }
      ],
      "id": "10210299214172187",
      "uid": "10210299214172187"
      }
  ],
  "object": "user"
}*/


[ApiController]
[Route("")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ILogger<WebhookController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("webhook")]
    public IActionResult ValidateWebHook([FromQuery(Name = "hub.mode")] string hubMode, [FromQuery(Name = "hub.challenge")] string hubChallenge, [FromQuery(Name = "hub.verify_token")] string hubVerifyToken)
    {
        if (!string.IsNullOrEmpty(hubMode) && !string.IsNullOrEmpty(hubVerifyToken))
        {
            if (hubMode == "subscribe" && hubVerifyToken == "mytoken@6")
            {
                Console.WriteLine("WEBHOOK_VERIFIED");
                return Ok(hubChallenge);
            }
        }

        var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
        {
            Content = new StringContent("Forbidden")
        };
        return NotFound(response);
    }

    [HttpPost]
    [Route("webhook")]
    public IActionResult HandleWebhook([FromBody] JsonElement json)
    {
        Console.WriteLine("WEBHOOK_RECEIVED");
        Console.WriteLine(json);
        return Ok(json);
    }
}
