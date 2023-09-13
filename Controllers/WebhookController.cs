using Azure.AI.OpenAI;
using ChatBotAPI.Models;
using ChatBotAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

public class Entry
{
    public string id { get; set; }
    public long time { get; set; }
    public List<Messaging> messaging { get; set; }
}

public class Message
{
    public string mid { get; set; }
    public string text { get; set; }
    public Tags tags { get; set; }
}

public class Messaging
{
    public Sender sender { get; set; }
    public Recipient recipient { get; set; }
    public long timestamp { get; set; }
    public Message message { get; set; }
}

public class Recipient
{
    public string id { get; set; }
}

public class Root
{
    public string @object { get; set; }
    public List<Entry> entry { get; set; }
}

public class Sender
{
    public string id { get; set; }
}

public class Tags
{
    public string source { get; set; }
}

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
    private readonly AIResponseService aIResponseService;

    public WebhookController(ILogger<WebhookController> logger, [FromServices] AIResponseService _aIResponseService)
    {
        _logger = logger;
        aIResponseService = _aIResponseService;
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
    public async Task<IActionResult> HandleWebhook([FromBody] JsonElement json)
    {
        Console.WriteLine("WEBHOOK_RECEIVED");
        Console.WriteLine($"{json.ToString()}");
        Root message = json.Deserialize<Root>();
        Console.WriteLine(message?.entry[0].messaging[0].message.text);
        // goes to gpt service gets message response
        List<MessageItem> messages = new List<MessageItem> { };
        messages?.Add(new MessageItem
        {
            Message = message?.entry[0].messaging[0].message.text,
            Role = "User"
        });
        var _message = await aIResponseService.getMessage(messages);
        Console.WriteLine(_message);
        //

        // calls graphAPIservice to send the message
        return Ok(json);
    }
}
