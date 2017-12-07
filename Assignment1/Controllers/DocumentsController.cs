using BusinessLogic;
using Common;
using System;
using System.Collections.Generic;
using System.IO;
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
        public ActionResult Add(Document d, HttpPostedFileBase filePath)
        {
            DocumentsOperations dops = new DocumentsOperations();
            try
            {
                if (filePath != null)
                {
                    if (Path.GetExtension(filePath.FileName).ToLower().Equals(".docx"))
                    {
                        if (filePath.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                        {
                            byte[] whitelist = new byte[] { 80, 75, 3, 4, 20, 0, 6, 0 };
                            byte[] inputRead = new byte[8];
                            filePath.InputStream.Read(inputRead, 0, 8);

                            bool flag = true;
                            for (int i = 0; i < 8; i++)
                            {
                                if (whitelist[i] != inputRead[i])
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag == true)
                            {
                                if (filePath.ContentLength <= (1048576 * 5))
                                {
                                    string absolutePath = Server.MapPath("\\UploadedDocuments\\");
                                    string relativePath = "\\UploadedDocuments\\";

                                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(filePath.FileName);

                                    d.FilePath = relativePath + fileName; // saves path to the image in the database

                                    filePath.InputStream.Position = 0;
                                    Stream s = new Encryption().HybridEncryptFile(filePath.InputStream, User.Identity.Name, new UsersOperations().GetUser(User.Identity.Name).PublicKey);
                                    s.Position = 0;
                                    FileStream fs = new FileStream(absolutePath + fileName, FileMode.CreateNew, FileAccess.Write);
                                    s.CopyTo(fs);
                                    fs.Close();

                                    s.Position = 0;
                                    d.Signature = new Encryption().DigitalSign(s, new UsersOperations().GetUser(User.Identity.Name).PrivateKey);
                                    dops.AddDocument(User.Identity.Name, d);

                                    ViewData["success_message"] = "Document uploaded successfully";
                                    ModelState.Clear();
                                }
                                else
                                {
                                    new LogsOperations().AddLog(
                                        new Log()
                                        {
                                            Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                            Exception = "Very large document",
                                            Time = DateTime.Now,
                                            Message = "Very large document"
                                        }
                                    );

                                    ViewData["message"] = "The document must be smaller than 5MB";
                                }
                            }
                            else
                            {
                                new LogsOperations().AddLog(
                                    new Log()
                                    {
                                        Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                        Exception = "The header values were not of a Word Document",
                                        Time = DateTime.Now,
                                        Message = "Not a word document"
                                    }
                                );

                                ViewData["message"] = "This is not a valid .docx file";
                            }
                        }
                        else
                        {
                            new LogsOperations().AddLog(
                                new Log()
                                {
                                    Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                    Exception = "Content Type was not of a Word Document",
                                    Time = DateTime.Now,
                                    Message = "Not a word document"
                                }
                            );

                            ViewData["message"] = "This is not a valid .docx file";
                        }
                    }
                    else
                        {
                            new LogsOperations().AddLog(
                                new Log()
                                {
                                    Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                    Exception = "File did not end with .docx",
                                    Time = DateTime.Now,
                                    Message = "Not a .docx file"
                                }
                            );

                            ViewData["message"] = "This file is not a document";
                        }
                }
                else
                {
                    new LogsOperations().AddLog(
                        new Log()
                        {
                            Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                            Exception = "No document was selected to be uploaded",
                            Time = DateTime.Now,
                            Message = "No document"
                        }
                    );

                    ViewData["message"] = "Please select a document";
                }

            }
            catch (DocumentExistsException de)
            {
                new LogsOperations().AddLog(
                    new Log()
                    {
                        Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                        Exception = de.Message,
                        Time = DateTime.Now,
                        Message = de.Message
                    }
                );

                ViewData["error_message"] = de.Message;
            }catch (Exception ex)
            {
                new LogsOperations().AddLog(
                    new Log()
                    {
                        Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                        Exception = ex.Message,
                        Time = DateTime.Now,
                        Message = "Unable to add document"
                    }
                );

                ViewData["error_message"] = "Unable to add document";
            }
            return View();
        }

        [Authorize]
        public ActionResult DownloadFile(string documentId)
        {
            if (documentId != null)
            {
                int decryptedDocumentId = 0;
                try
                {
                    decryptedDocumentId = Convert.ToInt32(new Encryption().DecryptString(documentId, User.Identity.Name));
                }
                catch (FormatException fe)
                {
                    TempData["error_message"] = "Document does not exist";

                    new LogsOperations().AddLog(
                        new Log()
                        {
                            Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                            Exception = fe.Message,
                            Time = DateTime.Now,
                            Message = "User tried to manually search for a document in the address bar"
                        }
                    );
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["error_message"] = "Document unavailable";
                    new LogsOperations().AddLog(
                        new Log()
                        {
                            Controller = "Comment",
                            Exception = ex.Message,
                            Time = DateTime.Now,
                            Message = "documentId decryption error"
                        }
                    );
                    return RedirectToAction("Index");
                }


                DocumentsOperations dops = new DocumentsOperations();
                if (dops.DoesDocumentExist(decryptedDocumentId))
                {
                    try
                    {
                        Document d = dops.GetDocument(decryptedDocumentId);
                        if (dops.IsReviewerAllocatedToDocument(User.Identity.Name, decryptedDocumentId))
                        {
                            string absolutePath = Server.MapPath(d.FilePath);

                            if (System.IO.File.Exists(absolutePath) == true)
                            {
                                FileStream fs = System.IO.File.OpenRead(absolutePath);
                                MemoryStream ms = new MemoryStream();
                                fs.CopyTo(ms);
                                ms.Position = 0;

                                try
                                {
                                    if (new Encryption().DigitalVerify(ms, new UsersOperations().GetUser(d.Username_fk).PublicKey, new DocumentsOperations().GetDocument(decryptedDocumentId).Signature))
                                    {
                                        MemoryStream msOut = new MemoryStream(new Encryption().HybridDecryptFile(ms, new UsersOperations().GetUser(d.Username_fk).PrivateKey));
                                        msOut.Position = 0;
                                        return File(msOut.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet, d.FilePath);
                                    }
                                    else
                                    {
                                        TempData["error_message"] = "Unable to verify document";
                                        new LogsOperations().AddLog(
                                            new Log()
                                            {
                                                Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                                Exception = "Unable to verify document",
                                                Time = DateTime.Now,
                                                Message = "Unable to verify document"
                                            }
                                        );
                                        return RedirectToAction("Index");
                                    }
                                }
                                catch(Exception ex)
                                {
                                    TempData["error_message"] = "Unable to verify document";
                                    new LogsOperations().AddLog(
                                        new Log()
                                        {
                                            Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                            Exception = "Unable to verify document",
                                            Time = DateTime.Now,
                                            Message = "Unable to verify document"
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
                    catch(Exception ex)
                    {
                        TempData["error_message"] = "Unable to download document";
                        new LogsOperations().AddLog(
                            new Log()
                            {
                                Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                Exception = ex.Message,
                                Time = DateTime.Now,
                                Message = "Unable to download document"
                            }
                        );
                        return RedirectToAction("Index");
                    }
                }else
                {
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
        public ActionResult Comment(string documentId)
        {
            if (documentId != null)
            {
                int documentIdToFind = 0;
                try
                {
                    documentIdToFind = Convert.ToInt32(new Encryption().DecryptString(documentId, User.Identity.Name));
                }
                catch(FormatException fe)
                {
                    TempData["error_message"] = "Document does not exist";

                    new LogsOperations().AddLog(
                        new Log()
                        {
                            Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                            Exception = fe.Message,
                            Time = DateTime.Now,
                            Message = "User tried to manually search for a document in the address bar"
                        }
                    );
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    TempData["error_message"] = "Document unavailable";
                    new LogsOperations().AddLog(
                        new Log()
                        {
                            Controller = "Comment",
                            Exception = ex.Message,
                            Time = DateTime.Now,
                            Message = "documentId decryption error"
                        }
                    );
                    return RedirectToAction("Index");
                }

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
                    catch(Exception ex)
                    {
                        TempData["error_message"] = ex.Message;
                        new LogsOperations().AddLog(
                            new Log()
                            {
                                Controller = RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(),
                                Exception = ex.Message,
                                Time = DateTime.Now,
                                Message = "Error checking reviewing permissions"
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