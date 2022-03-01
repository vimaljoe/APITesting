using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace APITests
{
    // Class for actions related to the content of the Rest response
    public class ContentHelper
    {
        //Function to deserialize the Rest reponse
        public static T GetContent<T>(string response)
        {
            return JsonConvert.DeserializeObject<T>(response);
        }
    }
}
