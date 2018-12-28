using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TemperatureWebSite.Models;

namespace TemperatureWebSite.Controllers
{
    public class AccountController : AServiceController
    {
        public AccountController(HttpClient client) : base(client)
        {
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Account account)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                HttpRequestMessage request = CreateRequestToService(HttpMethod.Post, "api/Account/Login", account);
                HttpResponseMessage response = await Client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        ModelState.AddModelError("Password", "Incorrect username or password");
                    }
                    return View();
                }

                var success = PassCookiesToClient(response);
                if (!success)
                {
                    return View("Error");
                }

                // successful login
                return RedirectToAction("Index", "Temperature");
            }
            catch (Exception)
            {
                return View();
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }

        // takes the auth set-cookie from an api response, and adds it to
        // the response to the browser we are currently constructing
        private bool PassCookiesToClient(HttpResponseMessage apiResponse)
        {
            if (apiResponse.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                // here the "value" contains both the name and the value of the cookie
                var authValue = values.FirstOrDefault(x => x.StartsWith(s_CookieName));
                if (authValue != null)
                {
                    Response.Headers.Add("Set-Cookie", authValue);
                    return true;
                }
            }
            return false;
        }
    }
}