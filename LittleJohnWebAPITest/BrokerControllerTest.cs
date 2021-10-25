using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using LittleJohnWebAPI.Controllers;
using LittleJohnWebAPI.Data.Tickers;
using LittleJohnWebAPI.Data.Users;
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

        private IUsersRepository _usersRepository;
        private BrokerController _brokerController;
        private ITickersRepository _tickersRepository;
        private ITokenAuthorizer _tokenAuthorizer;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void Setup()
        {
            _tickersRepository = A.Fake<ITickersRepository>();
            _usersRepository = A.Fake<IUsersRepository>();
            _tokenAuthorizer = A.Fake<ITokenAuthorizer>();
            _brokerController = new BrokerController(_tickersRepository, _usersRepository, _tokenAuthorizer, A.Fake<ILogger<BrokerController>>());
        }

        #endregion

        #region GetUserPortfolio

        [Test]
        public void IfRequestIsNull_GetUserPortfolio_ShouldReturnBadRequest()
        {
            _brokerController.GetUserPortfolio().Result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void IfNoAuthenticationHeaderIsProvided_GetUserPortfolio_ShouldReturnBadRequest()
        {
            var httpContext = new DefaultHttpContext();
            _brokerController = new BrokerController(_tickersRepository, _usersRepository, _tokenAuthorizer, A.Fake<ILogger<BrokerController>>())
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = httpContext
                    }
                };

            _brokerController.GetUserPortfolio().Result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void IfTokenAuthorizerThrowsUserNotAuthorizedException_GetUserPortfolio_ShouldReturnUnauthorized()
        {
            var httpContext = AValidHttpContext();
            _brokerController = new BrokerController(_tickersRepository, _usersRepository, _tokenAuthorizer,  A.Fake<ILogger<BrokerController>>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            A.CallTo(() => _tokenAuthorizer.GetAuthorizedUsernameOrFail(A<string>._)).Throws<UserNotAuthorizedException>();

            _brokerController.GetUserPortfolio().Result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Test]
        public void IfUserRepositoryThrowsUserNotFoundException_GetUserPortfolio_ShouldReturnNotFound()
        {
            var httpContext = AValidHttpContext();
            _brokerController = new BrokerController(_tickersRepository, _usersRepository, _tokenAuthorizer, A.Fake<ILogger<BrokerController>>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            const string aUsername = "aUsername";
            A.CallTo(() => _tokenAuthorizer.GetAuthorizedUsernameOrFail(A<string>._)).Returns(aUsername);
            A.CallTo(() => _usersRepository.GetUserByUsername(aUsername)).Throws<UserNotFoundException>();

            _brokerController.GetUserPortfolio().Result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public void IfTickersRepositoryThrowsTickerNotFound_GetUserPortfolio_ShouldReturnNotFound()
        {
            var httpContext = AValidHttpContext();
            _brokerController = new BrokerController(_tickersRepository, _usersRepository, _tokenAuthorizer, A.Fake<ILogger<BrokerController>>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            const string aUsername = "aUsername";
            const string aTicker = "aTicker";
            A.CallTo(() => _tokenAuthorizer.GetAuthorizedUsernameOrFail(A<string>._)).Returns(aUsername);
            A.CallTo(() => _usersRepository.GetUserByUsername(aUsername)).Returns(new User
            {
                Portfolio = new List<string>
                {
                    aTicker
                }
            });
            A.CallTo(() => _tickersRepository.GetCurrentPrice(aTicker)).Throws<TickerNotFoundException>();

            _brokerController.GetUserPortfolio().Result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public void HappyPath()
        {
            var httpContext = AValidHttpContext();
            _brokerController = new BrokerController(_tickersRepository, _usersRepository, _tokenAuthorizer, A.Fake<ILogger<BrokerController>>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            const string aUsername = "aUsername";
            const string aTicker = "aTicker";
            const decimal expectedPrice = 10.50m;
            A.CallTo(() => _tokenAuthorizer.GetAuthorizedUsernameOrFail(A<string>._)).Returns(aUsername);
            A.CallTo(() => _usersRepository.GetUserByUsername(aUsername)).Returns(new User
            {
                Portfolio = new List<string>
                {
                    aTicker
                }
            });
            A.CallTo(() => _tickersRepository.GetCurrentPrice(aTicker)).Returns(expectedPrice);

            var result = _brokerController.GetUserPortfolio().Result;
            result.Value.First().Symbol.Should().Be(aTicker);
            result.Value.First().Price.Should().Be("10.50");
        }

        #endregion

        #region Utility Methods

        private static DefaultHttpContext AValidHttpContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] =
                "Basic ZXlKaGJHY2lPaUpTVXpJMU5pSXNJbXRwWkNJNklqY3dOekpFUVRNNVJqVkVNRGN3UkRWRE9EVTNORFUzUlRVMk9VRXpORUV4SWl3aWRIbHdJam9pWVhRcmFuZDBJbjAuZXlKdVltWWlPakUyTXpVeE5qa3pOalFzSW1WNGNDSTZNVFl6TlRFM01qazJOQ3dpYVhOeklqb2lhSFIwY0hNNkx5OXNiMk5oYkdodmMzUTZOVEl3TVNJc0ltRjFaQ0k2V3lKcGJuUnliM053WldOMElpd2liR2wwZEd4bGFtOW9ibUZ3YVNKZExDSmpiR2xsYm5SZmFXUWlPaUoxYzJWeU1TSXNJbXAwYVNJNklqUkROVFl3UkRBek16RkdOVVV6TUVKR01UVTRNMFUyTlRFMFFUUXdPVEF4SWl3aWFXRjBJam94TmpNMU1UWTVNelkwTENKelkyOXdaU0k2V3lKcGJuUnliM053WldOMExuSmxZV1FpTENKc2FYUjBiR1ZxYjJodVlYQnBMbkpsWVdRaVhYMC5QWDR2cmxlbXE4bndvQS15WTF2QUplaHgyOG9mZXljcVhIMzgyb3lDNERfU1ZxbVdRc3R1UU1CUlNEUFlXZkR2WXo5cGtsRFAzTmNNNWliTFpPWkhWWWttSUFvS1NfbjJqd1dvZWg0YlprLXJaMVhjaHJRaDczaTg3UmlVS0FsMDkxeldYeUU3bnd0SlV4cHBHU2hUQXo4cDRla1k2YllaM2E4OFoyMmc5WEwweTNhdy1QYXpwN0hoazdMclZFemF0bFduRVpfS0wxOTU2Z08tYk9yRTRyT3NDaWw1aUJXQTdyNEJwWWtRdUVyVTZaV2MtOWhwX2pUNGlQUTlCUEZLT25ZNnFuTHhNQk5hZmJsUk1Sd2xOaUxHbWdrTFM4X1lKWEUtbno4VE9seEx6X1BmZC1weDBsM2x1dlNPMkdwOTFLSTU5VHY2Zkx5UEM1b1hDQi1TWWc=";
            return httpContext;
        }

        #endregion
    }
}