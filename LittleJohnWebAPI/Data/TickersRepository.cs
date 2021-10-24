using System;

namespace LittleJohnWebAPI.Data
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
            ValidateTicker(ticker);

            return _fakeTickersService.GetCurrentPrice(ticker);
        }

        #region Utility Methods

        private static void ValidateTicker(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new ArgumentException("Argument invalid", nameof(ticker));
            }
        }

        #endregion
    }
}