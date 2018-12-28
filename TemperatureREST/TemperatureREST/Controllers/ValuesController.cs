using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TemperatureREST.Models;

namespace TemperatureREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            // whatever we return as an ActionResult will be automatically
            // serialized as JSON.
            // when making REST services, we do not use views at all, or any HTML,
            // or any view rendering step - we just respond with the data in JSON format.
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Dictionary<int, Temperature>> Get(int id)
        {
            return new Dictionary<int, Temperature>
            {
                [1] = new Temperature(),
                [4] = new Temperature()
            };
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
