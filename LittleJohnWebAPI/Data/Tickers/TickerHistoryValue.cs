using System;

namespace LittleJohnWebAPI.Data.Tickers
{
    public class TickerHistoryValue
    {
        public DateTime Day { get; init; }
        public decimal Price { get; init; }
    }
}