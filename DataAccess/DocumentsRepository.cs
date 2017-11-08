using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DocumentsRepository : ConnectionClass
    {
        public DocumentsRepository() : base(){

        }

        public List<Document> GetDocuments()
        {
            return Entity.Documents.ToList();
        }

        public List<Document> GetDocuments(string username)
        {
            return Entity.Documents.Where(x => x.Username_fk == username).ToList();
        }

        public List<Document> GetDocument(int id)
        {
            return Entity.Documents.Where(x => x.Id == id).ToList();
        }

        /// <summary>
        /// Allocate a Document to be available to a Reviewer.
        /// </summary>
        /// <param name="u">Reviewer to view the document</param>
        /// <param name="d">The document to be viewed</param>
        public void AllocateDocumentToUser(User u, Document d)
        {
            u.Documents.Add(d);
            Entity.SaveChanges();
        }

        /// <summary>
        /// Checks if the user is already allocated to the document.
        /// </summary>
        /// <param name="u">User</param>
        /// <param name="d">Document</param>
        /// <returns></returns>
        public bool IsUserAllocatedToDocument(User u, Document d)
        {
            return u.Documents.Contains(d);
        }

        public void Delete(Document d)
        {
            Entity.Documents.Remove(d);
            Entity.SaveChanges();
        }

    }
}
