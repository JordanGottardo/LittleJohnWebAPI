using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LittleJohnWebAPI.Data.Tickers;
using LittleJohnWebAPI.Utils;
using Microsoft.AspNetCore.Http;

namespace LittleJohnWebAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class BrokerController : ControllerBase
    {
        #region Private fields

        private readonly ITickersRepository _tickersRepository;
        private readonly ITokenUtils _tokenUtils;
        private readonly ILogger<BrokerController> _logger;

        #endregion

        #region Initialization

        public BrokerController(
            ITickersRepository tickersRepository, 
            ITokenUtils tokenUtils,
            ILogger<BrokerController> logger)
        {
            _tickersRepository = tickersRepository;
            _tokenUtils = tokenUtils;
            _logger = logger;
        }

        #endregion

        [HttpGet]
        [Route("tickers")]
        public ActionResult<IEnumerable<TickerInfo>> GetUserPortfolio()
        {
            _logger.LogInformation("Get user ticker portfolio invoked");
            var stopWatch = Stopwatch.StartNew();

            try
            {
                var portfolio = GetUserPortfolioOrFail();

                _logger.LogInformation($"Get user ticker portfolio finished, elapsed time {stopWatch.Elapsed}");
                
                return portfolio.Select(ToTickerInfo).ToList();
            }
            catch (ArgumentException e)
            {
                LogError(e);
                return BadRequest("Invalid parameter provided to request");
            }
            catch (MissingAuthorizationHeaderException e)
            {
                LogError(e);
                return BadRequest("Missing or wrong header value: ensure you are using Basic Auth scheme with access token as username and empty password");
            }
            catch (InvalidPortfolioException e)
            {
                LogError(e);
                return BadRequest("Invalid portfolio provided: ensure the provided portfolio has a minimum of 1 to a maximum of 10 tickers");
            }
            catch (InvalidTokenException e)
            {
                LogError(e);
                return Unauthorized("Invalid access token provided");
            }
            catch (TickerNotFoundException e)
            {
                LogError(e);
                return NotFound("One or more ticker associated to user portfolio were not found");
            }
            catch (Exception e)
            {
                LogError(e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("tickers/{ticker}/history")]
        public ActionResult<IEnumerable<TickerDateAndPrice>> GetTickerHistory(string ticker)
        {
            _logger.LogInformation($"Get ticker history invoked for ticker {ticker}");
            
            var stopWatch = Stopwatch.StartNew();

            try
            {
                GetUserPortfolioOrFail();

                var historicalValues = _tickersRepository
                    .GetLast90DaysHistoryValues(ticker)
                    .OrderByDescending(value => value.Day);
                var serializableValues = ToSerializableHistoricalValues(historicalValues);

                _logger.LogInformation($"Get ticker history finished, elapsed time {stopWatch.Elapsed}");

                return Ok(serializableValues);
            }
            catch (ArgumentException e)
            {
                LogError(e);
                return BadRequest("Invalid parameter provided to request");
            }
            catch (MissingAuthorizationHeaderException e)
            {
                LogError(e);
                return BadRequest("Missing or wrong header value: ensure you are using Basic Auth scheme with access token as username and empty password");
            }
            catch (InvalidPortfolioException e)
            {
                LogError(e);
                return BadRequest("Invalid portfolio provided: ensure the provided portfolio has a minimum of 1 to a maximum of 10 tickers");
            }
            catch (InvalidTokenException e)
            {
                LogError(e);
                return Unauthorized("Invalid access token provided");
            }
            catch (TickerNotFoundException e)
            {
                LogError(e);
                return NotFound($"Ticker {ticker} not found");
            }
        }

        #region Utility Methods

        private string GetAccessTokenHeaderValueOrFail()
        {
            if (Request == null || !Request.Headers.ContainsKey("Authorization"))
            {
                throw new MissingAuthorizationHeaderException();
            }

            string username;

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                username = credentials[0];
            }
            catch (Exception)
            {
                throw new MissingAuthorizationHeaderException();
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new MissingAuthorizationHeaderException();
            }

            return username;
        }

        private void LogError(Exception e)
        {
            _logger.LogError(e, "An error occurred while getting tickers");
        }

        private TickerInfo ToTickerInfo(string ticker)
        {
            return new()
            {
                Symbol = ticker.ToUpper(),
                Price = ToString(_tickersRepository.GetCurrentPrice(ticker))
            };
        }

        private IEnumerable<string> GetUserPortfolioOrFail()
        {
            var accessToken = GetAccessTokenHeaderValueOrFail();

            return _tokenUtils.GetUserPortfolioOrFail(accessToken);
        }

        private static string ToString(decimal value)
        {
            return value.ToString("F", new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            });
        }

        private static IEnumerable<TickerDateAndPrice> ToSerializableHistoricalValues(IEnumerable<TickerHistoryValue> tickerHistoryValues)
        {
            return tickerHistoryValues.Select(value => new TickerDateAndPrice
            {
                Price = ToString(value.Price),
                Date = value.Day.ToString("yyyy-MM-dd")

            });
        }
        #endregion
    }
}
