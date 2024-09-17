namespace Looplex.DotNet.Middlewares.OAuth2.Domain;

/// <summary>
///     A label indicating the type of resource, e.g., 'User' or 'Group'.
/// </summary>
public enum GrantType
{
    TokenExchange,
    ClientCredentials
}