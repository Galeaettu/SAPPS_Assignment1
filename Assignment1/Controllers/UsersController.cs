using BusinessLogic;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User u)
        {
            UsersOperations uo = new UsersOperations();

            try
            {
                if (ModelState.IsValid)
                {
                    uo.Register(u);

                    ViewData["message"] = "User registered successfully";
                    ModelState.Clear();
                }
            }
            catch(UsernameExistsException ex)
            {
                ModelState.AddModelError("Username", ex.Message);
            }
            catch(Exception ex)
            {
                // Add log to error
                ViewData["message"] = "User registration failed";
            }
            return View();
        }
    }
}