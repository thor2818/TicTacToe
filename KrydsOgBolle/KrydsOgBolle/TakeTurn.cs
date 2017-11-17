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
    public static class TakeTurn
    {
        [FunctionName("TakeTurn")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [Table("gamestate", Connection = "AzureWebJobsStorage")]IQueryable<GameState> inTable,
            [Table("gamestate", Connection = "AzureWebJobsStorage")]CloudTable outTable,
            TraceWriter log)
        {
            dynamic data = await req.Content.ReadAsAsync<object>();
            string name = data?.player;
            string partitionkey = data?.partitionKey;
            int? pos = data?.position;

            if (string.IsNullOrEmpty(name))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name in the request body");
            }

            if (string.IsNullOrEmpty(partitionkey))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a partitionKey in the request body");
            }

            if (pos == null || pos < 0 || pos > 8)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a pos from 0 to 8 in the request body");
            }

            var query = from game in inTable
                        where game.PartitionKey == partitionkey
                        select game;

            GameState g = query.FirstOrDefault();
            char[] positions = g.Board.ToCharArray();
            if(positions[(int)pos] != '-')
                return req.CreateResponse(HttpStatusCode.BadRequest, g);


            if (g.PlayerTurn == 1)
            {
                g.PlayerTurn = 2;
                positions[(int)pos] = 'X';

            }
            else
            {
                g.PlayerTurn = 1;
                positions[(int)pos] = 'O';
            }
            g.Board = new string(positions);

            TableOperation updateOperation = TableOperation.Replace(g);
            TableResult result = outTable.Execute(updateOperation);
            return req.CreateResponse(HttpStatusCode.Created, g);
        }

        public class Person : TableEntity
        {
            public string Name { get; set; }
        }
    }
}
