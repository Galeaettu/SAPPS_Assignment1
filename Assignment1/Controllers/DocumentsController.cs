using BusinessLogic;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment1.Controllers
{
    public class DocumentsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            DocumentsOperations dops = new DocumentsOperations();
            var myList = dops.GetAllDocuments(User.Identity.Name);

            return View(myList);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Document d)
        {
            DocumentsOperations dop = new DocumentsOperations();
            dop.AddDocument(User.Identity.Name, d);
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AllocateReviewer(string reviewer, int documentId)
        {
            DocumentsOperations dops = new DocumentsOperations();

            try
            {
                dops.AllocateReviewerToDocument(documentId, reviewer);
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
            }
            return View();
        }
    }
}