using System.Collections.Generic;

namespace LittleJohnWebAPI.Data.Users
{
    public class User
    {
        public string Username { get; set; }

        public List<string> Portfolio { get; set; }
    }
}