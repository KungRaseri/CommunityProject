using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Web.Tests.Pages;

namespace Web.Tests
{
    [TestClass]
    public class AuthTests
    {
        private AuthPage _authPage;

        [TestInitialize]
        public void SetUp()
        {
        }

        [TestMethod]
        public void Login_WithValidCredentials_Success()
        {
        }

        [TestMethod]
        public void Login_WithInvalidCredentials_Fail()
        {
        }
    }
}
