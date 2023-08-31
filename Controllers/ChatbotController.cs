using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure;
using Azure.AI.TextAnalytics;

namespace ChatBotAPI.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        readonly static Uri endpoint = new("https://language-serivice-300823.cognitiveservices.azure.com/");
        readonly static AzureKeyCredential credential = new("a09bd858a12c4a71a8528c172b6f41e2");
        private TextAnalyticsClient client = new(endpoint, credential);


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
    }
}
