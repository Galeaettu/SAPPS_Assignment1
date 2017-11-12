using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class LoginAttemptsRepository : ConnectionClass
    {
        public LoginAttemptsRepository() : base()
        {

        }

        public List<LoginAttempt> GetAttempts()
        {
            return Entity.LoginAttempts.ToList();
        }

        public LoginAttempt GetAttempt(string username)
        {
            return Entity.LoginAttempts.SingleOrDefault(x => x.Username_fk == username);
        }

        public bool DoesAttemptExist(string username)
        {
            return (GetAttempt(username) == null) ? false : true;
        }

        public void AddAttempt(LoginAttempt l)
        {
            Entity.LoginAttempts.Add(l);
            Entity.SaveChanges();
        }
    }
}
