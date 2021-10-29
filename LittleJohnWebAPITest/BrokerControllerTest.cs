using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using LittleJohnWebAPI.Controllers;
using LittleJohnWebAPI.Data.Tickers;
using LittleJohnWebAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace LittleJohnWebAPITest
{
    [TestFixture]
    internal class BrokerControllerTest
    {
        #region Fixture

        private BrokerController _brokerController;
        private ITickersRepository _tickersRepository;
        private ITokenUtils _tokenUtils;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void Setup()
        {
            _tickersRepository = A.Fake<ITickersRepository>();
            _tokenUtils = A.Fake<ITokenUtils>();
            _brokerController = new BrokerController(_tickersRepository, _tokenUtils, A.Fake<ILogger<BrokerController>>());
        }

        #endregion

        #region GetUserPortfolio

        [Test]
        public void IfRequestIsNull_GetUserPortfolio_ShouldReturnBadRequest()
        {
            _brokerController.GetUserPortfolio().Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void IfNoAuthenticationHeaderIsProvided_GetUserPortfolio_ShouldReturnBadRequest()
        {
            var httpContext = new DefaultHttpContext();
            _brokerController = new BrokerController(_tickersRepository, _tokenUtils, A.Fake<ILogger<BrokerController>>())
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = httpContext
                    }
                };

            _brokerController.GetUserPortfolio().Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void IfTokenUtilsThrowsInvalidPortfolioException_GetUserPortfolio_ShouldReturnBadRequest()
        {
            var httpContext = AValidHttpContext();
            _brokerController = new BrokerController(_tickersRepository, _tokenUtils,  A.Fake<ILogger<BrokerController>>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            A.CallTo(() => _tokenUtils.GetUserPortfolioOrFail(A<string>._)).Throws<InvalidPortfolioException>();

            _brokerController.GetUserPortfolio().Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void IfTokenUtilsThrowsInvalidTokenException_GetUserPortfolio_ShouldReturnUnauthorized()
        {
            var httpContext = AValidHttpContext();
            _brokerController = new BrokerController(_tickersRepository, _tokenUtils, A.Fake<ILogger<BrokerController>>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            A.CallTo(() => _tokenUtils.GetUserPortfolioOrFail(A<string>._)).Throws<InvalidTokenException>();

            _brokerController.GetUserPortfolio().Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Test]
        public void IfTickersRepositoryThrowsTickerNotFound_GetUserPortfolio_ShouldReturnNotFound()
        {
            var httpContext = AValidHttpContext();
            _brokerController = new BrokerController(_tickersRepository, _tokenUtils, A.Fake<ILogger<BrokerController>>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            const string ticker = "aTicker";
            A.CallTo(() => _tokenUtils.GetUserPortfolioOrFail(A<string>._)).Returns(new List<string>{ticker});
            A.CallTo(() => _tickersRepository.GetCurrentPrice(ticker)).Throws<TickerNotFoundException>();

            _brokerController.GetUserPortfolio().Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public void HappyPath()
        {
            var httpContext = AValidHttpContext();
            _brokerController = new BrokerController(_tickersRepository, _tokenUtils, A.Fake<ILogger<BrokerController>>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            const string aTicker = "aTicker";
            const string anotherTicker = "anotherTicker";
            const decimal aTickerExpectedPrice = 10.50m;
            const decimal anotherTickerExpectedPrice = 123.22m;
            A.CallTo(() => _tokenUtils.GetUserPortfolioOrFail(A<string>._)).Returns(new List<string> { aTicker, anotherTicker });
            A.CallTo(() => _tickersRepository.GetCurrentPrice(aTicker)).Returns(aTickerExpectedPrice);
            A.CallTo(() => _tickersRepository.GetCurrentPrice(anotherTicker)).Returns(anotherTickerExpectedPrice);

            var result = _brokerController.GetUserPortfolio();

            var userPortfolio = result.Value.ToList();
            userPortfolio.Should().HaveCount(2);
            userPortfolio[0].Symbol.Should().Be("ATICKER");
            userPortfolio[0].Price.Should().Be("10.50");
            userPortfolio[1].Symbol.Should().Be("ANOTHERTICKER");
            userPortfolio[1].Price.Should().Be("123.22");
        }

        #endregion

        #region Utility Methods

        private static DefaultHttpContext AValidHttpContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Basic ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SndiM0owWm05c2FXOGlPbHNpUVVGUVRDSXNJa0ZOV2s0aUxDSk9Sa3hZSWwxOS5aR2llR3hCODFGUUZTUnlYMk1nWnFYekRYa0h0d2VqdmNIdS1fdTBoN0lr";
            return httpContext;
        }

        #endregion
    }
}