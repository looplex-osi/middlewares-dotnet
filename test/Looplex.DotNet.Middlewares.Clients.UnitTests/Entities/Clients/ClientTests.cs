using FluentAssertions;
using Looplex.DotNet.Middlewares.Clients.Entities.Clients;
using Looplex.DotNet.Middlewares.ScimV2.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Schemas;

namespace WebApplication1Looplex.DotNet.Middlewares.Clients.UnitTests.Entities.Clients
{
    [TestClass]
    public class ClientTests
    {
        [TestInitialize]
        public void Init()
        {
            if (!Schema.Schemas.ContainsKey(typeof(Client)))
                Schema.Add<Client>(File.ReadAllText("./Entities/Schemas/Client.1.0.schema.json"));
        }

        [TestMethod]
        [DataRow("id")]
        [DataRow("displayName")]
        [DataRow("secret")]
        [DataRow("expirationTime")]
        [DataRow("notBefore")]
        public void Client_ShouldBeRequired(string field)
        {
            // Arrange
            var json = @"{
              
            }";
            
            // Act
            var client = Resource.FromJson<Client>(json, out var messages);
            
            // Assert
            Assert.IsFalse(messages.Count == 0);
            Assert.IsTrue(messages.Any(m => 
                m.IndexOf("required", StringComparison.InvariantCultureIgnoreCase) >= 0
                && m.IndexOf(field, StringComparison.Ordinal) >= 0));
        }

        [TestMethod]
        public void Client_ShouldBeValid()
        {
            var json = File.ReadAllText("./Entities/Clients/Mocks/ValidClient.json");
            var expectedClient = new Client
            {
                Id = "client123",
                ExternalId = "external456",
                Meta = new Meta
                {
                    ResourceType = "Client",
                    Created = DateTimeOffset.Parse("2023-07-17T12:34:56Z"),
                    LastModified = DateTimeOffset.Parse("2024-07-17T12:34:56Z"),
                    Location = new Uri("https://example.com/scim/v2/Clients/client123"),
                    Version = "W/\"456\""
                },
                DisplayName = "OAuth Client",
                Secret = "supersecret",
                ExpirationTime = DateTimeOffset.Parse("2025-07-17T12:34:56Z"),
                NotBefore = DateTimeOffset.Parse("2023-07-17T12:34:56Z")
            };
            
            // Act
            var client = Resource.FromJson<Client>(json, out var messages);
            
            // Assert
            Assert.IsTrue(messages.Count == 0);
            client.Should().BeEquivalentTo(expectedClient);
        }
    }
}