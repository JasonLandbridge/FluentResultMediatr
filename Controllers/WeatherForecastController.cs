using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using FluentResultsMediatr.Model;
using FluentResultsMediatr.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FluentResultsMediatr.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {


        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMediator _mediator;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator  mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        // GET api/<WeatherForecastController>/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<WeatherForecast>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result))]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetWeatherForecastByIdQuery(id));
                if (result.IsFailed)
                {
                    return NotFound(result.Reasons);
                }

                return Ok(result);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }    
        
        // GET api/<WeatherForecastController>/5
        [HttpGet("no-typed/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<WeatherForecast>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result))]
        public async Task<IActionResult> GetWithNoTypedResult(int id)
        {
            var result = await _mediator.Send(new GetWeatherForecastByIdNoTypedResultQuery(id));
            if (result.IsFailed)
            {
                return NotFound(result.Reasons);
            }

            return Ok(result);
        }
    }
}
