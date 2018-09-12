
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents.Client;
using System;
using Microsoft.Azure.Documents;
using System.Text;
using System.Linq;

namespace FunctionApp3
{
    public static class GetRatings
    {
        

        [FunctionName("GetRatings")]
        public static HttpResponseMessage GetList([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string userId = req.Query["userId"];
            var jsonToReturn = "";

            var databaseId = "Openhackdb";
            var collectionId = "Ratings";
            var endpointUrl = "https://ch2db.documents.azure.com:443/";
            var authorizationKey = "PKtozp4b4GKoI59psRXTSw6vXm6jDTIdcyVfBxIkB52x9dDTQFW6N6Vy9Cwz8jl3cV7nse5owLCS0glrkGGXtw==";
            using (var docClient = new DocumentClient(new Uri(endpointUrl), authorizationKey))
            {
                var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

                var ratings =
                (from f in docClient.CreateDocumentQuery<ProductRating>(collectionLink)
                where f.UserId == userId
                select f).ToList();
                /*var ratings = docClient.CreateDocumentQuery<ProductRating>(
                        collectionLink,
                        "SELECT * FROM c WHERE c.UserId ='" + userId + "'",
                        new FeedOptions { EnableCrossPartitionQuery = true })*/;
                jsonToReturn = JsonConvert.SerializeObject(ratings);

            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
            };

        }

    }
}