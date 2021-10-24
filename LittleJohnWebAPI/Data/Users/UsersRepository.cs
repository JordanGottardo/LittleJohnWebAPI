using System;

namespace LittleJohnWebAPI.Data.Users
{
    internal class UsersRepository : IUsersRepository
    {
        #region Private fields

        private readonly IFakeUsersService _fakeUserService;

        #endregion

        #region Initialization

        public UsersRepository(IFakeUsersService fakeUserService)
        {
            _fakeUserService = fakeUserService;
        }

        #endregion

        public User GetUserByUsername(string username)
        {
            ValidateUsernameOrThrow(username);

            return _fakeUserService.GetUserByUsername(username);
        }

        #region Utility methods

        private static void ValidateUsernameOrThrow(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Invalid argument", nameof(username));
            }
        }

        #endregion
    }
}