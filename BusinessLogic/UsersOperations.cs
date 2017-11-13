using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Common;
using System.Web;
using System.Net;
using System.Web.Security;
using System.Net.Sockets;

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
            //return new UsersRepository().Login(username, new Encryption().HashString(password));

            LoginAttemptsRepository loginAttempts = new LoginAttemptsRepository();

            bool validLogin = new UsersRepository().Login(username, new Encryption().HashString(password));
            if (validLogin == true)
            {
                return validLogin;
            }
            else
            {
                string userIp = loginAttempts.GetIP();

                if ((loginAttempts.DoesAttemptExist(username) == false ) && (loginAttempts.DoesAttemptExist(userIp) == false))
                {
                    LoginAttempt l = new LoginAttempt
                    {
                        Username = username,
                        Attempt = 1,
                        Time = DateTime.Now,
                        Blocked = false,
                        Ip_Address = userIp
                    };
                    loginAttempts.AddAttempt(l);
                }
                else
                {
                    LoginAttempt old;

                    if (loginAttempts.DoesAttemptExist(username) == true)
                    {
                        old = loginAttempts.GetAttempt(username);


                    }
                    else
                    {
                        old = loginAttempts.GetAttempt(userIp);
                    }

                    LoginAttempt update = new LoginAttempt
                    {
                        Username = username,
                        Attempt = old.Attempt++,
                        Time = DateTime.Now,
                        Blocked = false,
                        Ip_Address = userIp
                    };
                    loginAttempts.UpdateAttempt(old, update);

                }
                return false;
            }
        }

        #endregion

        #region Write Operations

        public void Register(User u)
        {
            UsersRepository ur = new UsersRepository();
            if (ur.DoesUsernameExist(u.Username) == true)
            {
                throw new UsernameExistsException("Username already exists");
            }
            else
            {
                u.Password = new Encryption().HashString(u.Password);
                ur.AddUser(u);
                ur.AllocateRoleToUser(u, ur.GetRole(1));
            }
        }

        public void Delete(string username)
        {
            UsersRepository ur = new UsersRepository();
            if (ur.DoesUsernameExist(username))
            {
                ur.Delete(ur.GetUser(username));
            }
            else throw new UsernameExistsException("Username does not exist.");
        }

        public void AlocateRoleToUser(string username, int roleId)
        {
            UsersRepository ur = new UsersRepository();
            User u = ur.GetUser(username);
            Role r = ur.GetRole(roleId);

            if (ur.IsUserAllocatedToRole(u, r))
            {
                throw new Exception("User cannot be allocated to role because he already has that role");
            }
            else
            {
                ur.AllocateRoleToUser(u, r);
            }
        }

        #endregion
    }
}
