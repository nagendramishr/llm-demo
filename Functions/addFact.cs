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
    public class addFact
    {
        private readonly ILogger<addFact> _logger;

        public addFact(ILogger<addFact> logger)
        {
            _logger = logger;
        }

        [Function("addFact")]
        public async Task<MultiResponse> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req
        )
        {
            _logger.LogInformation("addFact invoked");

            try {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(requestBody);
                string message = data.GetProperty("message").GetString();

                // Return a response to both HTTP trigger and storage output binding.
                return new MultiResponse()
                {
                    // Write a single message.
                    Messages = [message ],
                    Result = new OkObjectResult("Message enqueued."),
                };
            }
            catch (Exception)
            {
                return new MultiResponse()
                {
                    Result = new BadRequestObjectResult("Invalid request body."),
                };
            }

        }
    }

    public class MultiResponse
    {
        [QueueOutput("output-queue", Connection = "Queue")]
        public string[] Messages { get; set; }
        [HttpResult]
        public IActionResult Result { get; set; }    }
}
