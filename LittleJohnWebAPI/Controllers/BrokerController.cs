using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LittleJohnWebAPI.Data;
using LittleJohnWebAPI.Data.Tickers;

namespace LittleJohnWebAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class BrokerController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        #region Private fields

        private readonly ITickersRepository _tickersRepository;
        private readonly ILogger<BrokerController> _logger;

        #endregion

        public BrokerController(ITickersRepository tickersRepository, ILogger<BrokerController> logger)
        {
            _tickersRepository = tickersRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("tickers")]
        public IEnumerable<TickerInfo> Get()
        {
            _logger.LogInformation("tickers API logger");

            return new List<TickerInfo>
            {
                new()
                {
                    Symbol = "AAPL",
                    Price = _tickersRepository.GetCurrentPrice("AAPL")
                },
                new()
                {
                    Symbol = "AMZN",
                    Price = _tickersRepository.GetCurrentPrice("AMZN")
                }
            };
        }

        //[HttpGet]
        //[Route("tickers")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    _logger.LogInformation("tickers API logger");

        //    var rng = new Random();

        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
    }
}
