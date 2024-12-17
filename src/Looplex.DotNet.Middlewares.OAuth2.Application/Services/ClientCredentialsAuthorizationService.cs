using System.Net;
using System.Security.Claims;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Middlewares.ApiKeys.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Dtos;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.ExtensionMethods;
using Looplex.DotNet.Middlewares.OAuth2.Domain;
using Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;
using Looplex.OpenForExtension.Abstractions.Commands;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Looplex.OpenForExtension.Abstractions.ExtensionMethods;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Services;

public class ClientCredentialsAuthorizationService(
    IConfiguration configuration,
    IApiKeyService apiKeyService,
    IJwtService jwtService) : IAuthorizationService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IApiKeyService _apiKeyService = apiKeyService;
    private readonly IJwtService _jwtService = jwtService;

    public async Task CreateAccessToken(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        string authorization = context.GetRequiredValue<string>("Authorization");
        var json = context.GetRequiredValue<string>("Resource");
        var clientCredentialsDto = JsonConvert.DeserializeObject<ClientCredentialsDto>(json)!;
        await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);
        
        ArgumentNullException.ThrowIfNull(clientCredentialsDto, "body");

        ValidateAuthorizationHeader(authorization);
        ValidateGrantType(clientCredentialsDto.GrantType);
        var (clientId, clientSecret) = TryGetClientCredentials(authorization);
        var apiKey = await GetApiKeyByIdAndSecretOrDefaultAsync(clientId.ToString(), clientSecret, context, cancellationToken);
        await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);

        context.Roles["ApiKey"] = apiKey;
        await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

        if (!context.SkipDefaultAction)
        {
            var accessToken = CreateAccessToken((IApiKey)context.Roles["ApiKey"]);
            context.Result = new AccessTokenDto
            {
                AccessToken = accessToken
            };
        }

        await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
    }

    private static void ValidateAuthorizationHeader(string? authorization)
    {
        if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Basic "))
        {
            throw new HttpRequestException("Invalid authorization.", null, HttpStatusCode.Unauthorized);
        }
    }

    private static (Guid, string) TryGetClientCredentials(string? authorization)
    {
        if (authorization != default 
            && authorization.IsBasicAuthentication(out var base64Credentials)
            && base64Credentials != default)
        {
            return DecodeCredentials(base64Credentials);
        }
        
        throw new HttpRequestException("Invalid authorization.", null, HttpStatusCode.Unauthorized);
    }

    private static (Guid, string) DecodeCredentials(string credentials)
    {
        var parts = StringUtils.Base64Decode(credentials).Split(':');

        if (parts.Length != 2)
        {
            throw new HttpRequestException("Invalid credentials format.", null, HttpStatusCode.Unauthorized);
        }

        return (Guid.Parse(parts[0]), parts[1]);
    }

    private static void ValidateGrantType(string grantType)
    {
        if (!grantType
                .Equals(Constants.ClientCredentialsGrantType, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new HttpRequestException($"{Constants.GrantType} is invalid.", null, HttpStatusCode.Unauthorized);
        }
    }

    private Task<IApiKey> GetApiKeyByIdAndSecretOrDefaultAsync(string clientId, string clientSecret, IContext parentContext, CancellationToken cancellationToken)
    {
        var contextFactory = parentContext.Services.GetRequiredService<IContextFactory>();
        var context = contextFactory.Create(["AuthorizationService.ValidateClientCredentials"]);
            
        context.State.ParentContext = parentContext;
        context.State.ClientId = clientId;
        context.State.ClientSecret = clientSecret;
        
        return GetApiKeyByIdAndSecretOrDefaultAsync(context, cancellationToken);
    }

    private async Task<IApiKey> GetApiKeyByIdAndSecretOrDefaultAsync(IContext context, CancellationToken cancellationToken)
    {
        await _apiKeyService.GetByIdAndSecretOrDefaultAsync(context, cancellationToken);
        IApiKey? apiKey = default;
        if (context.Roles.TryGetValue("ApiKey", out var role))
        {
            apiKey = (IApiKey)role;
        }
        if (apiKey == default)
        {
            throw new EntityInvalidException(["Invalid clientId or clientSecret."]);
        }
        if (apiKey.NotBefore > DateTimeOffset.UtcNow)
        {
            throw new EntityInvalidException(["Client access not allowed."]);
        }
        if (apiKey.ExpirationTime <= DateTimeOffset.UtcNow)
        {
            throw new EntityInvalidException(["Client access is expired."]);
        }

        return apiKey;
    }

    private string CreateAccessToken(IApiKey apiKey)
    {
        var claims = new ClaimsIdentity([
            new Claim(Constants.ClientId, apiKey.ClientId.ToString()!),
        ]);

        var audience = _configuration["Audience"]!;
        var issuer = _configuration["Issuer"]!;
        var tokenExpirationTimeInMinutes = _configuration.GetValue<int>("TokenExpirationTimeInMinutes");

        var privateKey = StringUtils.Base64Decode(_configuration["PrivateKey"]!);
        
        var accessToken = _jwtService.GenerateToken(privateKey, issuer, audience, claims, TimeSpan.FromMinutes(tokenExpirationTimeInMinutes));
        return accessToken;
    }
}