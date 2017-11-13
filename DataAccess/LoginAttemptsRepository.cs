using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public LoginAttempt GetAttempt(string keyword)
        {
            return Entity.LoginAttempts.SingleOrDefault(x => x.Username == keyword || x.Ip_Address == keyword);
        }

        public bool DoesAttemptExist(string keyword)
        {
            return (GetAttempt(keyword) == null) ? false : true;
        }

        public void AddAttempt(LoginAttempt l)
        {
            Entity.LoginAttempts.Add(l);
            Entity.SaveChanges();
        }

        public string GetIP()
        {
            string strHostName = "";
            strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();

        }
    }
}
