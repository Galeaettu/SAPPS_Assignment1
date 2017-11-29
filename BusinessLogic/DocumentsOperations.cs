using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Common;

namespace BusinessLogic
{
    public class DocumentsOperations
    {
        #region Read

        public List<Document> GetAllDocuments(string username)
        {
            DocumentsRepository dr = new DocumentsRepository();

            User u = new UsersRepository().GetUser(username);
            return dr.GetDocuments(u);
        }

        public List<Document> GetReviewDocuments(string username)
        {
            DocumentsRepository dr = new DocumentsRepository();

            User u = new UsersRepository().GetUser(username);
            return dr.GetReviewDocuments(u);
        }

        public Document GetDocument(int documentid)
        {
            DocumentsRepository dr = new DocumentsRepository();

            return dr.GetDocument(documentid);
        }

        public List<Comment> GetComments(int documentId)
        {
            DocumentsRepository dr = new DocumentsRepository();
            Document d = dr.GetDocument(documentId);
            return dr.GetComments(d);
        }

        public bool IsReviewerAllocatedToDocument(string username, int documentId)
        {
            DocumentsRepository dr = new DocumentsRepository();
            if (!dr.DoesDocumentExist(documentId))
            {
                throw new DocumentExistsException("Document does not exist");
            }

            User u = dr.GetUser(username);
            Document d = dr.GetDocument(documentId);
            return dr.IsReviewerAllocatedToDocument(u, d);
        }

        public bool DoesDocumentExist(int documentId)
        {
            return new DocumentsRepository().DoesDocumentExist(documentId);
        }

        #endregion

        #region Write

        public void AddDocument(string username, Document d)
        {
            UsersRepository ur = new UsersRepository();
            User u = ur.GetUser(username);

            DocumentsRepository dr = new DocumentsRepository();
            dr.AllocateDocumentToUser(u, d);
            dr.AddDocument(d, u);
        }

        public void AllocateReviewerToDocument(int document, string username)
        {
            DocumentsRepository dr = new DocumentsRepository();

            User u = dr.GetUser(username);
            Document d = dr.GetDocument(document);

            if (dr.IsReviewerAllocatedToDocument(u,d))
            {
                throw new Exception("Reviewer is already allocated to the document.");
            }
            else
            {
                dr.AllocateReviewerToDocument(u, d);
            }
        }

        public void DeAllocateReviewerFromDocument(int document, string username)
        {
            DocumentsRepository dr = new DocumentsRepository();

            User u = dr.GetUser(username);
            Document d = dr.GetDocument(document);

            if (!dr.IsReviewerAllocatedToDocument(u, d))
            {
                throw new Exception("Reviewer is not allocated to the document.");
            }
            else
            {
                dr.DeAllocateReviewerFromDocument(u, d);
            }
        }

        public void AddComment(Document d, Comment c, string username)
        {
            DocumentsRepository dr = new DocumentsRepository();
            UsersRepository ur = new UsersRepository
            {
                Entity = dr.Entity
            };
            User u = ur.GetUser(username);
            d = dr.GetDocument(d.Id);

            if (dr.IsReviewerAllocatedToDocument(u, d))
            {
                try
                {
                    c.Username_fk = username;
                    c.Document_fk = d.Id;
                    c.DatePlaced = DateTime.Now;
                    dr.AllocateComment(u, d, c);
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot add comment");
                }
            }
            else
            {
                throw new Exception("Reviewer is not allocated to the document.");
            }
        }

        #endregion
    }
}
