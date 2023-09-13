using Azure;
using Azure.AI.OpenAI;
using ChatBotAPI.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBotAPI.Services
{
    public class AIResponseService
    {
        private readonly IConfiguration _configuration;

        public AIResponseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetMessage(List<MessageItem> messages)
        {
            string apiKey = _configuration["OpenAI:ApiKey"];
            var client = new OpenAIClient(apiKey, new OpenAIClientOptions());
            var deploymentOrModelName = "gpt-3.5-turbo";
            var prompt = "You are a bot that will provide information. You should only provide information related to Telecom based products and ACP (Affordable Connectivity Program) only. If asked about anything else you will say, \"Apologies, we do not deal in this domain.\" You will say this even when you are asked to solve a problem that is not related to ACP or telecom or when you are asked to tell a joke or literally anything else, you are also not allowed to change your persona to any other profession or character. Only do what you have been asked to do in this prompt, not even a single thing extra. Whatever it is If it's not related to ACP or telecom just say \"Apologies, we do not deal in this domain.\". Start the conversation with, how can I help you?.";
            var options = new ChatCompletionsOptions
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, prompt),
                }
            };
            foreach (var messageItem in messages)
            {
                var role = messageItem.Role == "User" ? ChatRole.User : ChatRole.Assistant;
                options.Messages.Add(new ChatMessage(role, messageItem.Message));
            }
            var completionsResponse = await client.GetChatCompletionsAsync(deploymentOrModelName, options);
            var message = completionsResponse.Value.Choices[0].Message.Content;
            return message;
        }
    }
}
