using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public class resetDB
    {
        private readonly ILogger<resetDB> _logger;

        public resetDB(ILogger<resetDB> logger)
        {
            _logger = logger;
        }

        [Function("resetDB")]

        public MultiResponse2 Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,
            [CosmosDBInput(
                databaseName: "%CosmosDb%",
                containerName: "%CosmosContainer%",
                SqlQuery = "select * FROM c",
                Connection = "dbstr")] IEnumerable<SamFact> allFacts)
        {
            MultiResponse2 mr = new MultiResponse2();

            int count = 0;
            foreach (var fact in allFacts)
            {
                fact.Delete = true;
                mr.records.Add(fact);
                count++;
            }

            mr.Result= new OkObjectResult($"{count} item(s) updated with delete=true.");
            return mr;
        }
    }

    public class MultiResponse2
    {
        [HttpResult]
        public IActionResult Result { get; set; } = new OkObjectResult("No records found.");

        [CosmosDBOutput(
            databaseName: "%CosmosDb%",
            containerName: "%CosmosContainer%",
            Connection = "dbstr")] 
        public List<object> records  {get; set; } = new List<object>();

        public MultiResponse2()
        {
            Result = new OkObjectResult("No records found.");
            records = new List<object>();
        }
        
    }
}
