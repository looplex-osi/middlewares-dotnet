using FluentAssertions;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Middlewares.ApiKeys.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Dtos;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.UnitTests.Services
{
    [TestClass]
    public class DomainProviderServiceTestsTests
    {
        private IConfiguration _mockConfiguration = null!;
        private IApiKeyService _mockApiKeyService = null!;
        private IServiceProvider _mockServiceProvider = null!;
        private IContextFactory _mockContextFactory = null!;
        private IJwtService _mockJwtService = null!;
        private DefaultHttpContext _httpContext = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockConfiguration = Substitute.For<IConfiguration>();
            _mockApiKeyService = Substitute.For<IApiKeyService>();
            _mockServiceProvider = Substitute.For<IServiceProvider>();
            _mockContextFactory = Substitute.For<IContextFactory>();
            _mockJwtService = Substitute.For<IJwtService>();
            _httpContext = new DefaultHttpContext();

            var configurationSection = Substitute.For<IConfigurationSection>();
            configurationSection.Value.Returns("20");
            _mockConfiguration.GetSection("TokenExpirationTimeInMinutes").Returns(configurationSection);

            _mockServiceProvider.GetService(typeof(IContextFactory)).Returns(_mockContextFactory);
        }

        [TestMethod]
        public async Task DomainProviderService_GetDomainFromHeader_Not_Found_Header_Should_Return_null()
        {
            DomainProviderService service = new DomainProviderService();
            string domain = service.GetDomainFromHeader(_httpContext);

            domain.Should().BeNull();
        }
        [TestMethod]
        public async Task DomainProviderService_GetDomainFromHeader_Null_Context_Should_Return_null()
        {
            DomainProviderService service = new DomainProviderService();
            string domain = service.GetDomainFromHeader(null);

            domain.Should().BeNull();
        }

        [TestMethod]
        public async Task DomainProviderService_GetDomainFromHeader_Domain_Found()
        {
            string domain = "looplex.com.br";
            _httpContext.Request.Headers["X-looplex-tenant"] = domain;

            DomainProviderService service = new DomainProviderService();

            string result = service.GetDomainFromHeader(_httpContext);

            result.Should().Be(domain);
        }



    }
}
