using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Services
{
    public class DomainProviderService : IDomainProviderService
    {
        public string GetDomainFromHeader(HttpContext httpContext)
        {
            return httpContext.Request.Headers["X-looplex-tenant"];
        }
    }
}
