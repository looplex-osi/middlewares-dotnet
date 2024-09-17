namespace Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;

public interface IApiKey
{
    string? ClientName { get; }
    Guid? ClientId { get; }
    string? Digest { get; }
    int? UserId { get; }
    DateTimeOffset ExpirationTime { get; }
    DateTimeOffset NotBefore { get; }
}