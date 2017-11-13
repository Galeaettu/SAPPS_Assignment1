using BusinessLogic;
using Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
                    if ((u.Password.Length >= 6) && (u.Password.Length <= 15))
                    {
                        if(Regex.IsMatch(u.Password, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,15}$"))
                        {
                            if (ModelState.IsValid)
                            {
                                uo.Register(u);

                                ViewData["success_message"] = "User registered successfully";
                                ModelState.Clear();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Password", "Passwords must contain at least one digit, one uppercase and one lowercase");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Passwords must be between 6 and 15 characters long");
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
            else
            {
                ViewData["error_message"] = "Invalid reCaptcha";
            }
            return View();
        }
    }
}