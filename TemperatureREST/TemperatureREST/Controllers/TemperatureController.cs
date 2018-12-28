using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TemperatureREST.Models;

namespace TemperatureREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // only logged-in users can access any of these action methods.
    public class TemperatureController : ControllerBase
    {
        // really we would use a DB, but for dmeo purposes, a static list.
        public static List<Temperature> Data = new List<Temperature>
        {
            new Temperature
            {
                Id = 1,
                Time = DateTime.Now,
                Value = 36,
                Unit = TemperatureUnit.Celsius
            }
        };

        // GET: api/Temperature
        [HttpGet]
        // the return type can be just your type
        // or, ActionResult<YourType>, both will work, but the latter also allows you to
        // return error messages
        [AllowAnonymous] // override [Authorize], allow not logged-in users to access this one method
        public ActionResult<IEnumerable<Temperature>> Get()
        {
            try
            {
                return Data; // wrapped in "200 OK" response
            }
            catch (Exception ex)
            {
                // internal server error
                // (but sending server exceptions blindly to the client is not good for security)
                return StatusCode(500, ex);
            }
        }

        // GET: api/Temperature/5
        // give this route a name so that it can be targeted by name in CreatedAtRoute for example
        [HttpGet("{id}", Name = "Get")]
        // override content negotation and only provide this media type from this action method
        // (not very RESTful)
        [Produces("application/json")]
        public ActionResult<Temperature> Get(int id)
        {
            Temperature result;
            try
            {
                result = Data.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                // internal server error
                return StatusCode(500, ex);
            }
            if (result == null)
            {
                return NotFound();
            }
            // (for returning custom response body content)
            //var content = new StringContent("content", Encoding.UTF8, "(my media type)");
            // returning ObjectResult does content negotiation with the client based on
            // what formatters we have configured in Startup.cs and on the client's "Accept" header.
            // [FormatFilter] is possible on an action method, this allows a "?format=<file-extension>"
            //   to specify the format instead of Accept header, but this is not super RESTful
            return result;
        }

        // POST: api/Temperature
        // for inserting a new resource
        [HttpPost]
        public ActionResult Post([FromBody, Bind("Time,Value,Unit")] Temperature value)
        {
            // with [ApiController] attribute on controller, this validation is done automatically.
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            // in our action methods, we have access to Request property and Response property
            // on ControllerBase class.
            // so you can access any info about the recieved request and do conditions
            Response.Headers.Add("CustomHeader", new StringValues("mycustomvalue"));

            // assign an ID to the record that's not in use
            int newId;
            try
            {
                newId = (Data.Count == 0) ? 1 : (Data.Max(x => x.Id) + 1);
            }
            catch (Exception ex)
            {
                // internal server error
                return StatusCode(500, ex);
            }
            value.Id = newId;
            try
            {
                Data.Add(value);
            }
            catch (Exception ex)
            {
                // internal server error
                return StatusCode(500, ex);
            }
            // return proper 201 Created response, based on correct route for get-by-ID,
            // and return the representation of the object.
            return CreatedAtRoute("Get", new { id = newId }, value);
            // that is based on the route name defined on the Get(int id) action method.
            // can also use CreatedAtAction
        }

        // PUT: api/Temperature/5
        // replace an existing resource
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Temperature value)
        {
            Temperature existing;
            try
            {
                existing = Data.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                // internal server error
                return StatusCode(500, ex);
            }
            if (existing == null)
            {
                return NotFound(); // if resource doesn't exist, i'll return an error
            }
            if (id != value.Id)
            {
                return BadRequest("cannot change ID");
            }
            try
            {
                Data.Remove(existing);
                Data.Add(value);
            }
            catch (Exception ex)
            {
                // internal server error
                return StatusCode(500, ex);
            }
            // return proper 204 No Content response
            return NoContent(); // success = Ok()
        }

        // DELETE: api/Temperature/5
        // DELETE is for deleting resources
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")] // comma-separated list of allowed roles to this action method.
        public ActionResult Delete(int id)
        {
            Temperature existing;
            try
            {
                existing = Data.FirstOrDefault(x => x.Id == id);
                if (existing == null)
                {
                    return NotFound(); // if resource doesn't exist, i'll return an error
                }
                Data.Remove(existing);
            }
            catch (Exception ex)
            {
                // internal server error
                return StatusCode(500, ex);
            }
            // return proper 204 No Content response
            return NoContent(); // success = Ok()
        }
    }
}
