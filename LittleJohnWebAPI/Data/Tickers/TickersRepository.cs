using System;

namespace LittleJohnWebAPI.Data.Tickers
{
    internal class TickersRepository : ITickersRepository
    {
        #region Private Fields

        private readonly IFakeTickersService _fakeTickersService;

        #endregion

        #region Initialization

        public TickersRepository(IFakeTickersService fakeTickersService)
        {
            _fakeTickersService = fakeTickersService;
        }

        #endregion

        public decimal GetCurrentPrice(string ticker)
        {
            ValidateTickerOrThrow(ticker);

            return _fakeTickersService.GetCurrentPrice(ticker);
        }

        #region Utility Methods

        private static void ValidateTickerOrThrow(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new ArgumentException("Invalid argument", nameof(ticker));
            }
        }

        #endregion
    }
}