using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Extensions;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace Functions
{
    public class addPrompt
    {
        private readonly ILogger<addPrompt> _logger;

        public addPrompt(ILogger<addPrompt> logger)
        {
            _logger = logger;
        }

        [Function("addPrompt")]
        public async Task<PromptMultiResponse> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req
        )
        {
            _logger.LogInformation("addPrompt invoked");

            try {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(requestBody);
                string text = data.GetProperty("Text").GetString() ?? string.Empty;
                string title = data.GetProperty("Title").GetString() ?? string.Empty;
                string id = data.GetProperty("id").GetString() ?? string.Empty;
                bool delete = data.GetProperty("Delete").GetBoolean();

                var prompt = new Prompt() { Text = text, Title = title, Delete = delete };
                if (!string.IsNullOrEmpty(id))
                {
                    prompt.id = id;
                }

                // Return a response to both HTTP trigger and storage output binding.
                return new PromptMultiResponse()
                {
                    // Write a single message.
                    Prompt = [prompt ],
                    Result = new OkObjectResult("Prompt update received."),
                };
            }
            catch (Exception)
            {
                return new PromptMultiResponse()
                {
                    Result = new BadRequestObjectResult("Invalid request body."),
                };
            }

        }
    }

    public class PromptMultiResponse
    {
        [CosmosDBOutput(databaseName:"%CosmosDb%", containerName:"%PromptsContainer%", Connection = "dbstr", CreateIfNotExists = true)]
        public Prompt[]? Prompt { get; set; }
        [HttpResult]
        public required IActionResult Result { get; set; }
    }
}
