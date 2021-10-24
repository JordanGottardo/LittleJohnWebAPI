namespace LittleJohnWebAPI.Data
{
    internal interface IFakeTickersService
    {
        decimal GetCurrentPrice(string ticker);
    }
}