using Common;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class LogsOperations
    {
        public void AddLog(Log l)
        {
            LogsRepository lr = new LogsRepository();
            lr.AddLog(l);
        }
    }
}
