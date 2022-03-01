using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace APITesting
{
    public class APIHelper
    {
        public RestClient restClient;
        public RestRequest restRequest;
        public string baseUrl = "https://api.coingecko.com/";
        public string version = "api/v3/";

        // Function to generate URL, returning the  Rest Client
        public RestClient GenerateUrl(string endpoint)
        {
            var url = Path.Combine(baseUrl, version, endpoint);
            var restClient = new RestClient(url);
            return restClient;
        }

        // Function to create a PUT request with headers and 
        // parameters like body, returning the Rest request
        public RestRequest CreatePutRequest(string body)
        {
            var restRequest = new RestRequest(Method.PUT);
            restRequest.AddHeader("Accept", "application/json");
            restRequest.AddParameter("application/json", body, ParameterType.RequestBody);
            return restRequest;
        }

        // Function to create a GET request with headers, returning the Rest request
        public RestRequest CreateGetRequest()
        {
            var restRequest = new RestRequest(Method.GET);
            restRequest.AddHeader("Accept", "application/json");
            return restRequest;
        }

        // Function to execute the Rest request and Returning the Rest client
        public IRestResponse GetResponse(RestClient restClient, RestRequest restRequest)
        {
            return restClient.Execute(restRequest);
        }

        // Function to create a stub for BTC PUT request
        // This creates a specific request with paramters 
        // and creates a specific response with status, headers and body
        public WireMockServer CreateBTCPutStub(WireMockServer server)
        {            
            server.Given(
                Request.Create().WithPath("/btc/10000").UsingPut()
            )
            .RespondWith(
                Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "text/plain")
                .WithBody("BTC price set to 10000!")
            );
            return server;
        }
    }
}
