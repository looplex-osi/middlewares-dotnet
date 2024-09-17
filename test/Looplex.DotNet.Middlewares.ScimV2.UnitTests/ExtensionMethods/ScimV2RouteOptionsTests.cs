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
using Looplex.OpenForExtension.Abstractions.Contexts;
using NSubstitute;
using Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;
using Microsoft.Extensions.Configuration;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.ExtensionMethods;

[TestClass]
public class ScimV2RouteOptionsTests
{
    private IConfiguration _configurationMock = null!;
    private ICrudService _crudServiceMock = null!;
    private IApiKeyService _apiKeyServiceMock = null!;
    private IContextFactory _contextFactoryMock = null!;
    private IServiceProvider _serviceProviderMock = null!;
    private IJwtService _jwtServiceMock = null!;
    private IHttpClientFactory _httpClientFactoryMock = null!;
    private IContext _context = null!;
    private HttpClient _client = null!;
    private IHost _host = null!;

    [TestInitialize]
    public void Initialize()
    {
        _configurationMock = Substitute.For<IConfiguration>();
        _crudServiceMock = Substitute.For<ICrudService>();
        _apiKeyServiceMock = Substitute.For<IApiKeyService>();
        _contextFactoryMock = Substitute.For<IContextFactory>();
        _serviceProviderMock = Substitute.For<IServiceProvider>();
        _jwtServiceMock = Substitute.For<IJwtService>();
        _jwtServiceMock
            .ValidateToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);
        _httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
        _serviceProviderMock.GetService(typeof(IConfiguration)).Returns(_configurationMock);
        _serviceProviderMock.GetService(typeof(IJwtService)).Returns(_jwtServiceMock);  
        _context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        _context.State.Returns(state);
        _context.Result.Returns("mock_result");
        _context.Services.Returns(_serviceProviderMock); 
        _contextFactoryMock.Create(Arg.Any<IEnumerable<string>>()).Returns(_context);
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
                        
                        services.AddOAuth2Services();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.UseScimV2Routes<ICrudService>("cars", new ScimV2RouteOptions());
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
        var url = "/cars?page=1&perPage=10"; 

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(responseString.Contains("mock_result"));
        Assert.AreEqual(1, _context.State.Pagination.Page);
        Assert.AreEqual(10, _context.State.Pagination.PerPage);
    }
    
    [TestMethod]
    public async Task GetById_Endpoint_Returns_Mock_Message()
    {
        // Arrange
        var url = "/cars/id_car"; 

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(responseString.Contains("mock_result"));
        Assert.AreEqual("id_car", _context.State.Id);
        await _crudServiceMock.Received(1).GetByIdAsync(_context, Arg.Any<CancellationToken>());
    }
    
    [TestMethod]
    public async Task Post_Endpoint_Returns_Mock_Message()
    {
        // Arrange
        var url = "/cars"; 
        HttpContent content = new StringContent("resourceContent", Encoding.UTF8, "application/text");
        
        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        Assert.AreEqual("resourceContent", _context.State.Resource);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(string.Empty, responseString);
        Assert.AreEqual("cars/mock_result", response.Headers.Location!.ToString());
        await _crudServiceMock.Received(1).CreateAsync(_context, Arg.Any<CancellationToken>());
    }
    
    [TestMethod]
    public async Task Patch_Endpoint_Returns_Mock_Message()
    {
        // Arrange
        var url = "/cars/id_car"; 
        HttpContent content = new StringContent("operationsContent", Encoding.UTF8, "application/text");
        
        // Act
        var response = await _client.PatchAsync(url, content);
        
        // Assert
        Assert.AreEqual("operationsContent", _context.State.Operations);
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(string.Empty, responseString);
        Assert.AreEqual("id_car", _context.State.Id);
        Assert.AreEqual("cars/id_car", response.Headers.Location!.ToString());
        await _crudServiceMock.Received(1).PatchAsync(_context, Arg.Any<CancellationToken>());
    }
    
    [TestMethod]
    public async Task Delete_Endpoint_Returns_Mock_Message()
    {
        // Arrange
        var url = "/cars/id_car"; 
        HttpContent content = new StringContent("operationsContent", Encoding.UTF8, "application/text");
        
        // Act
        var response = await _client.DeleteAsync(url);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(string.Empty, responseString);
        Assert.AreEqual("id_car", _context.State.Id);
        await _crudServiceMock.Received(1).DeleteAsync(_context, Arg.Any<CancellationToken>());
    }
}