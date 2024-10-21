using FluentAssertions;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.UnitTests.Entities.Groups;

[TestClass]
public class GroupTests
{
    [TestInitialize]
    public void Init()
    {
        if (!Schemas.ContainsKey(typeof(Group)))
            Schemas.Add(typeof(Group), File.ReadAllText("./Entities/Schemas/Group.1.0.schema.json"));
    }
        
    [TestMethod]
    [DataRow("id")]
    [DataRow("displayName")]
    public void Client_ShouldBeRequired(string field)
    {
        // Arrange
        var json = @"{
              
            }";
            
        // Act
        _ = Resource.FromJson<Group>(json, out var messages);
            
        // Assert
        Assert.IsFalse(messages.Count == 0);
        Assert.IsTrue(messages.Any(m => 
            m.IndexOf("required", StringComparison.InvariantCultureIgnoreCase) >= 0
            && m.IndexOf(field, StringComparison.Ordinal) >= 0));
    }

    [TestMethod]
    public void Group_ShouldBeValid()
    {
        var json = File.ReadAllText("./Entities/Groups/Mocks/ValidGroup.json");
        var expectedGroup = new Group
        {
            UniqueId = Guid.Parse("a33ba1e5-11e7-437a-821e-97bd1e6752a3"),
            ExternalId = "external456",
            Meta = new Meta
            {
                ResourceType = "Group",
                Created = DateTimeOffset.Parse("2023-07-17T12:34:56Z"),
                LastModified = DateTimeOffset.Parse("2024-07-17T12:34:56Z"),
                Location = new Uri("https://example.com/scim/v2/Groups/group123"),
                Version = "W/\"456\""
            },
            DisplayName = "Engineering Group",
            Members = new List<MemberElement>
            {
                new()
                {
                    Value = "user789",
                    Ref = new Uri("https://example.com/scim/v2/Users/user789"),
                    Type = GroupType.User
                }
            }
        };
            
        // Act
        var group = Resource.FromJson<Group>(json, out var messages);
            
        // Assert
        Assert.IsTrue(messages.Count == 0);
        group.Should().BeEquivalentTo(expectedGroup);
    }
}