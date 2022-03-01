using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITesting
{
    // Class that uses helper functions to return the requested reponses to the tests
    public class CoinGecko
    {
        // Function to return the reponse of a healthcheck endpoint in CoinGecko API
        public IRestResponse GetHealth()
        {
            var test = new APIHelper();
            var url = test.GenerateUrl("ping");
            var request = test.CreateGetRequest();
            var response = test.GetResponse(url, request);
            return response;
        }

        // Function to return the reponse of a exchange rate endpoint in CoinGecko API
        public IRestResponse GetCurrency()
        {
            var test = new APIHelper();
            var url = test.GenerateUrl("exchange_rates");
            var request = test.CreateGetRequest();
            var response = test.GetResponse(url, request);
            return response;
        }

        // Function to return the reponse of an exchange endpoint in CoinGecko API
        // This can be used for any exchange 
        public IRestResponse GetExchange(string exchange)
        {
            var test = new APIHelper();
            var url = test.GenerateUrl(exchange);
            var request = test.CreateGetRequest();
            var response = test.GetResponse(url, request);
            return response;
        }
    }
}
