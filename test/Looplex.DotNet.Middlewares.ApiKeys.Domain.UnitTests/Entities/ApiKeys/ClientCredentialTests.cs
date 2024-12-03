using FluentAssertions;
using Looplex.DotNet.Middlewares.ApiKeys.Domain.Entities.ClientCredentials;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;

namespace Looplex.DotNet.Middlewares.ApiKeys.Domain.UnitTests.Entities.ApiKeys;

[TestClass]
public class ClientCredentialTests
{
    [TestMethod]
    [DataRow("clientName")]
    [DataRow("expirationTime")]
    [DataRow("notBefore")]
    public void Client_ShouldBeRequired(string field)
    {
        // Arrange
        var json = @"{
              
            }";
        var jsonSchema = File.ReadAllText("./Entities/Schemas/ClientCredential.1.0.schema.json");

        // Act
        var client = Resource.FromJson<ClientCredential>(json, jsonSchema, out var messages);
            
        // Assert
        Assert.IsFalse(messages.Count == 0);
        Assert.IsTrue(messages.Any(m => 
            m.IndexOf("required", StringComparison.InvariantCultureIgnoreCase) >= 0
            && m.IndexOf(field, StringComparison.Ordinal) >= 0));
    }

    [TestMethod]
    public void Client_ShouldBeValid()
    {
        var json = File.ReadAllText("./Entities/ApiKeys/Mocks/ValidApiKey.json");
        var expectedClient = new ClientCredential
        {
            ExternalId = "external456",
            ClientName = "OAuth Client",
            ExpirationTime = DateTimeOffset.Parse("2025-07-17T12:34:56Z"),
            NotBefore = DateTimeOffset.Parse("2023-07-17T12:34:56Z")
        };
        var jsonSchema = File.ReadAllText("./Entities/Schemas/ClientCredential.1.0.schema.json");

        // Act
        var client = Resource.FromJson<ClientCredential>(json, jsonSchema, out var messages);
            
        // Assert
        Assert.IsTrue(messages.Count == 0);
        client.Should().BeEquivalentTo(expectedClient);
    }
}