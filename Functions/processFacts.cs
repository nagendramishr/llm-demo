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
        public void Run([QueueTrigger("myqueue-items", Connection = "")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
