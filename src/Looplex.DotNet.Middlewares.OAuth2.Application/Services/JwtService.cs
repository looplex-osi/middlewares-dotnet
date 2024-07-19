using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Services;

public sealed class JwtService : IDisposable
{
    private readonly RSA _privateKey;
    private readonly RSA _publicKey;

    public JwtService(string privateKey, string publicKey)
    {
        _publicKey = RSA.Create();
        _privateKey = RSA.Create();
        _publicKey.ImportFromPem(publicKey);
        _privateKey.ImportFromPem(privateKey);
    }

    public void Dispose()
    {
        _privateKey.Dispose();
        _publicKey.Dispose();
    }

    public string GenerateToken(string issuer, string audience, ClaimsIdentity claimsIdentity, TimeSpan expiration)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var creds = new SigningCredentials(new RsaSecurityKey(_privateKey), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.Add(expiration),
            SigningCredentials = creds
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string issuer, string audience, string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(_publicKey)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            },            
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,                
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}