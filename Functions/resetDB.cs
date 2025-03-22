using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class resetDB
    {
        private readonly ILogger<resetDB> _logger;

        public resetDB(ILogger<resetDB> logger)
        {
            _logger = logger;
        }

        [Function("resetDB")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,
                    [CosmosDBInput(
                databaseName: "%CosmosDb%",
                containerName: "%CosmosContainer%",
                SqlQuery = "select * FROM c",
                Connection = "dbstr")] IEnumerable<SamFact> allFacts,
                    [CosmosDBOutput(
                databaseName: "%CosmosDb%",
                containerName: "%CosmosContainer%",
                Connection = "dbstr")] IAsyncCollector<SamFact> outFacts)

        {
            int count = 0;
            foreach (var fact in allFacts)
            {
                fact.delete = true;
                await outFacts.AddAsync(fact);
                count++;
            }

            return new OkObjectResult($"{count} item(s) updated with delete=true.");
        }
    }
}
