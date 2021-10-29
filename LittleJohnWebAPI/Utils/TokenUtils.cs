using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace LittleJohnWebAPI.Utils
{
    internal class TokenUtils : ITokenUtils
    {
        public IEnumerable<string> GetUserPortfolioOrFail(string accessToken)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();

                var validationParams = new TokenValidationParameters
                {
                    ValidateLifetime = false,
                    SignatureValidator = (token, _) =>
                    {
                        var jwt = new JwtSecurityToken(token);

                        return jwt;
                    },
                    ValidateAudience = false,
                    ValidateIssuer = false
                };

                var tokenInVerification = jwtTokenHandler.ValidateToken(accessToken, validationParams, out var validatedToken);

                var portfolio = tokenInVerification.Claims.Where(claim => claim.Type == "portfolio").Select(claim => claim.Value).ToList();

                if (!portfolio.Any() || portfolio.Count > 10)
                {
                    throw new InvalidPortfolioException();
                }

                return portfolio;
            }
            catch (ArgumentException e)
            {
                throw new InvalidTokenException("Invalid access token provided", e);
            }
        }
    }
}