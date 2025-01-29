using System.Collections.Generic;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.WebApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeatherForecast.WebApi.Controllers {
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/values")]
    public class ValuesController : ControllerBase {
        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<Value> Get() {
            return new[] { new Value() { Id = 1, Name = "value", Description = "description" }, new Value() { Id = 2, Name = "value", Description = "description" } };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id) {
            return Ok(new Value() { Id = id, Name = "value", Description = "description" });
        }

        // POST api/<ValuesController>
        [HttpPost]
        public IActionResult Post([FromBody] Value value) {
            return Ok(value);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Value value) {
            return Ok(value);
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {
            return NoContent();
        }
    }
}
