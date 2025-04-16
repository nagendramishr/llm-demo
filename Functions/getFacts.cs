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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="{bid}")] HttpRequest req,
            [CosmosDBInput(
                databaseName: "%CosmosDb%",
                containerName: "%CosmosContainer%",
                SqlQuery = "SELECT * FROM c where c.Delete = false and c.boardId = '{bid}'",
                Connection = "dbstr")] IEnumerable<SamFact> allFacts,
            ILogger log)
        {
            List<string> facts = new List<string>();

            foreach (var sf in allFacts) {
            //         log.LogInformation($"ID: {sf.id} Text: {sf.text}");
                var a = sf.Message.Trim();
                if (a.EndsWith(".")) {
                    facts.Add(a);
                }
                else {
                    facts.Add(a + ".");
                }
            }

            return new JsonResult(facts);

            //return new  OkObjectResult(sb.ToString());
        }
    }
}
