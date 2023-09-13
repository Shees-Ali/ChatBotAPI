using Azure;
using Azure.AI.OpenAI;
using ChatBotAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotAPI.Services
{
    public class AIResponseService
    {
        private readonly IConfiguration _configuration;

        public AIResponseService([FromServices] IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public async Task<string> getMessage(List<MessageItem> messages)
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
            var message = completionsResponse.Value.Choices[0].Message.Content;
            return message;
        }
    }
}
