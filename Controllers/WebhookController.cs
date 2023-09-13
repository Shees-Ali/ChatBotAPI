using ChatBotAPI.Models;
using ChatBotAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

[ApiController]
[Route("")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;
    private readonly AIResponseService aIResponseService;
    private readonly GraphAPIService graphAPIService;
    public WebhookController(ILogger<WebhookController> logger, AIResponseService _aIResponseService, GraphAPIService _graphAPIService)
    {
        _logger = logger;
        aIResponseService = _aIResponseService;
        graphAPIService = _graphAPIService;
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
        MetaMessageResponse message = json.Deserialize<MetaMessageResponse>();
        Console.WriteLine(message?.entry[0].messaging[0].message.text);
        // goes to gpt service gets message response
        List<MessageItem> messages = new List<MessageItem> { };
        messages?.Add(new MessageItem
        {
            Message = message?.entry[0].messaging[0].message.text,
            Role = "User"
        });
        var model_response = await aIResponseService.GetMessage(messages);
        Console.WriteLine(model_response);
        Console.WriteLine(message.entry[0].messaging[0].sender.id);
        // calls graphAPIservice to send the message
        var api_response = await graphAPIService.SendMessage(model_response, message.entry[0].messaging[0].sender.id);
        Console.WriteLine("API_RESPONSE_RECEIVED");
        Console.WriteLine(api_response.MessageId);
        return Ok(json);
    }
}
