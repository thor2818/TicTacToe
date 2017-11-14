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
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "put")]GameState gamestate, [Table("gamestate", Connection = "AzureWebJobsStorage")]CloudTable outTable, TraceWriter log)
        {
            if (string.IsNullOrEmpty(gamestate.Player1))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("A non-empty Name must be specified.")
                };
            };

            log.Info($"Game started by {gamestate.Player1}");
            gamestate.PartitionKey = Guid.NewGuid().ToString();
            gamestate.RowKey = "";
            //her skal vi sætte brættet til et tomt bræt
            // gamestate.BoardRow1 = "---";
            // gamestate.BoardRow2 = "---";
            // gamestate.BoardRow3 = "---";
            TableOperation updateOperation = TableOperation.InsertOrReplace(gamestate);
            TableResult result = outTable.Execute(updateOperation);
            return new HttpResponseMessage((HttpStatusCode)result.HttpStatusCode);
        }

    }
}
