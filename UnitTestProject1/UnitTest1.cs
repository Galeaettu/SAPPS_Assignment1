using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using BusinessLogic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestInvalidLogin()
        {
            UsersOperations uo = new UsersOperations();

            //Username and password that do not exist
            String username = "Chrissdafdfasdftian";
            String password = "Pass123";
            try
            {
                //Checking if the result is false, the user does not log in.
                Assert.IsFalse(condition: uo.Login(username, password));
                Assert.IsFalse(condition: uo.IsBlocked(username), message: "User is blocked");
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestBlockedLogin()
        {
            //This test will check whether the user will be blocked after 3 invalid
            //login attempts.

            UsersOperations uo = new UsersOperations();

            //Username and password that do not exist
            String username = "Christian1";
            String password = "wrongPassword1";
            try
            {
                //Attempting to log in with invalid log in details 3 times so as to be blocked

                //User is not blocked
                Assert.IsFalse(condition: uo.IsBlocked(username), message: "User is blocked");

                //First Attempt
                Assert.IsFalse(condition: uo.Login(username, password));
                //User is not blocked
                Assert.IsFalse(condition: uo.IsBlocked(username), message: "User is blocked");

                //Second Attempt
                Assert.IsFalse(condition: uo.Login(username, password));
                //User is not blocked
                Assert.IsFalse(condition: uo.IsBlocked(username), message: "User is blocked");

                //Third Attempt
                Assert.IsFalse(condition: uo.Login(username, password));
                //User is blocked after the third attempt
                Assert.IsTrue(condition: uo.IsBlocked(username), message: "User is not blocked");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestBlockedUsingDifferentUsernames()
        {
            //This test will check whether the user will be blocked after 3 invalid
            //login attempts using different usernames. (Testing attempts on the same IP Address)

            UsersOperations uo = new UsersOperations();

            //Username and password that do not exist
            String username1 = "Christian10";
            String username2 = "Christian20";
            String username3 = "Christian30";
            String password = "wrongPassword1";
            try
            {
                //Attempting to log in with invalid log in details 3 times so as to be blocked

                //First Attempt
                Assert.IsFalse(condition: uo.Login(username1, password));
                //User is not blocked
                Assert.IsFalse(condition: uo.IsBlocked(username1), message: "User is blocked");

                //Second Attempt
                Assert.IsFalse(condition: uo.Login(username2, password));
                //User is not blocked
                Assert.IsFalse(condition: uo.IsBlocked(username2), message: "User is blocked");

                //Third Attempt
                Assert.IsFalse(condition: uo.Login(username3, password));
                //Computer is blocked after the third attempt
                Assert.IsTrue(condition: uo.IsBlocked(username3), message: "User is not blocked");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
