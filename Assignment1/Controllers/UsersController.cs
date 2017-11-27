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
            {
                ViewData["error_message"] = "Google reCaptcha validation failed";

                new LogsOperations().AddLog(
                    new Log()
                    {
                        Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                        Exception = "reCaptcha Failed",
                        Time = DateTime.Now,
                        Message = "reCaptcha Failed"
                    }
                );
            }

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
                                TempData["success_message"] = "User registered successfully";
                                ModelState.Clear();
                                return RedirectToAction("Login", "Accounts");
                            }
                            else
                            {
                                new LogsOperations().AddLog(
                                    new Log()
                                    {
                                        Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                        Exception = "Invalid Model State",
                                        Time = DateTime.Now,
                                        Message = "Invalid Model State"
                                    }
                                );
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Password", "Passwords must contain at least one digit, one uppercase and one lowercase");

                            new LogsOperations().AddLog(
                                new Log()
                                {
                                    Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                    Exception = "Weak password strength",
                                    Time = DateTime.Now,
                                    Message = "Weak password strength"
                                }
                            );
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Passwords must be between 6 and 15 characters long");

                        new LogsOperations().AddLog(
                            new Log()
                            {
                                Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                Exception = "Invalid password length",
                                Time = DateTime.Now,
                                Message = "Invalid password length"
                            }
                        );
                    }
                }
                catch (UsernameExistsException ex)
                {
                    ModelState.AddModelError("Username", ex.Message);

                    new LogsOperations().AddLog(
                        new Log()
                        {
                            Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                            Exception = ex.Message.ToString(),
                            Time = DateTime.Now,
                            Message = ex.Message.ToString()
                        }
                    );
                }
                catch (Exception ex)
                {
                    ViewData["error_message"] = "User registration failed";

                    new LogsOperations().AddLog(
                        new Log()
                        {
                            Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                            Exception = ex.Message.ToString(),
                            Time = DateTime.Now,
                            Message = ex.Message.ToString()
                        }
                    );
                }
            }
            return View();
        }
    }
}