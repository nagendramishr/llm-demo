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
    public class getPrompts
    {

        //[Function("HttpFunction")]
        [Function("getPrompts")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            [CosmosDBInput(
                databaseName: "%CosmosDb%",
                containerName: "%PromptsContainer%",
                SqlQuery = "SELECT * FROM c",
                Connection = "dbstr")] IEnumerable<Prompt> allPrompts,
            ILogger log)
        {
            List<string> prompts = new List<string>();

            foreach (var item in allPrompts) {
            //         log.LogInformation($"ID: {sf.id} Text: {sf.text}");
                var a = item.Text.Trim();
                if (a.EndsWith(".")) {
                    prompts.Add(a);
                }
                else {
                    prompts.Add(a + ".");
                }
            }

            return new JsonResult(prompts);

            //return new  OkObjectResult(sb.ToString());
        }
    }
}
