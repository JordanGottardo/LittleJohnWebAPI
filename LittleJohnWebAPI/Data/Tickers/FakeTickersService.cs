using System;
using System.Collections.Generic;
using System.Linq;
using LittleJohnWebAPI.Utils;

namespace LittleJohnWebAPI.Data.Tickers
{
    internal class FakeTickersService : IFakeTickersService
    {
        #region Private fields

        private readonly IDictionary<string, int> _tickers;

        #endregion

        #region Initialization

        public FakeTickersService()
        {
            _tickers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"AAPL", 1},
                {"MSFT", 2},
                {"GOOG", 3},
                {"AMZN", 4},
                {"FB", 5},
                {"TSLA", 6},
                {"NVDA", 7},
                {"JPM", 8},
                {"BABA", 9},
                {"JNJ", 10},
                {"WMT", 11},
                {"PG", 12},
                {"PYPL", 13},
                {"DIS", 14},
                {"ADBE", 15},
                {"PFE", 16},
                {"V", 17},
                {"MA", 18},
                {"CRM", 19},
                {"NFLX", 20}
            };
        }

        #endregion

        public decimal GetCurrentPrice(string ticker)
        {
            if (_tickers.ContainsKey(ticker))
            {
                return GenerateTickerPrice(ticker, DateTime.Today);
            }

            throw new TickerNotFoundException($"Ticker {ticker} not found");
        }

        public IEnumerable<TickerHistoryValue> GetLast90DaysHistoryValues(string ticker)
        {
            if (_tickers.ContainsKey(ticker))
            {
                return GenerateFakeHistoryValues(ticker);
            }

            throw new TickerNotFoundException($"Ticker {ticker} not found");
        }

        #region Utility Methods

        private IEnumerable<TickerHistoryValue> GenerateFakeHistoryValues(string ticker)
        {
            var today = DateTime.Today;

            var tickerHistoryValue = new List<TickerHistoryValue>();

            for (var i = 0; i < 90; i++)
            {
                var day = today.AddDays(i * -1);

                tickerHistoryValue.Add(new TickerHistoryValue
                {
                    Day = day,
                    Price = GenerateTickerPrice(ticker, day)
                });
            }

            return tickerHistoryValue;
        }

        private decimal GenerateTickerPrice(string ticker, DateTime date)
        {
            var seed = _tickers[ticker] + GetTotalHoursBetweenDateAndUnixEpoch(date);
            return new Random(seed).NextDecimal();
        }

        private static int GetTotalHoursBetweenDateAndUnixEpoch(DateTime date)
        {
            return Convert.ToInt32(
                date
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalHours);
        }
        #endregion
    }
}