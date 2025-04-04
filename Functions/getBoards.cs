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
    public class getBoards
    {

        //[Function("HttpFunction")]
        [Function("getBoards")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            [CosmosDBInput(
                databaseName: "%CosmosDb%",
                containerName: "%BoardsContainer%",
                SqlQuery = "SELECT * FROM c where c.Delete = false",
                Connection = "dbstr")] IEnumerable<Board> allBoards,
            ILogger log)
        {

            return new JsonResult(allBoards);

            //return new  OkObjectResult(sb.ToString());
        }
    }
}
