using System.IdentityModel.Tokens.Jwt;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Services;

public class TokenService() : ITokenService
{
    public bool ValidateToken(string issuer, string audience, string token)
    {
        bool isValid;
        try
        {
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{issuer}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever());
            var openIdConfig = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;

            var validationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = openIdConfig.SigningKeys,
                    ValidateIssuer = true,
                    ValidIssuer = $"{issuer}",
                    ValidateAudience = true,
                    ValidAudiences = [audience],
                    ValidateLifetime = true,
                    ValidateTokenReplay = true
                };

            JwtSecurityTokenHandler handler = new();
            handler.ValidateToken(token, validationParameters, out _);
            isValid = true;
        }
        catch
        {
            isValid = false;
        }
        return isValid;
    }
}