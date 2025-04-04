using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Extensions;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.Buffers;

namespace Functions
{
    public class addBoard
    {
        private readonly ILogger<addBoard> _logger;

        public addBoard(ILogger<addBoard> logger)
        {
            _logger = logger;
        }

        [Function("addBoard")]
        public async Task<BoardMultiResponse> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req
        )
        {
            _logger.LogInformation("addBoard invoked");

            try {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(requestBody);

                // Uses the JSON Element extension methods to get the properties.
                string owner = data.GetPropertyOrDefault("Owner", string.Empty);
                string title = data.GetPropertyOrDefault("Title", string.Empty);
                string id = data.GetGuidPropertyOrDefault("id", Guid.NewGuid()).ToString();
                bool delete = data.GetPropertyOrDefault("Delete", false);
                DateTime created = data.GetPropertyOrDefault("Created", DateTime.UtcNow);

                // Create a new Board object with the properties.
                _logger.LogInformation($"Creating new Board with id: {id} title: {title} delete: {delete} owner: {owner} created: {created}"); 
                var Board = new Board() { Title = title, Delete = delete, Owner = owner, Created = created};

                // Return a response to both HTTP trigger and storage output binding.
                return new BoardMultiResponse()
                {
                    // Write a single message.
                    Board = [Board ],
                    Result = new OkObjectResult("Board update received."),
                };
            }
            catch (Exception)
            {
                return new BoardMultiResponse()
                {
                    Result = new BadRequestObjectResult("Invalid request body."),
                };
            }

        }
    }

    public class BoardMultiResponse
    {
        [CosmosDBOutput(databaseName:"%CosmosDb%", containerName:"%BoardsContainer%", Connection = "dbstr", CreateIfNotExists = true)]
        public Board[]? Board { get; set; }
        [HttpResult]
        public required IActionResult Result { get; set; }
    }
}
