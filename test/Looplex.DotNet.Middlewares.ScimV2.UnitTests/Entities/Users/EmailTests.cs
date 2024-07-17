using System.ComponentModel.DataAnnotations;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Entities.Users
{
    [TestClass]
    public class EmailTests
    {
        [TestMethod]
        public void Value_ShouldBeRequired()
        {
            var email = new Email { Value = null };
            var results = ValidateProperty(email, "Value");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual("The Value field is required.", results[0].ErrorMessage);
        }

        [TestMethod]
        public void Value_ShouldBeValidEmailAddress()
        {
            var email = new Email { Value = "invalid-email" };
            var results = ValidateProperty(email, "Value");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual("Invalid email address.", results[0].ErrorMessage);
        }

        [TestMethod]
        public void Value_ShouldBeValid()
        {
            var email = new Email { Value = "test@example.com" };
            var results = ValidateProperty(email, "Value");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void Type_CanBeNull()
        {
            var email = new Email { Value = "test@example.com", Type = null };
            var results = ValidateProperty(email, "Type");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void Type_ShouldBeValid()
        {
            var email = new Email { Value = "test@example.com", Type = "work" };
            var results = ValidateProperty(email, "Type");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void Type_ShouldBeInvalid()
        {
            var email = new Email { Value = "test@example.com", Type = "invalid" };
            var results = ValidateProperty(email, "Type");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual("Type must be either 'work', 'home', or 'other'.", results[0].ErrorMessage);
        }

        private IList<ValidationResult> ValidateProperty(object model, string propertyName)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = propertyName };
            Validator.TryValidateProperty(model.GetType()!.GetProperty(propertyName)!.GetValue(model, null), context, validationResults);
            return validationResults;
        }
    }
}
