using System;
using System.Collections.Generic;
using System.Linq;
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

        #region GetCurrentPrice

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
        public void GetCurrentPrice_ShouldThrowTickerNotFoundException()
        {
            const string ticker = "aTicker";
            const decimal expectedPrice = 25.50m;
            A.CallTo(() => _fakeTickersService.GetCurrentPrice(ticker)).Returns(expectedPrice);

            var price = _tickersRepository.GetCurrentPrice(ticker);

            price.Should().Be(expectedPrice);

        }

        #endregion

        #region GetLast90DaysHistoryValues

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void WhenInvokedWithNullOrEmptyOrWhitespace_GetLast90DaysHistoryValues_ShouldThrowArgumentException(string ticker)
        {
            _tickersRepository.Invoking(repo => repo.GetLast90DaysHistoryValues(ticker))
                .Should()
                .Throw<ArgumentException>();
        }

        [Test]
        public void WhenInvokedWithUnknownTicker_GetLast90DaysHistoryValues_ShouldThrowTickerNotFoundException()
        {
            const string ticker = "aTicker";
            var exception = new TickerNotFoundException();
            A.CallTo(() => _fakeTickersService.GetLast90DaysHistoryValues(ticker)).Throws(exception);

            _tickersRepository.Invoking(repo => repo.GetLast90DaysHistoryValues(ticker))
                .Should()
                .Throw<TickerNotFoundException>()
                .Which
                .Should()
                .Be(exception);
        }

        [Test]
        public void GetLast90DaysHistoryValues_ShouldThrowTickerNotFoundException()
        {
            const string ticker = "aTicker";
            var expectedHistoryValues = HistoryValues().ToList();
            A.CallTo(() => _fakeTickersService.GetLast90DaysHistoryValues(ticker)).Returns(expectedHistoryValues);

            var historyValues = _tickersRepository.GetLast90DaysHistoryValues(ticker);

            historyValues.SequenceEqual(expectedHistoryValues).Should().BeTrue();
        }

        #endregion

        #region Utility Methods

        private static IEnumerable<TickerHistoryValue> HistoryValues()
        {
            return new List<TickerHistoryValue>
            {
                new()
                {
                    Price = 30.50m,
                    Day = DateTime.Now
                },
                new()
                {
                    Price = 12.30m,
                    Day = DateTime.Now.AddDays(-30)
                }
            };
        }

        #endregion
    }
}