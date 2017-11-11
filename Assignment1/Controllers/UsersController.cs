using BusinessLogic;
using Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Assignment1.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
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
        public ActionResult Register(User u)
        {
            UsersOperations uo = new UsersOperations();

            bool status = new AccountsController().VerifyCaptcha(this);

            if (status == false)
                ViewData["error_message"] = "Google reCaptcha validation failed";

            if (status)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        uo.Register(u);

                        ViewData["success_message"] = "User registered successfully";
                        ModelState.Clear();
                    }
                }
                catch (UsernameExistsException ex)
                {
                    ModelState.AddModelError("Username", ex.Message);
                }
                catch (Exception ex)
                {
                    // Add log to error
                    ViewData["error_message"] = "User registration failed";
                }
            }
            return View();
        }
    }
}