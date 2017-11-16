using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class LogsRepository : ConnectionClass
    {
        public LogsRepository() : base()
        {

        }

        public void AddLog(Log l)
        {
            Entity.Logs.Add(l);
            Entity.SaveChanges();
        }
    }
}
