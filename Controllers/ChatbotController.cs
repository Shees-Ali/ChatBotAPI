using Microsoft.AspNetCore.Mvc;
using Azure;
using Azure.AI.TextAnalytics;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.OpenAI;
using ChatBotAPI.Models;

namespace ChatBotAPI.Controllers
{
    [Route("api/chat")]
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

        [HttpGet]
        [Route("analyze")]
        public async Task<IActionResult> TextSummarizationExample()
        {
            string document = @"The extractive summarization feature in Text Analytics uses natural language processing techniques to locate key sentences in an unstructured text document. 
                These sentences collectively convey the main idea of the document. This feature is provided as an API for developers. 
                They can use it to build intelligent solutions based on the relevant information extracted to support various use cases. 
                In the public preview, extractive summarization supports several languages. It is based on pretrained multilingual transformer models, part of our quest for holistic representations. 
                It draws its strength from transfer learning across monolingual and harness the shared nature of languages to produce models of improved quality and efficiency.";

            var batchInput = new List<string>
            {
                document
            };
            var result = await client.AbstractiveSummarizeAsync(WaitUntil.Completed, batchInput);
            var analysis = await client.AnalyzeSentimentAsync(document);
            var _result = await client.RecognizeEntitiesAsync(document);
            return Ok(new { Status = "Success", SummaryResponse = result, AnalysisResponse = analysis, EnitityRecognitionResult = _result });
        }

        [HttpGet]
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
        }

        // Main chat function that will be used for communication with OpenAI
        [HttpPost]
        [Route("conversation")]
        public async Task<IActionResult> OpenAIChat(List<MessageItem> messages)
        {
            string apiKey = _configuration["OpenAI:ApiKey"];
            var client = new OpenAIClient(apiKey, new OpenAIClientOptions());
            var deploymentOrModelName = "gpt-3.5-turbo-16k-0613";
            ChatCompletionsOptions options = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are a bot for a task manager. You will help with task management. Answer with Hello, I am here to help you with task management."),
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
