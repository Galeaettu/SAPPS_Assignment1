﻿using System;
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
