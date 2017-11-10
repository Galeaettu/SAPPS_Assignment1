using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
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
            if (new UsersOperations().Login(username, password) == true)
            {
                FormsAuthentication.SetAuthCookie(username, true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["message"] = "Account does not exist";
            }
            return View();
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}