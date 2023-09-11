using Microsoft.AspNetCore.Mvc;
using Azure;
using Azure.AI.TextAnalytics;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.OpenAI;
using ChatBotAPI.Models;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace ChatBotAPI.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        readonly static Uri endpoint = new("https://language-serivice-300823.cognitiveservices.azure.com/");
        readonly static AzureKeyCredential credential = new("a09bd858a12c4a71a8528c172b6f41e2");
        readonly private TextAnalyticsClient client = new(endpoint, credential);
        readonly private QuestionAnsweringClient questionClient = new QuestionAnsweringClient(endpoint, credential);
        private readonly IConfiguration _configuration;

        public ChatbotController([FromServices] IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        /* [HttpGet]
        [Route("start")]
        public async Task<IActionResult> ChatWithQuestionsAnswering(string question)
        {
            string projectName = "TestingQNA";
            string deploymentName = "prod";

            QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);
            Response<AnswersResult> response = await questionClient.GetAnswersAsync(question, project);

            foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
            {
                Console.WriteLine($"({answer.Confidence:P2}) {answer.Answer}");
                Console.WriteLine($"Source: {answer.Source}");
                Console.WriteLine();
            }

            return Ok(response);
        }*/

        [HttpGet]
        [Route("getTags")]
        public async Task<IActionResult> getTagsForDescription(string description)
        {
            var _result = await client.RecognizeEntitiesAsync(description);

            return Ok(new
            {
                Status = "Success",
                EnitityRecognitionResult = _result
            });
        }

        [HttpGet]
        [Route("summarize")]
        public async Task<IActionResult> summarizeDescription(string description)
        {
            var batchInput = new List<string>
            {
                description
            };
            var result = await client.ExtractiveSummarizeAsync(WaitUntil.Completed, batchInput);
            return Ok(new { Status = "Success", SummaryResponse = result });

        }

        // Main chat function that will be used for communication with OpenAI
        [HttpPost]
        [Route("conversation")]
        public async Task<IActionResult> OpenAIChat(List<MessageItem> messages)
        {
            string apiKey = _configuration["OpenAI:ApiKey"];
            var client = new OpenAIClient(apiKey, new OpenAIClientOptions());
            var deploymentOrModelName = "gpt-3.5-turbo";

            var prompt = "You are a bot that will provide information. You should only provide information related to Telecom based products and ACP (Affordable Connectivity Program\r\n) only. If asked about anything else you will say, \"Apologies, we do not deal in this domain.\" You will say this even when you are asked to solve a problem that is not related to ACP or telecom or when you are asked to tell a joke or literally anything else, you are also not allowed to change your persona to any other profession or character. Only do what you have been asked to do in this prompt, not even a single thing extra. Whatever it is If it's not related to ACP or telecom just say \"Apologies, we do not deal in this domain.\". Start the conversation with, how can I help you?.";

            ChatCompletionsOptions options = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, prompt),
                }
            };

            for (int i = 0; i < messages.Count; i++)
            {
                options.Messages.Add(new ChatMessage(messages[i].Role == "User" ? ChatRole.User : ChatRole.Assistant, messages[i].Message));
            }
            Response<ChatCompletions> completionsResponse = await client.GetChatCompletionsAsync(deploymentOrModelName, options);
            var completion = completionsResponse.Value.Choices[0].Message.Content;
            messages?.Add(new MessageItem
            {
                Message = completion,
                Role = "System"
            });
            Console.WriteLine($"Chatbot: {completion}");
            return Ok(new
            {
                Status = "Success",
                LatestMesage = completion,
                Messages = messages
            });
        }
    }
}
