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
        public ActionResult Review()
        {
            DocumentsOperations dops = new DocumentsOperations();
            var myList = dops.GetReviewDocuments(User.Identity.Name);

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

            ViewData["success_message"] = "Document added successfully";
            ModelState.Clear();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AllocateReviewer(string reviewer, int documentId, string permissionType)
        {
            DocumentsOperations dops = new DocumentsOperations();

            try
            {
                if(permissionType == "Add")
                {
                    dops.AllocateReviewerToDocument(documentId, reviewer);
                    TempData["success_message"] = "Reviewer has been allocated to the document";
                }
                else
                {
                    dops.DeAllocateReviewerFromDocument(documentId, reviewer);
                    TempData["success_message"] = "Reviewer was deallocated from the document";
                }
                
            }
            catch(Exception ex)
            {
                TempData["error_message"] = ex.Message;
                new LogsOperations().AddLog(
                    new Log()
                    {
                        Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                        Exception = ex.Message,
                        Time = DateTime.Now,
                        Message = reviewer + "Faliure allocating reviewer"
                    }
                );
            }

            return RedirectToAction("Index", "Documents");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Comment(int? documentId)
        {
            if (documentId != null)
            {
                int documentIdToFind = (int)documentId;
                DocumentsOperations dops = new DocumentsOperations();
                if (dops.DoesDocumentExist(documentIdToFind))
                {
                    try
                    {
                        Document d = dops.GetDocument(documentIdToFind);
                        if (dops.IsReviewerAllocatedToDocument(User.Identity.Name, documentIdToFind) || d.Username_fk == User.Identity.Name)
                        {
                            ViewData["document_title"] = d.Title;
                            ViewData["document_id"] = d.Id;

                            return View();
                        }
                        else
                        {
                            TempData["error_message"] = "You are not a reviewer of this document";
                            new LogsOperations().AddLog(
                                new Log()
                                {
                                    Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                    Exception = "User is not document's reviewer",
                                    Time = DateTime.Now,
                                    Message = "User is not document's reviewer"
                                }
                            );
                            return RedirectToAction("Index");
                        }
                    }
                    catch (DocumentExistsException ex)
                    {
                        TempData["error_message"] = ex.Message;
                        new LogsOperations().AddLog(
                            new Log()
                            {
                                Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                Exception = ex.Message,
                                Time = DateTime.Now,
                                Message = ex.Message
                            }
                        );

                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["error_message"] = "Document does not exist";
                    new LogsOperations().AddLog(
                        new Log()
                        {
                            Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                            Exception = "Document does not exist",
                            Time = DateTime.Now,
                            Message = "Document does not exist"
                        }
                    );
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["error_message"] = "No document selected";
                new LogsOperations().AddLog(
                    new Log()
                    {
                        Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                        Exception = "No document selected",
                        Time = DateTime.Now,
                        Message = "No document selected"
                    }
                );
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment(int document, Comment c)
        {
            DocumentsOperations dops = new DocumentsOperations();
            Document d = dops.GetDocument(document);
            ViewData["document_id"] = d.Id;
            try
            {
                dops.AddComment(d, c, User.Identity.Name);
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                ViewData["error_message"] = ex.Message;
                new LogsOperations().AddLog(
                    new Log()
                    {
                        Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                        Exception = ex.Message,
                        Time = DateTime.Now,
                        Message = "Adding comment exception"
                    }
                );
            }
            return View();
        }
    }
}