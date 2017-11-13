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

        public List<LoginAttempt> GetUsernameAttempts()
        {
            return Entity.LoginAttempts.ToList();
        }

        public List<IpAttempt> GetIpAttempts()
        {
            return Entity.IpAttempts.ToList();
        }

        public LoginAttempt GetUsernameAttempt(string username)
        {
            return Entity.LoginAttempts.SingleOrDefault(x => x.Username == username);
        }

        public IpAttempt GetIpAttempt(string address)
        {
            return Entity.IpAttempts.SingleOrDefault(x => x.Ip_Address == address);
        }

        public bool DoesUsernameAttemptExist(string username)
        {
            return (GetUsernameAttempt(username) == null) ? false : true;
        }

        public bool DoesIpAttemptExist(string address)
        {
            return (GetIpAttempt(address) == null) ? false : true;
        }

        public void AddUsernameAttempt(LoginAttempt l)
        {
            Entity.LoginAttempts.Add(l);
            Entity.SaveChanges();
        }

        public void AddIpAttempt(IpAttempt i)
        {
            Entity.IpAttempts.Add(i);
            Entity.SaveChanges();
        }

        public void UpdateUsernameAttempt(LoginAttempt old, LoginAttempt updated)
        {
            old.Username = updated.Username;
            old.Time = updated.Time;
            old.Blocked = updated.Blocked;
            Entity.SaveChanges();
        }

        public void UpdateIpAttempt(IpAttempt old, IpAttempt updated)
        {
            old.Ip_Address = updated.Ip_Address;
            old.Time = updated.Time;
            old.Blocked = updated.Blocked;
            Entity.SaveChanges();
        }

        public void DeleteUsernameAttempt(string username)
        {
            Entity.LoginAttempts.Remove(GetUsernameAttempt(username));
            Entity.SaveChanges();
        }

        public void DeleteIpAttempt(string address)
        {
            Entity.IpAttempts.Remove(GetIpAttempt(address));
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
