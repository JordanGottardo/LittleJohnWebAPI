using System;
using System.Collections;
using System.Collections.Generic;

namespace LittleJohnWebAPI.Data.Users
{
    internal class FakeUsersService : IFakeUsersService
    {
        #region Private fields

        private readonly IDictionary<string, User> _userDictionary;

        #endregion

        #region Initialization

        public FakeUsersService()
        {
            _userDictionary = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase)
            {
                { "User1", new User
                {
                    Username = "User1",
                    Portfolio = new List<string>
                    {
                        "AAPL", "AMZN", "TSLA", "NVDA", "BABA", "V"
                    }
                } },
                { "User2", new User
                {
                    Username = "User2",
                    Portfolio = new List<string>
                    {
                        "V", "CRM", "NFLX"
                    }
                } },
            };
        }

        #endregion

        public User GetUserByUsername(string username)
        {
            if (_userDictionary.ContainsKey(username))
            {
                return _userDictionary[username];
            }

            throw new UserNotFoundException($"User {username} not found");
        }
    }
}