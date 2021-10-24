namespace LittleJohnWebAPI.Data.Users
{
    public interface IUsersRepository
    {
        User GetUserByUsername(string username);
    }
}