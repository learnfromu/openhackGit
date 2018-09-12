
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
    public static class CreateRating
    {
       

        [FunctionName("CreateRating")]
        public static async Task<HttpResponseMessage> Create([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var rating = req.Content.ReadAsAsync<ProductRating>().Result;
            if(rating.Rating>6 || rating.Rating<0)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "rating must be within [0-5], your value is: " + rating.Rating);
            }
            Document resultDoc = null;
            using (var client = new HttpClient())
            {
                string url = "https://serverlessohproduct.trafficmanager.net/api/GetProduct?productId="+rating.ProductId;
                //var content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                var response = await client.GetAsync(url).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadAsAsync<Product>();
                    if(result == null)
                    {
                        return req.CreateResponse(HttpStatusCode.BadRequest, "Product is wrong: " + rating.ProductId);
                    }
                }else
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Product is wrong: " + rating.ProductId);
                }
                
                url = "https://serverlessohuser.trafficmanager.net/api/GetUser?userId=" + rating.UserId;
                //var content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                response = await client.GetAsync(url).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadAsAsync<User>();
                    if (result == null)
                    {
                        return req.CreateResponse(HttpStatusCode.BadRequest, "User is wrong:" + rating.UserId);
                    }
                }else
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, "User is wrong:" + rating.UserId);
                }
                rating.PopulateIdandTime();
                var databaseId = "Openhackdb";
                var collectionId = "Ratings";
                var endpointUrl = "https://ch2db.documents.azure.com:443/";
                var authorizationKey = "PKtozp4b4GKoI59psRXTSw6vXm6jDTIdcyVfBxIkB52x9dDTQFW6N6Vy9Cwz8jl3cV7nse5owLCS0glrkGGXtw==";
                using (var docClient = new DocumentClient(new Uri(endpointUrl), authorizationKey))
                {
                    var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
                    //var product = new { ProductId = "aa", UserId = "bbb", Ratings = 5 };
                    resultDoc = docClient.CreateDocumentAsync(collectionLink, rating).Result;
                    Console.WriteLine(resultDoc);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(resultDoc), Encoding.UTF8, "application/json")
            };
        }
    }
}