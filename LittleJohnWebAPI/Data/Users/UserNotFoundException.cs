using System;

namespace LittleJohnWebAPI.Data.Users
{
    internal class UserNotFoundException : Exception
    {
        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string message): base(message)
        {
        }
    }
}