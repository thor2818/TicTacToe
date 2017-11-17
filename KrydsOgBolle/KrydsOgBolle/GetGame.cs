using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace KrydsOgBolle
{
    public static class GetGame
    {
        [FunctionName("GetGame")]
        public static async Task<HttpResponseMessage > Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req, [Table("gamestate", Connection = "AzureWebJobsStorage")]IQueryable<GameState> inTable, TraceWriter log)
        {
            dynamic data = await req.Content.ReadAsAsync<object>();
            string partitionkey = data?.gameid;

            if (string.IsNullOrEmpty(partitionkey))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name in the request body");
            }
            var query = from game in inTable where game.PartitionKey == partitionkey select game;
            return req.CreateResponse(HttpStatusCode.OK, query.FirstOrDefault());
        }
    }
    
}
