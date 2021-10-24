using System;
using FakeItEasy;
using FluentAssertions;
using LittleJohnWebAPI.Data;
using NUnit.Framework;

namespace LittleJohnWebAPITest
{
    public class TickersRepositoryTest
    {
        #region Fixture

        private IFakeTickersService _fakeTickersService;
        private TickersRepository _tickersRepository;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void Setup()
        {
            _fakeTickersService = A.Fake<IFakeTickersService>();
            _tickersRepository = new TickersRepository(_fakeTickersService);
        }

        #endregion

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void WhenInvokedWithNullOrEmptyOrWhitespace_TickersRepository_ShouldThrowArgumentException(string ticker)
        {
            _tickersRepository.Invoking(repo => repo.GetCurrentPrice(ticker))
                .Should()
                .Throw<ArgumentException>();
        }

        [Test]
        public void WhenInvokedWithUnknownTicker_TickersRepository_ShouldThrowTickerNotFoundException()
        {
            var ticker = "aTicker";
            var exception = new TickerNotFoundException();
            A.CallTo(() => _fakeTickersService.GetCurrentPrice(ticker)).Throws(exception);

            _tickersRepository.Invoking(repo => repo.GetCurrentPrice(ticker))
                .Should()
                .Throw<TickerNotFoundException>()
                .Which
                .Should()
                .Be(exception);
        }
    }
}