namespace LittleJohnWebAPI.Data
{
    public interface ITickersRepository
    {
        decimal GetCurrentPrice(string ticker);
    }
}