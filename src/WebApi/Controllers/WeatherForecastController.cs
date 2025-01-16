using System;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeatherForecast.WebApi.Models;

namespace WeatherForecast.WebApi.Controllers {
    /// <summary>
    /// Weather Forecast Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/forecasts")]
    public class WeatherForecastController : ControllerBase {
        private static readonly string[] Summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

        private readonly ILogger<WeatherForecastController> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger) {
            this.logger = logger;
        }

        /// <summary>
        /// Gets a list of forecasts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Forecast[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetForecasts() {
            var forecasts = Enumerable.Range(1, 5).Select(index => new Forecast {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            return Ok(forecasts);
        }

#pragma warning disable S1133 // Suppress obsolete warning
        /// <summary>
        /// Gets a forecast by int id
        /// </summary>
        /// <param name="intId">the id of the customer to get</param>
        [HttpGet("{intId:int}")]
        [ProducesResponseType(typeof(Forecast), StatusCodes.Status200OK)]
        [Obsolete("Use endpoint that accepts a guidId instead of int")]
        public Task<IActionResult> GetForecastsByIntId(int intId) {
            var dto = new Forecast {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };
            return Task.FromResult<IActionResult>(Ok(dto));
        }
#pragma warning restore S1133 // Suppress obsolete warning

        /// <summary>
        /// Gets a forecast by guid guidId
        /// </summary>
        /// <param name="guidId">the guidId of the customer to get</param>
        [HttpGet("{guidId:guid}")]
        [ActionName(nameof(GetCustomerAsync))]
        [ProducesResponseType(typeof(Forecast), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomerAsync(Guid guidId) {
            var dto = new Forecast {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };
            return Ok(dto);
        }
    }
}
