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
        public ActionResult Login(string username, string password)
        {
            bool status = new AccountsController().VerifyCaptcha(this);

            if (status == false)
                ViewData["error_message"] = "Google reCaptcha validation failed";
            if (status)
            {
                if (new UsersOperations().Login(username, password) == true)
                {
                    FormsAuthentication.SetAuthCookie(username, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["message"] = "An invalid username or password was entered. Please try again.";
                }
            }
            else
            {
                ViewData["error_message"] = "Invalid reCaptcha";
            }

            return View();
        }

        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public bool VerifyCaptcha(Controller controller)
        {
            var response = controller.Request["g-recaptcha-response"];
            string secretKey = "6LfGBzgUAAAAAH3mHW5T_hveNRdaDC5VkA23qG1L";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            return status;
        }
    }
}