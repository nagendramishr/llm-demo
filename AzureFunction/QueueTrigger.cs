using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AzureFunction
{
    public class QueueTrigger
    {
        [FunctionName("QueueTrigger")]
        public async Task Run([QueueTrigger("newsamfact", Connection = "nvmstrgqueue")]string myQueueItem,
                        [CosmosDB(
                            databaseName: "openai",
                            containerName: "samfacts",
                            Connection = "CosmosDBConnection")]IAsyncCollector<SamFact> shipmentsOut,
                        ILogger log)
        {
            //log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var sf = JsonConvert.DeserializeObject<SamFact>(myQueueItem);

            try {
                var s = new SamFact() {
                    id = sf.id, 
                    text = sf.text
                    };

                await shipmentsOut.AddAsync(s);
                log.LogInformation( $"ID: {s.id} Text: {s.text}");
                return;
            }
            catch (Exception e) {
                log.LogError(e.Message);
            }
        }
    }
}
