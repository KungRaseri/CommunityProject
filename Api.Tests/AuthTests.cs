using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Api.Tests
{
    [TestClass, Ignore("We'll need to come back to this later, need to lern2code")]
    public class AuthTests
    {

        [TestInitialize]
        public void SetupTests()
        {
            Assert.Inconclusive("Not yet implemented");
        }

        [TestMethod]
        public void AuthController_Register_ReturnsUser()
        {
            Assert.Inconclusive("Not yet implemented");
        }

        [DataTestMethod]
        [DataRow("", "asjfhashfkajhsdf")]
        [DataRow("asdfjhakjsdfhkajshdf", "")]
        public void AuthController_Register_MissingParameters_ReturnsBadRequest(string email, string password)
        {
            Assert.Inconclusive("Not yet implemented");
        }

        [TestMethod]
        public void AuthController_Register_AccountExists_ReturnsBadRequest()
        {
            Assert.Inconclusive("Not yet implemented");
        }
    }
}
