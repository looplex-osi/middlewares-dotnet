namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

/// <summary>
///    The authentication scheme.  This specification defines the values
/// "oauth", "oauth2", "oauthbearertoken", "httpbasic", and "httpdigest".
/// </summary>
public enum AuthenticationSchemeType
{
    OAuth,
    OAuth2,
    OAuthBearerToken,
    HttpBasic,
    HttpDigest
}