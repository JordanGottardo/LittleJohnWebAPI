using System;
using FakeItEasy;
using FluentAssertions;
using LittleJohnWebAPI.Data.Users;
using NUnit.Framework;
using System.Collections.Generic;

namespace LittleJohnWebAPITest
{
    [TestFixture]
    internal class UsersRepositoryTest
    {
        #region Fixture

        private UsersRepository _usersRepository;
        private IFakeUsersService _fakeUserService;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void Setup()
        {
            _fakeUserService = A.Fake<IFakeUsersService>();
            _usersRepository = new UsersRepository(_fakeUserService);
        }

        #endregion

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void WhenInvokedWithNullOrEmptyOrWhitespace_GetUserByUsername_ShouldThrowArgumentException(string username)
        {
            _usersRepository.Invoking(repo => repo.GetUserByUsername(username))
                .Should()
                .Throw<ArgumentException>();
        }

        [Test]
        public void IfUsersIsUnknown_GetUserByUsername_ShouldThrowUserNotFoundException()
        {
            var aUsername = "aUsername";
            var exception = new UserNotFoundException();
            A.CallTo(() => _fakeUserService.GetUserByUsername(aUsername)).Throws(exception);

            _usersRepository.Invoking(repo => repo.GetUserByUsername(aUsername))
                .Should()
                .Throw<UserNotFoundException>()
                .Which
                .Should()
                .Be(exception);
        }

        [Test]
        public void GetUserByUsername_ShouldReturnTheRequestedUser()
        {
            var aUsername = "aUsername";
            var expectedUser = AUser();
            A.CallTo(() => _fakeUserService.GetUserByUsername(aUsername)).Returns(expectedUser);

            var user = _usersRepository.GetUserByUsername(aUsername);

            user.Should().Be(expectedUser);
        }

        #region Utility Methods

        private static User AUser()
        {
            return new()
            {
                Username = "User1",
                Portfolio = new List<string>
                {
                    "AAPL", "AMZN", "TSLA", "NVDA", "BABA", "V"
                }
            };
        }

        #endregion
    }
}