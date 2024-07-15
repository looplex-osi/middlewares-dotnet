using System.ComponentModel.DataAnnotations;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Entities.Users
{
    [TestClass]
    public class NameTests
    {
        private Name _name = null!;

        [TestInitialize]
        public void Setup()
        {
            _name = new Name
            {
                FamilyName = "Doe",
                GivenName = "John",
                MiddleName = "Middle",
                HonorificPrefix = "Mr.",
                HonorificSuffix = "Jr."
            };
        }

        [TestMethod]
        public void Formatted_ShouldReturnCorrectFormat()
        {
            // Arrange
            var expectedFormattedName = "Mr. John Middle Doe, Jr.";

            // Act
            var actualFormattedName = _name.Formatted;

            // Assert
            Assert.AreEqual(expectedFormattedName, actualFormattedName);
        }

        [TestMethod]
        public void FamilyName_ShouldBeRequired()
        {
            // Arrange
            _name.FamilyName = string.Empty;
            var context = new ValidationContext(_name) { MemberName = "FamilyName" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(_name.FamilyName, context, results);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("StringCannotBeEmpty"), results[0].ErrorMessage);
        }

        [TestMethod]
        public void GivenName_ShouldBeRequired()
        {
            // Arrange
            _name.GivenName = string.Empty;
            var context = new ValidationContext(_name) { MemberName = "GivenName" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(_name.GivenName, context, results);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("StringCannotBeEmpty"), results[0].ErrorMessage);
        }

        [TestMethod]
        public void FamilyName_ShouldHaveMinLength()
        {
            // Arrange
            _name.FamilyName = "D";
            var context = new ValidationContext(_name) { MemberName = "FamilyName" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(_name.FamilyName, context, results);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("StringDoesNotHaveMinLength"), results[0].ErrorMessage);
        }

        [TestMethod]
        public void GivenName_ShouldHaveMinLength()
        {
            // Arrange
            _name.GivenName = "J";
            var context = new ValidationContext(_name) { MemberName = "GivenName" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(_name.GivenName, context, results);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("StringDoesNotHaveMinLength"), results[0].ErrorMessage);
        }
    }
}
