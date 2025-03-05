using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json; 

namespace Functions
{
    public class processFacts
    {
        private readonly ILogger<processFacts> _logger;

        public processFacts(ILogger<processFacts> logger)
        {
            _logger = logger;
        }

        [Function(nameof(processFacts))]
        [CosmosDBOutput(databaseName:"%CosmosDb%", containerName:"%CosmosContainer%", Connection = "dbstr", CreateIfNotExists = true)]
        public string? Run([QueueTrigger("output-queue", Connection = "Queue")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processing : {message.MessageText}");

            if (!string.IsNullOrEmpty( message.MessageText) ) {
                SamFact sf = new ();
                sf.Message = message.MessageText.Trim();
                if (!sf.Message.EndsWith(".")) {
                    sf.Message = sf.Message + ".";
                }
                _logger.LogInformation($"Processed: {sf.Message}");
                return JsonSerializer.Serialize(sf);
            }
            else {
                _logger.LogError("Message is empty");
                return null;
            }
        }
    }
}
