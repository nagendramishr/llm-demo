using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace AzureFunction
{
    public class addFact
    {
        private readonly ILogger<addFact> _logger;

        public addFact(ILogger<addFact> logger)
        {
            _logger = logger;
        }

        [Function("addFact")]
        [QueueOutput("output-queue")]
        public static async Task<IActionResult> Run(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Queue("factqueue", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> factQueue,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string fact = data?.fact;
            
            if (string.IsNullOrEmpty(fact))
            {
                return new BadRequestObjectResult("Please pass a fact in the request body");
            }

            await factQueue.AddAsync(fact);

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
