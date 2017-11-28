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

        public List<Document> GetDocuments(User u)
        {
            //return Entity.Documents.Where(x => x.Username_fk == username).ToList();
            return u.Documents.ToList();
        }

        public Document GetDocument(int id)
        {
            return Entity.Documents.SingleOrDefault(x => x.Id == id);
        }

        public void AddDocument(Document d, User u)
        {
            d.Username_fk = u.Username;
            Entity.Documents.Add(d);
            Entity.SaveChanges();
        }

        public void AllocateDocumentToUser(User u, Document d)
        {
            u.Documents.Add(d);
            Entity.SaveChanges();
        }

        public void DeallocateDocumentFromUser(User u, Document d)
        {
            u.Documents.Remove(d);
            Entity.SaveChanges();
        }

        public User GetUser(string username)
        {
            return Entity.Users.SingleOrDefault(x => x.Username == username);
        }

        public void AllocateReviewerToDocument(User u, Document d)
        {
            
            d.Users.Add(u);
            Entity.SaveChanges();
        }

        public void DeAllocateReviewerFromDocument(User u, Document d)
        {
            d.Users.Remove(u);
            Entity.SaveChanges();
        }

        public bool IsUserAllocatedToDocument(User u, Document d)
        {
            return u.Documents.Contains(d);
        }

        public bool IsReviewerAllocatedToDocument(User u, Document d)
        {
            return d.Users.Contains(u);
        }

        public void Delete(Document d)
        {
            Entity.Documents.Remove(d);
            Entity.SaveChanges();
        }
        
        public List<Comment> GetComments()
        {
            return Entity.Comments.ToList();
        }

        public List<Comment> GetComments(Document d)
        {
            return d.Comments.ToList();
        }

        public void AllocateComment(User u, Document d, Comment c)
        {
            d.Comments.Add(c);
            u.Comments.Add(c);
            Entity.SaveChanges();
        }
    }
}
