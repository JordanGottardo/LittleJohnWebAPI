using System.Collections.Generic;

namespace LittleJohnWebAPI.Data.Tickers
{
    internal interface IFakeTickersService
    {
        decimal GetCurrentPrice(string ticker);
        IEnumerable<TickerHistoryValue> GetLast90DaysHistoryValues(string ticker);
    }
}