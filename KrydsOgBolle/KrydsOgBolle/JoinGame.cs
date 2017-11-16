using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace KrydsOgBolle
{
    public static class JoinGame
    {
        [FunctionName("JoinGame")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequestMessage req,
            [Table("gamestate", Connection = "AzureWebJobsStorage")]IQueryable<GameState> inTable,
            [Table("gamestate", Connection = "AzureWebJobsStorage")]ICollector<GameState> outTable,
            TraceWriter log)
        {
            dynamic data = await req.Content.ReadAsAsync<object>();
            string name = data?.name;
            string partitionkey = data?.partitionkey;

            if (string.IsNullOrEmpty(name))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name in the request body");
            }

            if (string.IsNullOrEmpty(partitionkey))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a PartitionKey in the request body");
            }

            var query = from game in inTable
                        where game.PartitionKey == partitionkey
                        select game;

            GameState g = query.FirstOrDefault();

            g.Player2 = name;
 

            return req.CreateResponse(HttpStatusCode.Created, g);
        }

        public class Person : TableEntity
        {
            public string Name { get; set; }
        }
    }
}
