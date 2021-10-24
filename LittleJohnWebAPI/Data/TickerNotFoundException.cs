using System;

namespace LittleJohnWebAPI.Data
{
    internal class TickerNotFoundException : Exception
    {

        #region Initialization

        public TickerNotFoundException()
        {

        }

        public TickerNotFoundException(string message) : base(message)
        {

        }

        #endregion
    }
}