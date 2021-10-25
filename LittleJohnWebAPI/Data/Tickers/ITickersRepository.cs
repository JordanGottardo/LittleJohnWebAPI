using System.Collections.Generic;

namespace LittleJohnWebAPI.Data.Tickers
{
    public interface ITickersRepository
    {
        decimal GetCurrentPrice(string ticker);
        IEnumerable<TickerHistoryValue> GetLast90DaysHistoryValues(string ticker);
    }
}