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
            List<Prompt> prompts = new List<Prompt>();

            foreach (var item in allPrompts) {
                var p = new Prompt();
            //         log.LogInformation($"ID: {sf.id} Text: {sf.text}");
                var text  = item.Text.Trim();
                if (!text.EndsWith(".")) {
                    text += ".";
                }
                p.Text = text;
                p.Title = item.Title.Trim();

                prompts.Add(p);
            }
 
            return new JsonResult(prompts);

            //return new  OkObjectResult(sb.ToString());
        }
    }
}
