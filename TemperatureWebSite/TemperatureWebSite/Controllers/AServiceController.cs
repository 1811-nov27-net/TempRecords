using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureWebSite.Controllers
{
    public class AServiceController : Controller
    {
        protected static readonly string s_CookieName = "TempApiAuth";
        private readonly Uri serviceUri;

        public HttpClient Client { get; set; }

        // dependency injection
        public AServiceController(HttpClient client, IConfiguration config)
        {
            Client = client;
            serviceUri = new Uri(config["ServiceUrl"]);
        }

        public HttpRequestMessage CreateRequestToService(HttpMethod method, string uri, object body = null)
        {
            var apiRequest = new HttpRequestMessage(method, new Uri(serviceUri, uri));

            if (body != null)
            {
                var jsonString = JsonConvert.SerializeObject(body);
                apiRequest.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            }

            // get the value of the app auth cookie from the client's request
            // (null if not present/logged-in)
            var cookieValue = Request.Cookies[s_CookieName];

            if (cookieValue != null)
            {
                // CookieHeaderValue handles the string formatting
                apiRequest.Headers.Add("Cookie", new CookieHeaderValue(s_CookieName, cookieValue).ToString());
            }

            return apiRequest;
            // when we construct the request ahead of time (instead of using GetAsync etc)
            // including the method in it, then, we use SendAsync to send that.
        }
    }
}
