using System;
using System.Collections.Generic;
using LittleJohnWebAPI.Utils;

namespace LittleJohnWebAPI.Data
{
    internal class FakeTickersService : IFakeTickersService
    {
        #region Private fields

        private readonly IDictionary<string, decimal> _currentTickerPriceDictionary;

        #endregion

        #region Initialization

        public FakeTickersService()
        {
            var random = new Random();

            _currentTickerPriceDictionary = new Dictionary<string, decimal>
            {
                {"AAPL", random.NextDecimal()},
                {"MSFT", random.NextDecimal()},
                {"GOOG", random.NextDecimal()},
                {"AMZN", random.NextDecimal()},
                {"FB", random.NextDecimal()},
                {"TSLA", random.NextDecimal()},
                {"NVDA", random.NextDecimal()},
                {"JPM", random.NextDecimal()},
                {"BABA", random.NextDecimal()},
                {"JNJ", random.NextDecimal()},
                {"WMT", random.NextDecimal()},
                {"PG", random.NextDecimal()},
                {"PYPL", random.NextDecimal()},
                {"DIS", random.NextDecimal()},
                {"ADBE", random.NextDecimal()},
                {"PFE", random.NextDecimal()},
                {"V", random.NextDecimal()},
                {"MA", random.NextDecimal()},
                {"CRM", random.NextDecimal()},
                {"NFLX", random.NextDecimal()}
            };
        }

        #endregion

        public decimal GetCurrentPrice(string ticker)
        {
            if (_currentTickerPriceDictionary.ContainsKey(ticker))
            {
                return _currentTickerPriceDictionary[ticker];
            }

            throw new TickerNotFoundException($"Ticker {ticker} not found");
        }
    }
}