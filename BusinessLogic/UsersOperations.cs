using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Common;

namespace BusinessLogic
{
    public class UsersOperations
    {
        #region Read Operations

        public List<User> GetAllUsers()
        {
            UsersRepository ur = new UsersRepository();
            return ur.GetUsers();
        }

        public List<User> Search(string keyword)
        {
            UsersRepository ur = new UsersRepository();
            if (string.IsNullOrEmpty(keyword))
                return ur.GetUsers();
            else
                return ur.GetUsers(keyword);
        }

        public User GetUser(string username)
        {
            return new UsersRepository().GetUser(username);
        }

        public bool Login(string username, string password)
        {
            //logic to validate how many attempts have been tried by this username to log in.

            return new UsersRepository().Login(username, password);
        }

        #endregion 
    }
}
