using System.Dynamic;
using System.Net;
using System.Text;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ApiKeys.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using NSubstitute;
using Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Microsoft.Extensions.Configuration;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Microsoft.AspNetCore.Http;
using Casbin;
using System.IO;
using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.ExtensionMethods;

[TestClass]
public class RoutesExtensionMethodsTests
{
    private IConfiguration _configurationMock = null!;
    private ICrudService _crudServiceMock = null!;
    private IApiKeyService _apiKeyServiceMock = null!;
    private IContextFactory _contextFactoryMock = null!;
    private IServiceProvider _serviceProviderMock = null!;
    private IJwtService _jwtServiceMock = null!;
    private IHttpClientFactory _httpClientFactoryMock = null!;
    private ISchemaService _schemaServiceMock = null!;
    private IResourceTypeService _resourceTypeServiceMock = null!;
    private ServiceProviderConfiguration _serviceProviderConfigurationMock = null!;
    private IScimV2Context _context = null!;
    private HttpClient _client = null!;
    private IHost _host = null!;
    private IRbacService _rbacService = null!;
    private IEnforcer _enforcer = null!;

    [TestInitialize]
    public void Initialize()
    {
        _rbacService = Substitute.For<IRbacService>();
        string directory = Directory.GetCurrentDirectory();
        _enforcer = Substitute.For<IEnforcer>();
        _rbacService.CheckPermissionAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(true));
        _configurationMock = Substitute.For<IConfiguration>();
        _configurationMock["Audience"].Returns("testAudience");
        _configurationMock["Issuer"].Returns("testIssuer");
        _crudServiceMock = Substitute.For<ICrudService>();
        _apiKeyServiceMock = Substitute.For<IApiKeyService>();
        _contextFactoryMock = Substitute.For<IContextFactory>();
        _serviceProviderMock = Substitute.For<IServiceProvider>();
        _jwtServiceMock = Substitute.For<IJwtService>();
        _jwtServiceMock
            .ValidateToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);
        _jwtServiceMock
            .GetUserIdFromToken(Arg.Any<string>())
            .Returns("testUserId");
        _httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
        _serviceProviderMock.GetService(typeof(IRbacService)).Returns(_rbacService);
        _serviceProviderMock.GetService(typeof(IConfiguration)).Returns(_configurationMock);
        _serviceProviderMock.GetService(typeof(IJwtService)).Returns(_jwtServiceMock);  
        _schemaServiceMock = Substitute.For<ISchemaService>();
        _resourceTypeServiceMock = Substitute.For<IResourceTypeService>();
        _context = Substitute.For<IScimV2Context>();
        var state = new ExpandoObject();
        _context.State.Returns(state);
        _context.Result.Returns("mock_result");
        _context.Services.Returns(_serviceProviderMock);
        _context.GetDomain().Returns("testDomain");
        var routeValues = new Dictionary<string, object?>();
        _context.RouteValues.Returns(routeValues);
        _contextFactoryMock.Create(Arg.Any<IEnumerable<string>>()).Returns(_context);
        _serviceProviderConfigurationMock = Substitute.For<ServiceProviderConfiguration>();
        _host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton(_configurationMock);
                        services.AddSingleton(_crudServiceMock);
                        services.AddSingleton(_apiKeyServiceMock);
                        services.AddSingleton(_contextFactoryMock);
                        services.AddSingleton(_httpClientFactoryMock);
                        services.AddSingleton(_configurationMock);
                        services.AddSingleton(_schemaServiceMock);
                        services.AddSingleton(_resourceTypeServiceMock);
                        services.AddSingleton(_serviceProviderConfigurationMock);

                        services.AddOAuth2Services(_enforcer);
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.UseScimV2RoutesAsync<Car, ICrudService>(
                                "domains/{domainId}/brands/{brandId}/cars",
                                "example/car.schema.json",
                                new ScimV2RouteOptions(),
                                CancellationToken.None).GetAwaiter().GetResult();
                        });
                    });
            })
            .Start();

        _client = _host.GetTestClient();
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _client.Dispose();
        _host.Dispose();
    }

    [TestMethod]
    public async Task Get_Endpoint_Returns_Mock_Message()
    {
        // Arrange
        _crudServiceMock.When(x => x.GetAllAsync(_context, Arg.Any<CancellationToken>()))
            .Do(_ =>
            {
                _context.State.Pagination.TotalCount = 0;
            });
        var url = "domains/admin/brands/ferrari/cars?startIndex=1&count=10&param1=1&param2=2"; 
        HttpContent content = new StringContent("resourceContent", Encoding.UTF8, "application/text");
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Content = content;
        request.Headers.Add("Header-1", "11");
        request.Headers.Add("Header-2", "22");
        request.Headers.Add("Authorization", "Bearer token");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(responseString.Contains("mock_result"));
        Assert.AreEqual("admin", _context.RouteValues["domainId"]);
        Assert.AreEqual("ferrari", _context.RouteValues["brandId"]);
        Assert.AreEqual("1", _context.Query["param1"]);
        Assert.AreEqual("2", _context.Query["param2"]);
        Assert.AreEqual("1", _context.Query["startIndex"]);
        Assert.AreEqual("10", _context.Query["count"]);
        Assert.AreEqual("11", _context.Headers["Header-1"]);
        Assert.AreEqual("22", _context.Headers["Header-2"]);
        Assert.AreEqual(1, _context.State.Pagination.StartIndex);
        Assert.AreEqual(10, _context.State.Pagination.ItemsPerPage);
    }
    
    [TestMethod]
    public async Task GetById_Endpoint_Returns_Mock_Message()
    {
        // Arrange
        var url = "domains/admin/brands/ferrari/cars/id_car?param1=1&param2=2"; 
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Header-1", "11");
        request.Headers.Add("Header-2", "22");
        request.Headers.Add("Authorization", "Bearer token");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(responseString.Contains("mock_result"));
        Assert.AreEqual("admin", _context.RouteValues["domainId"]);
        Assert.AreEqual("ferrari", _context.RouteValues["brandId"]);
        Assert.AreEqual("id_car", _context.RouteValues["carId"]);
        Assert.AreEqual("1", _context.Query["param1"]);
        Assert.AreEqual("2", _context.Query["param2"]);
        Assert.AreEqual("11", _context.Headers["Header-1"]);
        Assert.AreEqual("22", _context.Headers["Header-2"]);
        await _crudServiceMock.Received(1).GetByIdAsync(_context, Arg.Any<CancellationToken>());
    }
    
    [TestMethod]
    public async Task Post_Endpoint_Returns_Mock_Message()
    {
        // Arrange
        var url = "domains/admin/brands/ferrari/cars?param1=1&param2=2"; 
        HttpContent content = new StringContent("resourceContent", Encoding.UTF8, "application/text");
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = content;
        request.Headers.Add("Header-1", "11");
        request.Headers.Add("Header-2", "22");
        request.Headers.Add("Authorization", "Bearer token");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.AreEqual("resourceContent", _context.State.Resource);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(string.Empty, responseString);
        Assert.AreEqual("/domains/admin/brands/ferrari/cars/mock_result", response.Headers.Location!.ToString());
        Assert.AreEqual("admin", _context.RouteValues["domainId"]);
        Assert.AreEqual("ferrari", _context.RouteValues["brandId"]);
        Assert.AreEqual("1", _context.Query["param1"]);
        Assert.AreEqual("2", _context.Query["param2"]);
        Assert.AreEqual("11", _context.Headers["Header-1"]);
        Assert.AreEqual("22", _context.Headers["Header-2"]);
        await _crudServiceMock.Received(1).CreateAsync(_context, Arg.Any<CancellationToken>());
    }
    
    [TestMethod]
    public async Task Put_Endpoint_Returns_Mock_Message()
    {
        // Arrange
        var url = "domains/admin/brands/ferrari/cars/id_car?param1=1&param2=2"; 
        HttpContent content = new StringContent("resourceContent", Encoding.UTF8, "application/text");
        using var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Content = content;
        request.Headers.Add("Header-1", "11");
        request.Headers.Add("Header-2", "22");
        request.Headers.Add("Authorization", "Bearer token");

        // Act
        var response = await _client.SendAsync(request);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(string.Empty, responseString);
        Assert.AreEqual("admin", _context.RouteValues["domainId"]);
        Assert.AreEqual("ferrari", _context.RouteValues["brandId"]);
        Assert.AreEqual("id_car", _context.RouteValues["carId"]);
        Assert.AreEqual("1", _context.Query["param1"]);
        Assert.AreEqual("2", _context.Query["param2"]);
        Assert.AreEqual("11", _context.Headers["Header-1"]);
        Assert.AreEqual("22", _context.Headers["Header-2"]);
        await _crudServiceMock.Received(1).UpdateAsync(_context, Arg.Any<CancellationToken>());
    }
    
    [TestMethod]
    public async Task Patch_Endpoint_Returns_Mock_Message()
    {
        // Arrange
        var url = "domains/admin/brands/ferrari/cars/id_car?param1=1&param2=2"; 
        HttpContent content = new StringContent("operationsContent", Encoding.UTF8, "application/text");
        using var request = new HttpRequestMessage(HttpMethod.Patch, url);
        request.Content = content;
        request.Headers.Add("Header-1", "11");
        request.Headers.Add("Header-2", "22");
        request.Headers.Add("Authorization", "Bearer token");

        // Act
        var response = await _client.SendAsync(request);
        
        // Assert
        Assert.AreEqual("operationsContent", _context.State.Operations);
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(string.Empty, responseString);
        Assert.AreEqual("admin", _context.RouteValues["domainId"]);
        Assert.AreEqual("ferrari", _context.RouteValues["brandId"]);
        Assert.AreEqual("id_car", _context.RouteValues["carId"]);
        Assert.AreEqual("1", _context.Query["param1"]);
        Assert.AreEqual("2", _context.Query["param2"]);
        Assert.AreEqual("11", _context.Headers["Header-1"]);
        Assert.AreEqual("22", _context.Headers["Header-2"]);
        await _crudServiceMock.Received(1).PatchAsync(_context, Arg.Any<CancellationToken>());
    }
    
    [TestMethod]
    public async Task Delete_Endpoint_Returns_Mock_Message()
    {
        // Arrange
        var url = "domains/admin/brands/ferrari/cars/id_car?param1=1&param2=2"; 
        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Add("Header-1", "11");
        request.Headers.Add("Header-2", "22");
        request.Headers.Add("Authorization", "Bearer token");

        // Act
        var response = await _client.SendAsync(request);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(string.Empty, responseString);
        Assert.AreEqual("admin", _context.RouteValues["domainId"]);
        Assert.AreEqual("ferrari", _context.RouteValues["brandId"]);
        Assert.AreEqual("id_car", _context.RouteValues["carId"]);
        Assert.AreEqual("1", _context.Query["param1"]);
        Assert.AreEqual("2", _context.Query["param2"]);
        Assert.AreEqual("11", _context.Headers["Header-1"]);
        Assert.AreEqual("22", _context.Headers["Header-2"]);
        await _crudServiceMock.Received(1).DeleteAsync(_context, Arg.Any<CancellationToken>());
    }

    private class Car : Resource
    {
        
    };
}