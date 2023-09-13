using ChatBotAPI.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ChatBotAPI.Services;

public class SendMessageResponse
{
    public string RecipientId { get; set; }
    public string MessageId { get; set; }
};

public class GraphAPIService
{
    private readonly HttpClient client;
    private readonly IConfiguration _configuration;

    public GraphAPIService(IConfiguration configuration)
    {
        this._configuration = configuration;
        var apiURL = this._configuration["Meta:apiUrl"];
        client = new HttpClient()
        {
            BaseAddress = new Uri(apiURL)
        };
        client.DefaultRequestHeaders
          .Accept
          .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<SendMessageResponse> SendMessage(string message, string senderId)
    {

        var pageId = _configuration["Meta:pageId"];
        var pageAccessToken = _configuration["Meta:pageAccessToken"];
        var url = string.Format("/{0}/messages?access_token={1}", pageId, pageAccessToken);
        var result = new SendMessageResponse();
        var payload = new
        {
            recipient = new
            {
                id = senderId
            },
            messaging_type = "RESPONSE",
            message = new
            {
                text = message
            }
        };
        string payloadJson = JsonSerializer.Serialize(payload);
        var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        if (response.IsSuccessStatusCode)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<SendMessageResponse>(stringResponse,
              new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        else
        {
            Console.WriteLine("EXCEPTION_ENCOUNTERED");
            throw new HttpRequestException(response.Content.ToString());
        }
        Console.WriteLine("MESSAGE_SENT");
        return result;
    }
}
