namespace Looplex.DotNet.Middlewares.OAuth2.Domain.ExtensionMethods;

public static class GrantTypeExtensionMethods
{
    public static GrantType ToGrantType(this string grantType)
    {
        return grantType switch
        {
            Constants.TokenExchangeGrantType => GrantType.TokenExchange,
            Constants.ClientCredentialsGrantType => GrantType.ClientCredentials,
            _ => throw new ArgumentException("Invalid value", nameof(grantType))
        };
    }
}