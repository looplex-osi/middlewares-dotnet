using System.ComponentModel.DataAnnotations;
using Looplex.DotNet.Middlewares.ScimV2.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Users;
using Microsoft.Extensions.Localization;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Entities.Users
{
    [TestClass]
    public class UserTests
    {
        private IStringLocalizer<User> _localizer;

        [TestInitialize]
        public void Setup()
        {
            _localizer = Substitute.For<IStringLocalizer<User>>();
        }

        [TestMethod]
        public void UserName_ShouldBeRequired()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = ((string?)null)!,
                Name = Substitute.For<Name>()
            };
            var results = ValidateProperty(user, "UserName");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("PropertyIsRequired"), results[0].ErrorMessage);
        }
        
        private IList<ValidationResult> ValidateProperty(object model, string propertyName)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = propertyName };
            Validator.TryValidateProperty(model.GetType()!.GetProperty(propertyName)!.GetValue(model, null), context, validationResults);
            return validationResults;
        }

        [TestMethod]
        public void UserName_ShouldHaveMinLength()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "short",
                Name = Substitute.For<Name>()
            };
            var results = ValidateProperty(user, "UserName");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("StringDoesNotHaveMinLength"), results[0].ErrorMessage);
        }
        
        [TestMethod]
        public void UserName_Valid()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "jonhdoebr",
                Name = Substitute.For<Name>()
            };
            var results = ValidateProperty(user, "UserName");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void Name_ShouldBeRequired()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = null!
            };
            var results = ValidateProperty(user, "Name");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("PropertyIsRequired"), results[0].ErrorMessage);
        }
        
        [TestMethod]
        public void Name_Valid()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = new Name
                {
                    FamilyName = "Doe",
                    GivenName = "John",
                    MiddleName = "Middle",
                    HonorificPrefix = "Mr.",
                    HonorificSuffix = "Jr."
                }
            };
            var results = ValidateProperty(user, "Name");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void DisplayName_CannotBeEmpty()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                DisplayName = ""
            };
            var results = ValidateProperty(user, "DisplayName");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("StringCannotBeEmpty"), results[0].ErrorMessage);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("jonh d")]
        public void DisplayName_Valid(string? value)
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                DisplayName = value
            };
            var results = ValidateProperty(user, "DisplayName");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void NickName_CannotBeEmpty()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                NickName = ""
            };
            var results = ValidateProperty(user, "NickName");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("StringCannotBeEmpty"), results[0].ErrorMessage);
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("jonny")]
        public void NickName_Valid(string? value)
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                NickName = value
            };
            var results = ValidateProperty(user, "NickName");
            Assert.IsTrue(results.Count == 0);
        }
        
        [TestMethod]
        public void ProfileUrl_ShouldBeValidUrl()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                ProfileUrl = "invalid-url" 
            };
            var results = ValidateProperty(user, "ProfileUrl");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("PropertyIsInvalid"), results[0].ErrorMessage);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("https://jonghdoe.profile.com")]
        public void ProfileUrl_Valid(string? value)
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                ProfileUrl = value
            };
            var results = ValidateProperty(user, "ProfileUrl");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void Title_CannotBeEmpty()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                Title = ""
            };
            var results = ValidateProperty(user, "Title");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("StringCannotBeEmpty"), results[0].ErrorMessage);
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("Mr.")]
        public void Title_Valid(string? value)
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                Title = value
            };            
            var results = ValidateProperty(user, "Title");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void UserType_CannotBeEmpty()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                UserType = ""
            };
            var results = ValidateProperty(user, "UserType");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("StringCannotBeEmpty"), results[0].ErrorMessage);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("admin")]
        public void UserType_Valid(string? value)
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                UserType = value
            };
            var results = ValidateProperty(user, "UserType");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void PreferredLanguage_ShouldBeValid()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                PreferredLanguage = "invalid"
            };
            var results = ValidateProperty(user, "PreferredLanguage");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("PropertyIsInvalid"), results[0].ErrorMessage);
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("da, en-gb;q=0.8, en;q=0.7")]
        public void PreferredLanguage_Valid(string? value)
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                PreferredLanguage = value
            };
            var results = ValidateProperty(user, "PreferredLanguage");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void Locale_ShouldBeValid()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                Locale = "invalid"
            };            
            var results = ValidateProperty(user, "Locale");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("PropertyIsInvalid"), results[0].ErrorMessage);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("en-US, pt-BR, de")]
        public void Locale_Valid(string? value)
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                Locale = value
            };            
            var results = ValidateProperty(user, "Locale");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void Timezone_ShouldBeValid()
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                Timezone = "invalid"
            };               
            var results = ValidateProperty(user, "Timezone");
            Assert.IsFalse(results.Count == 0);
            Assert.AreEqual(Resources.ScimV2.Common.ResourceManager.GetString("PropertyIsInvalid"), results[0].ErrorMessage);
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("America/Sao_Paulo")]
        [DataRow("Asia/Shanghai")]
        public void Timezone_Valid(string? value)
        {
            var user = new User(_localizer)
            {
                Id = "",
                Meta = Substitute.For<Meta>(),
                UserName = "",
                Name = Substitute.For<Name>(),
                Timezone = value
            };               
            var results = ValidateProperty(user, "Timezone");
            Assert.IsTrue(results.Count == 0);
        }
    }
}