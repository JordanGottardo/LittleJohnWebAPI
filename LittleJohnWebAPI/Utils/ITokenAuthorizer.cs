using System.Threading.Tasks;

namespace LittleJohnWebAPI.Utils
{
    public interface ITokenAuthorizer
    {
        Task<string> GetAuthorizedUsernameOrFail(string accessToken);
    }
}