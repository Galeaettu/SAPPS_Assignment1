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

        public void DeAllocateReviewerToDocument(int document, string username)
        {
            UsersRepository ur = new UsersRepository();
            DocumentsRepository dr = new DocumentsRepository();

            User u = ur.GetUser(username);
            Document d = dr.GetDocument(document);

            if (!dr.IsReviewerAllocatedToDocument(u, d))
            {
                throw new Exception("Reviewer is not allocated to the document.");
            }
            else
            {
                dr.DeAllocateReviewerToDocument(u, d);
            }
        }

        #endregion
    }
}
