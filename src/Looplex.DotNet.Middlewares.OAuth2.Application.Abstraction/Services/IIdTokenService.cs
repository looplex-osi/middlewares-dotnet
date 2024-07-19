namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstraction.Services;

public interface IIdTokenService
{
    bool ValidateIdToken(string issuer, string tenantId, string audience, string token, out string? email);
}