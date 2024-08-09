using System.Net;
using System.Security.Claims;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Middlewares.Clients.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Dtos;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;
using Looplex.OpenForExtension.Abstractions.Commands;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Looplex.OpenForExtension.Abstractions.ExtensionMethods;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Services;

public class AuthorizationService(
    IConfiguration configuration,
    IClientService clientService,
    IIdTokenService idTokenService,
    IJwtService jwtService) : IAuthorizationService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IClientService _clientService = clientService;
    private readonly IIdTokenService _idTokenService = idTokenService;
    private readonly IJwtService _jwtService = jwtService;
    
    public async Task CreateAccessToken(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        string authorization = context.GetRequiredValue<string>("Authorization");
        var json = context.GetRequiredValue<string>("Resource");
        var clientCredentialsDto = JsonConvert.DeserializeObject<ClientCredentialsDto>(json);
        context.Plugins.Execute<IHandleInput>(context, cancellationToken);
        
        ArgumentNullException.ThrowIfNull(clientCredentialsDto, "body");
        ValidateAuthorizationHeader(authorization);
        ValidateGrantType(clientCredentialsDto);
        var email = ValidateIdToken(clientCredentialsDto);
        await ValidateClientCredentials(authorization![7..], context, cancellationToken);
        context.Plugins.Execute<IValidateInput>(context, cancellationToken);

        context.Roles["ClientCredentials"] = clientCredentialsDto;
        context.Plugins.Execute<IDefineRoles>(context, cancellationToken);

        context.Plugins.Execute<IBind>(context, cancellationToken);

        context.Plugins.Execute<IBeforeAction>(context, cancellationToken);

        if (!context.SkipDefaultAction)
        {
            var accessToken = CreateAccessToken(email!);
            context.Result = new AccessTokenDto
            {
                AccessToken = accessToken
            };
        }

        context.Plugins.Execute<IAfterAction>(context, cancellationToken);

        context.Plugins.Execute<IReleaseUnmanagedResources>(context, cancellationToken);
    }

    private static void ValidateAuthorizationHeader(string? authorization)
    {
        if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
        {
            throw new HttpRequestException("Invalid authorization.", null, HttpStatusCode.Unauthorized);
        }
    }

    private static void ValidateGrantType(ClientCredentialsDto clientCredentialsDto)
    {
        if (clientCredentialsDto.GrantType != "client_credentials")
        {
            throw new HttpRequestException("grant_type is invalid.", null, HttpStatusCode.Unauthorized);
        }
    }

    private string ValidateIdToken(ClientCredentialsDto clientCredentialsDto)
    {
        var oicdAudience = _configuration["OicdAudience"]!;
        var oicdIssuer = _configuration["OicdIssuer"]!;
        var oicdTenantId = _configuration["OicdTenantId"]!;

        if (!_idTokenService.ValidateIdToken(oicdIssuer, oicdTenantId, oicdAudience, clientCredentialsDto.IdToken, out string? email))
        {
            throw new HttpRequestException("IdToken is invalid.", null, HttpStatusCode.Unauthorized);
        }
        return email!;
    }

    private async Task ValidateClientCredentials(string credentials, IContext parentContext, CancellationToken cancellationToken)
    {
        var (clientId, clientSecret) = DecodeCredentials(credentials);

        var clientCredentials = new { ClientId = clientId, ClientSecret = clientSecret};
        if (!IsAdmin(clientId, clientSecret))
        {
            var contextFactory = parentContext.Services.GetRequiredService<IContextFactory>();
            var context = contextFactory.Create(["AuthorizationService.ValidateClientCredentials"]);
            
            context.State.ParentContext = parentContext;
            context.State.ClientId = clientId;
            context.State.ClientSecret = clientSecret;
                
            await ValidateClientCredentialsDefaultAction(context, cancellationToken);
        }
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

    private bool IsAdmin(Guid clientId, string clientSecret)
    {
        var isAdmin = false;
        var adminClientId = Guid.Parse(_configuration["AdminClientId"]!);
        var adminClientSecret = _configuration["AdminClientSecret"]!;

        if (clientId == adminClientId
            && clientSecret == adminClientSecret)
        {
            isAdmin = true;
        }

        return isAdmin;
    }

    private async Task ValidateClientCredentialsDefaultAction(IContext context, CancellationToken cancellationToken)
    {
        await _clientService.GetByIdAndSecretOrDefaultAsync(context, cancellationToken);
        var client = (IClient?) context.Result;
                
        if (client == default)
        {
            throw new EntityInvalidException(["Invalid clientId or clientSecret."]);
        }
        if (client.NotBefore > DateTimeOffset.UtcNow)
        {
            throw new EntityInvalidException(["Client access not allowed."]);
        }
        if (client.ExpirationTime <= DateTimeOffset.UtcNow)
        {
            throw new EntityInvalidException(["Client access is expired."]);
        }
    }

    private string CreateAccessToken(string email)
    {
        var claims = new ClaimsIdentity([
            new Claim(ClaimTypes.Email, email!),
        ]);

        var audience = _configuration["Audience"]!;
        var issuer = _configuration["Issuer"]!;
        var tokenExpirationTimeInMinutes = _configuration.GetValue<int>("TokenExpirationTimeInMinutes");

        var privateKey = StringUtils.Base64Decode(configuration["PrivateKey"]!);
        
        var accessToken = _jwtService.GenerateToken(privateKey, issuer, audience, claims, TimeSpan.FromMinutes(tokenExpirationTimeInMinutes));
        return accessToken;
    }
}