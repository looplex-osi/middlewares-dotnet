using System.Security.Claims;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;

public interface IJwtService
{
    string GenerateToken(string privateKey, string issuer, string audience, ClaimsIdentity claimsIdentity, TimeSpan expiration);
    bool ValidateToken(string publicKey, string issuer, string audience, string token);
}