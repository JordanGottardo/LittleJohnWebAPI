using System;
using System.Collections.Generic;
using System.Linq;
using LittleJohnWebAPI.Utils;

namespace LittleJohnWebAPI.Data.Tickers
{
    internal class FakeTickersService : IFakeTickersService
    {
        #region Private fields

        private readonly IDictionary<string, decimal> _currentTickerPriceDictionary;
        private readonly IDictionary<string, IEnumerable<TickerHistoryValue>> _last90DaysTickerHistoryDictionary;
        private readonly Random _random;

        #endregion

        #region Initialization

        public FakeTickersService()
        {
            _random = new Random(1234);

            _currentTickerPriceDictionary = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                {"AAPL", _random.NextDecimal()},
                {"MSFT", _random.NextDecimal()},
                {"GOOG", _random.NextDecimal()},
                {"AMZN", _random.NextDecimal()},
                {"FB", _random.NextDecimal()},
                {"TSLA", _random.NextDecimal()},
                {"NVDA", _random.NextDecimal()},
                {"JPM", _random.NextDecimal()},
                {"BABA", _random.NextDecimal()},
                {"JNJ", _random.NextDecimal()},
                {"WMT", _random.NextDecimal()},
                {"PG", _random.NextDecimal()},
                {"PYPL", _random.NextDecimal()},
                {"DIS", _random.NextDecimal()},
                {"ADBE", _random.NextDecimal()},
                {"PFE", _random.NextDecimal()},
                {"V", _random.NextDecimal()},
                {"MA", _random.NextDecimal()},
                {"CRM", _random.NextDecimal()},
                {"NFLX", _random.NextDecimal()}
            };

            _last90DaysTickerHistoryDictionary = new Dictionary<string, IEnumerable<TickerHistoryValue>>(StringComparer.OrdinalIgnoreCase);

            foreach (var ticker in _currentTickerPriceDictionary.Keys)
            {
                _last90DaysTickerHistoryDictionary[ticker] = GenerateFakeHistoryValues(_currentTickerPriceDictionary[ticker]);

            }
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

        public IEnumerable<TickerHistoryValue> GetLast90DaysHistoryValues(string ticker)
        {
            if (_last90DaysTickerHistoryDictionary.ContainsKey(ticker))
            {
                return _last90DaysTickerHistoryDictionary[ticker];
            }

            throw new TickerNotFoundException($"Ticker {ticker} not found");
        }

        #region Utility Methods

        private IEnumerable<TickerHistoryValue> GenerateFakeHistoryValues(decimal todayPrice)
        {
            var today = DateTime.Now;

            var tickerHistoryValue = new List<TickerHistoryValue>
            {
                new()
                {
                    Day = today,
                    Price = todayPrice
                }
            };

            for (var i = 1; i < 90; i++)
            {
                tickerHistoryValue.Add(new TickerHistoryValue
                {
                    Day = today.AddDays(i * -1),
                    Price = _random.NextDecimal()
                });
            }

            return tickerHistoryValue;
        }

        #endregion
    }
}