using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TemperatureWebSite.Controllers;

namespace TemperatureWebSite.Filters
{
    public class GetLoggedInUserFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // an as cast will be null if the cast fails
            var controller = context.Controller as AServiceController;
            if (controller != null)
            {
                HttpRequestMessage request = controller.CreateRequestToService(HttpMethod.Get, "api/account/loggedinuser");
                HttpResponseMessage response = await controller.Client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    controller.ViewBag.LoggedInUser = "";
                }
                controller.ViewBag.LoggedInUser = await response.Content.ReadAsStringAsync();
            }
            var resultContext = await next();
        }
    }
}
