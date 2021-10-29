using System.Collections.Generic;

namespace LittleJohnWebAPI.Utils
{
    public interface ITokenUtils
    {
        IEnumerable<string> GetUserPortfolioOrFail(string accessToken);
    }
}