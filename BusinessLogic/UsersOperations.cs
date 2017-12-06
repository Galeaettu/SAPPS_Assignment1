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

        public List<User> GetAllUsers(string exceptUsername)
        {
            UsersRepository ur = new UsersRepository();
            List<User> user = ur.GetSingleUser(exceptUsername);
            return ur.GetUsers().Except(user).ToList();
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

                if (loginAttempts.DoesUsernameAttemptExist(username) == false )
                {
                    LoginAttempt l = new LoginAttempt
                    {
                        Username = username,
                        Attempt = 1,
                        Time = DateTime.Now,
                        Blocked = false,
                    };
                    loginAttempts.AddUsernameAttempt(l);
                }
                else
                {
                    LoginAttempt oldusername;

                    if (loginAttempts.DoesUsernameAttemptExist(username) == true)
                    {
                        oldusername = loginAttempts.GetUsernameAttempt(username);
                        bool usernameBlockCheck = (oldusername.Attempt >= 2) ? true : false;

                        LoginAttempt updateUsername = new LoginAttempt
                        {
                            Username = username,
                            Attempt = oldusername.Attempt++,
                            Time = oldusername.Time,
                            Blocked = usernameBlockCheck,
                        };
                        loginAttempts.UpdateUsernameAttempt(oldusername, updateUsername);
                    }

                }

                if (loginAttempts.DoesIpAttemptExist(userIp) == false)
                {
                    IpAttempt i = new IpAttempt
                    {
                        Attempt = 1,
                        Time = DateTime.Now,
                        Blocked = false,
                        Ip_Address = userIp
                    };
                    loginAttempts.AddIpAttempt(i);
                }
                else
                {
                    IpAttempt oldIp;

                    if (loginAttempts.DoesIpAttemptExist(userIp) == true)
                    {
                        oldIp = loginAttempts.GetIpAttempt(userIp);
                        bool ipBlockCheck = (oldIp.Attempt >= 2) ? true : false;

                        IpAttempt updateIp = new IpAttempt
                        {
                            Ip_Address = userIp,
                            Attempt = oldIp.Attempt++,
                            Time = oldIp.Time,
                            Blocked = ipBlockCheck,
                        };
                        loginAttempts.UpdateIpAttempt(oldIp, updateIp);
                    }                    

                }
                return validLogin;
            }
        }

        public bool IsBlocked(string username)
        {
            LoginAttemptsRepository loginAttempts = new LoginAttemptsRepository();
            string userIp = loginAttempts.GetIP();

            bool usernameBlocked = false;
            bool ipBlocked = false;

            if (loginAttempts.DoesUsernameAttemptExist(username) == true)
            {
                LoginAttempt l = loginAttempts.GetUsernameAttempt(username);
                if ((bool)l.Blocked)
                {
                    TimeSpan elapsed = DateTime.Now.Subtract((DateTime)l.Time);
                    usernameBlocked = (elapsed.Minutes >= 15) ? false : true;
                    if (usernameBlocked == false)
                        loginAttempts.DeleteUsernameAttempt(l);
                }
            }
            if (loginAttempts.DoesIpAttemptExist(userIp) == true)
            {
                IpAttempt i = loginAttempts.GetIpAttempt(userIp);
                if ((bool)i.Blocked)
                {
                    TimeSpan elapsed = DateTime.Now.Subtract((DateTime)i.Time);
                    ipBlocked = (elapsed.Minutes >= 15) ? false : true;
                    if (ipBlocked == false)
                        loginAttempts.DeleteIpAttempt(i);
                }
            }
            return (usernameBlocked || ipBlocked);
        }

        public int RemainingBlocked(string username)
        {
            LoginAttemptsRepository loginAttempts = new LoginAttemptsRepository();
            string userIp = loginAttempts.GetIP();
            TimeSpan elapsedUsername = TimeSpan.Zero;
            TimeSpan elapsedIp = TimeSpan.Zero;

            TimeSpan result = TimeSpan.Zero;

            if (loginAttempts.DoesUsernameAttemptExist(username) == true)
            {
                LoginAttempt l = loginAttempts.GetUsernameAttempt(username);
                elapsedUsername = DateTime.Now.Subtract((DateTime)l.Time);
            }
            if (loginAttempts.DoesIpAttemptExist(userIp) == true)
            {
                IpAttempt i = loginAttempts.GetIpAttempt(userIp);
                elapsedIp = DateTime.Now.Subtract((DateTime)i.Time);
            }

            if ((elapsedUsername > TimeSpan.Zero) && (elapsedIp == TimeSpan.Zero))
            {
                result = elapsedUsername;
            }else if ((elapsedIp > TimeSpan.Zero) && (elapsedUsername == TimeSpan.Zero))
            {
                result = elapsedIp;
            }
            else
            {
                int compare = TimeSpan.Compare(elapsedUsername, elapsedIp);
                switch (compare)
                {
                    case -1:
                        result = elapsedUsername;
                        break;
                    case 0:
                        result = elapsedUsername;
                        break;
                    case 1:
                        result = elapsedUsername;
                        break;
                }
            }
            return (15 - (int)result.TotalMinutes);
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

                AsymmetricParameters myParameters = new Encryption().GenerateAsymmetricParameters();
                u.PublicKey = myParameters.PublicKey;
                u.PrivateKey = myParameters.PrivateKey;

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
