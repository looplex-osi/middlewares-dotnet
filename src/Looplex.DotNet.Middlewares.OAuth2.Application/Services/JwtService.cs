﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Microsoft.IdentityModel.Tokens;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Services;

public sealed class JwtService : IJwtService
{
    public string GenerateToken(
        string privateKey,
        string issuer,
        string audience,
        ClaimsIdentity claimsIdentity,
        TimeSpan expiration)
    {
        using var privateKeyRsa = RSA.Create();
        privateKeyRsa.ImportFromPem(privateKey);

        var tokenHandler = new JwtSecurityTokenHandler();

        var creds = new SigningCredentials(new RsaSecurityKey(privateKeyRsa), SecurityAlgorithms.RsaSha256)
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

    public string? GetUserIdFromToken(string accessToken)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(accessToken);

        var claim = decodedToken.Claims.FirstOrDefault(x => x.Type == "email");

        if (claim != null)
        {
            return claim.Value;
        }

        return null;
    }

    public bool ValidateToken(
        string publicKey,
        string issuer,
        string audience,
        string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        using var publicKeyRsa = RSA.Create();
        try
        {
            publicKeyRsa.ImportFromPem(publicKey);
        }
        catch (Exception)
        {
            return false;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(publicKeyRsa)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            },
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
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