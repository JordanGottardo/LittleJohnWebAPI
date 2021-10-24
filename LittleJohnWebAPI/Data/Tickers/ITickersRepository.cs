namespace LittleJohnWebAPI.Data.Tickers
{
    public interface ITickersRepository
    {
        decimal GetCurrentPrice(string ticker);
    }
}