using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace AzureFunction
{
    public static class GetFacts
    {
        [FunctionName("GetFacts")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "openai",
                containerName: "samfacts",
                SqlQuery = "SELECT * FROM c",
                Connection = "CosmosDBConnection")] IEnumerable<SamFact> allFacts,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            StringBuilder sb = new StringBuilder();

            foreach (var sf in allFacts) {
                log.LogInformation($"ID: {sf.id} Text: {sf.text}");
                var a = sf.text.Trim();
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
