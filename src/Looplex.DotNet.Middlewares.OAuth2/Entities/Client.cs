namespace Looplex.DotNet.Middlewares.OAuth2.Entities
{
    public class Client : IClient
    {
        public required string DisplayName { get; init; }
        public required string ClientId { get; init; }
        public required string Secret { get; init; }
        public required DateTime ExpirationTime { get; init; }
        public required DateTime NotBefore { get; init; }
    }
}
