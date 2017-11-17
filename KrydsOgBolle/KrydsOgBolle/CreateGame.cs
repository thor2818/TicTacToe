using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

namespace KrydsOgBolle
{
    public static class CreateGame
    {
        [FunctionName("CreateGame")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req, [Table("gamestate", Connection = "AzureWebJobsStorage")]CloudTable outTable, TraceWriter log)
        {
            dynamic data = await req.Content.ReadAsAsync<object>();
            string name = data?.name;

            if (string.IsNullOrEmpty(name))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("A non-empty Name must be specified.")
                };
            };

            log.Info($"Game started by {name}");
            GameState gamestate = new GameState();
            gamestate.PartitionKey = Guid.NewGuid().ToString();
            gamestate.RowKey = "";
            gamestate.Player1 = name;
            gamestate.PlayerTurn = 1;
            gamestate.Board = "---------";

            TableOperation updateOperation = TableOperation.InsertOrReplace(gamestate);
            TableResult result = outTable.Execute(updateOperation);
            return req.CreateResponse(HttpStatusCode.OK, gamestate);
        }
            
    }
}
