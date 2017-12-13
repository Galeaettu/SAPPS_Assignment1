using BusinessLogic;
using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace Assignment1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            EncryptConnString();
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            new LogsOperations().AddLog(
                new Log()
                {
                    Controller = "",
                    Exception = ex.Message,
                    Time = DateTime.Now,
                    Message = "Unhandled exception"
                }
            );
        }

        public void EncryptConnString()
        {

            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            ConfigurationSection section = config.GetSection("connectionStrings");
            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                config.Save();
            }
        }
    }
}
