namespace Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;

public interface IClient
{
    string DisplayName { get; }
    Guid? UniqueId { get; }
    string Secret { get; }
    DateTimeOffset ExpirationTime { get; }
    DateTimeOffset NotBefore { get; }
}