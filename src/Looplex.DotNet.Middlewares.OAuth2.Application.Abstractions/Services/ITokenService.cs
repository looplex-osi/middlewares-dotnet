namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;

public interface ITokenService
{
    bool ValidateToken(string issuer, string audience, string token);
}