namespace LittleJohnWebAPI.Data.Users
{
    internal interface IFakeUsersService
    {
        User GetUserByUsername(string username);
    }
}