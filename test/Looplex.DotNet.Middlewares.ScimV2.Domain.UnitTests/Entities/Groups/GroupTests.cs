using FluentAssertions;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Schemas;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.UnitTests.Entities.Groups;

[TestClass]
public class GroupTests
{
    [TestInitialize]
    public void Init()
    {
        if (!Schema.Schemas.ContainsKey(typeof(Group)))
            Schema.Add<Group>(File.ReadAllText("./Entities/Schemas/Group.1.0.schema.json"));
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
            Id = "group123",
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
                new MemberElement
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