using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Web.Security;

namespace Assignment1.Controllers
{
    public class AccountsController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (new UsersOperations().IsBlocked(username))
            {
                int time = new UsersOperations().RemainingBlocked(username);
                ViewData["error_message"] = "You are blocked for "+time+" more minutes. Try again later.";

                new LogsOperations().AddLog(
                    new Common.Log()
                    {
                        Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                        Exception = "User blocked",
                        Time = DateTime.Now,
                        Message = username + " tried to log in but is currently blocked"
                    }
                );
            }
            else
            {
                bool status = false;
                try
                {
                    status = new AccountsController().VerifyCaptcha(this);
                }
                catch (Exception e)
                {
                    ViewData["error_message"] = e.Message;

                    return View();
                }

                if (status == false)
                {
                    ViewData["error_message"] = "Google reCaptcha validation failed";

                    new LogsOperations().AddLog(
                        new Common.Log()
                        {
                            Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                            Exception = "reCaptcha Failed",
                            Time = DateTime.Now,
                            Message = username +": reCaptcha Failed"
                        }
                    );
                }
               
                if (status)
                {
                    if (new UsersOperations().Login(username, password) == true)
                    {
                        FormsAuthentication.SetAuthCookie(username, true);
                        return RedirectToAction("Index", "Documents");
                    }
                    else
                    {
                        new LogsOperations().AddLog(
                            new Common.Log()
                                {
                                    Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                    Exception = "Invalid Password",
                                    Time = DateTime.Now,
                                    Message = username + ": Invalid Password"
                            }
                        );
                        ViewData["message"] = "An invalid username or password was entered. Please try again.";
                    }
                }
            }

            return View();
        }

        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        public bool VerifyCaptcha(Controller controller)
        {
            var status = false;
            try
            {
                var response = controller.Request["g-recaptcha-response"];
                string secretKey = "6LfGBzgUAAAAAH3mHW5T_hveNRdaDC5VkA23qG1L";
                var client = new WebClient();
                var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
                var obj = JObject.Parse(result);
                status = (bool)obj.SelectToken("success");
            }
            catch(Exception e)
            {
                new LogsOperations().AddLog(
                    new Common.Log()
                    {
                        Controller = "VerifyCaptcha",
                        Exception = "reCaptcha failure",
                        Time = DateTime.Now,
                        Message = "reCaptcha failure"
                    }
                );
                //throw new Exception("Unable to verify reCaptcha");
            }
            return status;
        }
    }
}