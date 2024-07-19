using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstraction.Services;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Services;

public class IdTokenService() : IIdTokenService
{
    public bool ValidateIdToken(string issuer, string tenantId, string audience, string token, out string? email)
    {
        bool isValid = false;
        try
        {
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{issuer}/{tenantId}/v2.0/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            var openIdConfig = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;

            var validationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = openIdConfig.SigningKeys,
                    ValidateIssuer = true,
                    ValidIssuer = $"{issuer}/{tenantId}/v2.0",
                    ValidateAudience = true,
                    ValidAudiences = [audience],
                    ValidateLifetime = true,
                    ValidateTokenReplay = true
                };

            JwtSecurityTokenHandler handler = new();
            var user = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                throw new InvalidOperationException("Email claim missing in IdToken");
            }
            isValid = true;
        }
        catch (Exception)
        {
            email = null;
        }
        return isValid;
    }
}