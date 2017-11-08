using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace DataAccess
{
    public class UsersRepository : ConnectionClass
    {
        public UsersRepository() : base()
        {

        }

        public List<User> GetUsers()
        {
            return Entity.Users.ToList();
        }

        public List<User> GetUsers(string keyword)
        {
            return Entity.Users.Where(x => x.Name.Contains(keyword) || x.Surname.Contains(keyword)).ToList();
        }

        public void AddUser(User u)
        {
            Entity.Users.Add(u);
            Entity.SaveChanges();
        }

        public User GetUser(string username)
        {
            return Entity.Users.SingleOrDefault(x => x.Username == username);
        }

        public bool DoesUsernameExist(string username)
        {
            return (GetUser(username) == null) ? false : true;
        }

        public void AllocateRoleToUser(User u, Role r)
        {
            u.Roles.Add(r);
            Entity.SaveChanges();
        }

        public bool IsUserAllocatedToRole(User u, Role r)
        {
            return u.Roles.Contains(r);
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

        public Role GetRole(int id)
        {
            return Entity.Roles.SingleOrDefault(x => x.Id == id);
        }

        public void Delete(User u)
        {
            Entity.Users.Remove(u);
            Entity.SaveChanges();
        }

        public bool Login(string username, string password)
        {
            return Entity.Users.SingleOrDefault(x => x.Username == username && x.Password == password) == null ? false : true;
        }
    }
}
