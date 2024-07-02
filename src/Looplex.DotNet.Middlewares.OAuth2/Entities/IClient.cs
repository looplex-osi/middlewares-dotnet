namespace Looplex.DotNet.Middlewares.OAuth2.Entities
{
    public interface IClient
    {
        string DisplayName { get; }
        string Id { get; }
        string Secret { get; }
        DateTime ExpirationTime { get; }
        DateTime NotBefore { get; }
    }
}
