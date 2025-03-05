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

namespace Functions
{
    public class getFacts
    {

        //[Function("HttpFunction")]
        [Function("getFacts")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            [CosmosDBInput(
                databaseName: "%CosmosDb%",
                containerName: "%CosmosContainer%",
                SqlQuery = "SELECT * FROM c",
                Connection = "dbstr")] IEnumerable<SamFact> allFacts,
            ILogger log)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var sf in allFacts) {
            //         log.LogInformation($"ID: {sf.id} Text: {sf.text}");
                var a = sf.Message.Trim();
                if (a.EndsWith(".")) {
                    sb.Append(a+ " ");
                }
                else {
                    sb.Append(a + ". ");
                }
            }

            return new OkObjectResult(sb.ToString());
        }
    }
}
