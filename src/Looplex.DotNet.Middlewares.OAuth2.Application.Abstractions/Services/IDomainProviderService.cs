using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services
{
    public interface IDomainProviderService
    {
        string GetDomainFromHeader(HttpContext httpContext);
    }
}
