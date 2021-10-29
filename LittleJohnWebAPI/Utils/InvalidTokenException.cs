using System;

namespace LittleJohnWebAPI.Utils
{
    internal class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
            
        }

        public InvalidTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}