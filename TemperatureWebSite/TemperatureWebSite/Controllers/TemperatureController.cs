using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TemperatureWebSite.Models;

namespace TemperatureWebSite.Controllers
{
    public class TemperatureController : AServiceController
    {
        // dependency injection
        public TemperatureController(HttpClient client) : base(client)
        {
        }

        // GET: Temperature
        public async Task<ActionResult> Index()
        {
            // send "GET api/Temperature" to service, get headers of response
            HttpRequestMessage request = CreateRequestToService(HttpMethod.Get, "api/temperature");
            HttpResponseMessage response = await Client.SendAsync(request);

            // (if status code is not 200-299 (for success))
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account");
                }
                return RedirectToAction("Error", "Home");
            }

            // get the whole response body (second await)
            var responseBody = await response.Content.ReadAsStringAsync();

            // this is a string, so it must be deserialized into a C# object.
            // we could use DataContractSerializer, .NET built-in, but it's more awkward
            // than the third-party Json.NET aka Newtonsoft JSON.
            List<TemperatureRecord> temperatures = JsonConvert.DeserializeObject<List<TemperatureRecord>>(responseBody);

            return View(temperatures);
        }

        // GET: Temperature/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Temperature/Create
        public async Task<ActionResult> Create()
        {
            HttpRequestMessage request = CreateRequestToService(HttpMethod.Get, "api/account/loggedinuser");
            HttpResponseMessage response = await Client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account");
                }
                return View("Error");
            }

            // provide default value to Create form
            return View(new TemperatureRecord { Time = DateTime.Now });
        }

        //public static HttpContent ToContent<T>(T obj)
        //{
        //    // instead of this we can use PostAsJsonAsync, easier
        //    string json = JsonConvert.SerializeObject(obj);
        //    // declare the encoding (unicode)
        //    // and the "media type" (JSON) of the thing to send in the request body
        //    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        //    return content;
        //}

        // POST: Temperature/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TemperatureRecord record)
        {
            try
            {
                // set unit to 1 (celsius)
                record.Unit = 1;
                // use POST method, not GET, based on the route the service has defined
                HttpRequestMessage request = CreateRequestToService(HttpMethod.Post, "api/temperature", record);
                HttpResponseMessage response = await Client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    return View(record);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(record);
            }
        }

        // GET: Temperature/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var json = await Client.GetStringAsync($"https://localhost:44365/api/temperature/{id}");
            return View(JsonConvert.DeserializeObject<TemperatureRecord>(json));
        }

        // POST: Temperature/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, TemperatureRecord record)
        {
            try
            {
                record.Unit = 1;
                var url = $"https://localhost:44365/api/temperature/{id}";
                var response = await Client.PutAsJsonAsync(url, record);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View(record);
            }
            catch
            {
                return View(record);
            }
        }

        // GET: Temperature/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            // extract response body in just one await
            var json = await Client.GetStringAsync($"https://localhost:44365/api/temperature/{id}");
            return View(JsonConvert.DeserializeObject<TemperatureRecord>(json));
        }

        // POST: Temperature/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var response = await Client.DeleteAsync($"https://localhost:44365/api/temperature/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Delete), new { id });
            }
            catch
            {
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}