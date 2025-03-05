using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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
        public SamFact? Run([QueueTrigger("output-queue", Connection = "Queue")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");

            if (!string.IsNullOrEmpty( message.MessageText) ) {
                SamFact sf = new ();
                sf.Id = Guid.NewGuid().ToString();
                sf.Message = message.MessageText.Trim();
                if (!sf.Message.EndsWith(".")) {
                    sf.Message = sf.Message + ".";
                }

                return sf;
            }
            else {
                return null;
            }
        }
    }
}
