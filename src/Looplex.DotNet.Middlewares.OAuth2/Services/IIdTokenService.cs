namespace Looplex.DotNet.Middlewares.OAuth2.Services
{
    public interface IIdTokenService
    {
        bool ValidateIdToken(string issuer, string tenantId, string audience, string token, out string? email);
    }
}