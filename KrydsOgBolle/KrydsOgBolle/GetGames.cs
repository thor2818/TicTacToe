using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

namespace KrydsOgBolle
{
    public static class GetGames
    {
        [FunctionName("GetGames")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get")]HttpRequestMessage req, [Table("gamestate", Connection = "AzureWebJobsStorage")]IQueryable<GameState> inTable, TraceWriter log)
        {
            var query = from game in inTable select game;
            return req.CreateResponse(HttpStatusCode.OK, inTable.ToList());
        }
    }
    
}
