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
            var response = Request["g-recaptcha-response"];
            string secretKey = "6LfGBzgUAAAAAH3mHW5T_hveNRdaDC5VkA23qG1L";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
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