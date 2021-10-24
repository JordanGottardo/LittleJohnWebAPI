namespace LittleJohnWebAPI.Data.Tickers
{
    internal interface IFakeTickersService
    {
        decimal GetCurrentPrice(string ticker);
    }
}