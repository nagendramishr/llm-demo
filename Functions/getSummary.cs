using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions;
using Microsoft.Azure.Functions.Worker.Extensions.CosmosDB;
using System.Text.Json;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Chat;


using static System.Environment;

namespace Functions
{
    public class getSummary
    {
        private readonly ILogger<getSummary> _logger;

        public getSummary(ILogger<getSummary> logger)
        {
            _logger = logger;
        }

        [Function("getSummary")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route ="getFacts/{bid}")] HttpRequest req,
            string bid,
            [CosmosDBInput(
                databaseName: "%CosmosDb%",
                containerName: "%CosmosContainer%",
                SqlQuery = "SELECT * FROM c where c.Delete = false",// and c.boardId = '{bid}'",
                Connection = "dbstr")] IEnumerable<SamFact> allFacts,
            ILogger log
        )
        {
            int allFactCount = allFacts?.Count() ?? 0;
            _logger.LogInformation($"GetSummary: Retrieved {allFactCount} facts from CosmosDB");

            // filter out the facts that do not match the boardId
            List<SamFact> filteredFacts = new List<SamFact>();
            foreach (var sf in allFacts)
            {
                if (sf.BoardId == bid)
                {
                    filteredFacts.Add(sf);
                }
            }

            // Parse the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            _logger.LogInformation("GetSummary: Request body: {requestBody}", requestBody);
            _logger.LogInformation("GetSummary: Retrieving facts for boardId: {bid}", bid);

            // Log CosmosDB query information
            int factCount = filteredFacts?.Count() ?? 0;
            _logger.LogInformation("GetSummary: Retrieved {factCount} facts from CosmosDB", factCount);

            if (factCount == 0)
            {
                _logger.LogWarning("GetSummary: No facts found for boardId: {bid}. Check the query parameters.", bid);
            }
            else
            {
                // Log a few sample facts (up to 3)
                int samplesToLog = Math.Min(factCount, 3);
                for (int i = 0; i < samplesToLog; i++)
                {
                    _logger.LogInformation("GetSummary: Sample fact {i}: {message}", i, filteredFacts.ElementAt(i).Message);
                }
            }

            var data = JsonSerializer.Deserialize<Query>(requestBody);
            if (data == null || string.IsNullOrEmpty(data.Text))
            {
                data = new() { Text = "Please provide a valid request body with 'Text' property." };
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(data.Text);
            sb.AppendLine("Facts: ");

            foreach (var sf in filteredFacts)
            {
                var a = sf.Message.Trim();
                if (a.EndsWith(".") || a.EndsWith("?") || a.EndsWith("!"))
                {
                    sb.Append(a + " ");
                }
                else
                {
                    sb.Append(a + ". ");
                }

                sb.AppendLine();
            }

            var response = await RunQueryAsync(sb.ToString());

            return new OkObjectResult(response);
        }

        async Task<string> RunQueryAsync(string prompt)
        {
            _logger.LogInformation("Running with prompt: {prompt}", prompt);

            // Retrieve the OpenAI endpoint from environment variables
            var endpoint = GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "";
            if (string.IsNullOrEmpty(endpoint))
            {
                return "Please set the AZURE_OPENAI_ENDPOINT environment variable.";
            }

            // Use DefaultAzureCredential for Entra ID authentication
            var credential = new DefaultAzureCredential();
            AzureOpenAIClient azureClient;
            try
            {
                // Initialize the AzureOpenAIClient
                azureClient = new AzureOpenAIClient(new Uri(endpoint), credential);
            }
            catch (Exception e)
            {
                return $"Error initializing AzureOpenAIClient: {e.Message}";
            }
            // Initialize the ChatClient with the specified deployment name
            ChatClient chatClient = azureClient.GetChatClient("gpt-4o");

            // Create a list of chat messages
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(prompt),
            };

            // Create chat completion options  
            var options = new ChatCompletionOptions
            {
                Temperature = (float)0.7,
                MaxOutputTokenCount = 1200,

                TopP = (float)0.95,
                FrequencyPenalty = (float)0,
                PresencePenalty = (float)0
            };

            try
            {
                // Create the chat completion request
                ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);

                // Print the response
                if (completion != null)
                {
                    return JsonSerializer.Serialize(completion, new JsonSerializerOptions() { WriteIndented = true });
                }
                else
                {
                    return "No response received.";
                }
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }
    }
}
