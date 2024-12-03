using FluentAssertions;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.UnitTests.Entities.Users;

[TestClass]
public class UserTests
{
    [TestMethod]
    [DataRow("id")]
    [DataRow("userName")]
    public void Client_ShouldBeRequired(string field)
    {
        // Arrange
        var json = @"{
              
            }";
        var jsonSchema = File.ReadAllText("./Entities/Schemas/User.1.0.schema.json");

        // Act
        _ = Resource.FromJson<User>(json, jsonSchema, out var messages);
            
        // Assert
        Assert.IsFalse(messages.Count == 0);
        Assert.IsTrue(messages.Any(m => 
            m.IndexOf("required", StringComparison.InvariantCultureIgnoreCase) >= 0
            && m.IndexOf(field, StringComparison.Ordinal) >= 0));
    }

    [TestMethod]
    public void User_ShouldBeValid()
    {
        var json = File.ReadAllText("./Entities/Users/Mocks/ValidUser.json");
        var expectedUser = new User
        {
            UniqueId = Guid.Parse("862903bb-de5a-4a24-b191-8405e6e99c33"),
            UserName = "johndoe",
            ExternalId = "78910",
            Meta = new Meta
            {
                ResourceType = "User",
                Created = DateTimeOffset.Parse("2023-07-17T12:34:56Z"),
                LastModified = DateTimeOffset.Parse("2024-07-17T12:34:56Z"),
                Location = new Uri("https://example.com/scim/v2/Users/123456"),
                Version = "W/\"123\""
            },
            Name = new Name
            {
                Formatted = "Mr. John Doe",
                FamilyName = "Doe",
                GivenName = "John",
                MiddleName = "Middle",
                HonorificPrefix = "Mr.",
                HonorificSuffix = "Jr."
            },
            DisplayName = "John Doe",
            NickName = "Johnny",
            ProfileUrl = "https://example.com/users/johndoe",
            Title = "Software Engineer",
            UserType = "Employee",
            PreferredLanguage = "en_US",
            Locale = "en_US",
            Timezone = "America/Los_Angeles",
            Active = true,
            Password = "securepassword",
            Emails =
            [
                new EmailElement
                {
                    Value = "johndoe@example.com",
                    Type = EmailType.Work,
                    Primary = true
                }
            ],
            PhoneNumbers = new List<PhoneNumberElement>
            {
                new PhoneNumberElement
                {
                    Value = "+1-555-555-5555",
                    Type = PhoneNumberType.Mobile,
                    Primary = true
                }
            },
            Ims =
            [
                new ImElement
                {
                    Value = "john.doe",
                    Type = "gtalk",
                    Primary = true
                }
            ],
            Photos =
            [
                new PhotoElement
                {
                    Value = new Uri("https://example.com/photos/johndoe.jpg"),
                    Type = PhotoType.Photo,
                    Primary = true
                }
            ],
            Addresses =
            [
                new AddressElement
                {
                    Formatted = "123 Main St, Anytown, CA 12345, USA",
                    StreetAddress = "123 Main St",
                    Locality = "Anytown",
                    Region = "CA",
                    PostalCode = "12345",
                    Country = "US",
                    Type = AddressType.Work
                }
            ],
            Groups =
            [
                new GroupElement
                {
                    Value = "group1",
                    Ref = new Uri("https://example.com/scim/v2/Groups/group1"),
                    Type = GroupType.Direct
                }
            ],
            Entitlements =
            [
                new EntitlementElement
                {
                    Value = "entitlement1",
                    Type = "type1",
                    Primary = true
                }
            ],
            Roles =
            [
                new RoleElement
                {
                    Value = "role1",
                    Type = "type1",
                    Primary = true
                }
            ]
        };
        var jsonSchema = File.ReadAllText("./Entities/Schemas/User.1.0.schema.json");

        // Act
        var user = Resource.FromJson<User>(json, jsonSchema, out var messages);
            
        // Assert
        Assert.IsTrue(messages.Count == 0);
        user.Should().BeEquivalentTo(expectedUser);
    }
}