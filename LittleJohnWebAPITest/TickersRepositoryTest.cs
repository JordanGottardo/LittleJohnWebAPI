using System;
using FakeItEasy;
using FluentAssertions;
using LittleJohnWebAPI.Data.Tickers;
using NUnit.Framework;

namespace LittleJohnWebAPITest
{
    [TestFixture]
    internal class TickersRepositoryTest
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
        public void WhenInvokedWithNullOrEmptyOrWhitespace_GetCurrentPrice_ShouldThrowArgumentException(string ticker)
        {
            _tickersRepository.Invoking(repo => repo.GetCurrentPrice(ticker))
                .Should()
                .Throw<ArgumentException>();
        }

        [Test]
        public void WhenInvokedWithUnknownTicker_GetCurrentPrice_ShouldThrowTickerNotFoundException()
        {
            const string ticker = "aTicker";
            var exception = new TickerNotFoundException();
            A.CallTo(() => _fakeTickersService.GetCurrentPrice(ticker)).Throws(exception);

            _tickersRepository.Invoking(repo => repo.GetCurrentPrice(ticker))
                .Should()
                .Throw<TickerNotFoundException>()
                .Which
                .Should()
                .Be(exception);
        }

        [Test]
        public void TickersRepository_ShouldThrowTickerNotFoundException()
        {
            const string ticker = "aTicker";
            const decimal expectedPrice = 25.50m;
            A.CallTo(() => _fakeTickersService.GetCurrentPrice(ticker)).Returns(expectedPrice);

            var price = _tickersRepository.GetCurrentPrice(ticker);

            price.Should().Be(expectedPrice);

        }
    }
}