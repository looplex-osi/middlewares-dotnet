using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Middlewares.OAuth2.Entities;
using Looplex.OpenForExtension.Commands;
using Looplex.OpenForExtension.Context;
using Looplex.OpenForExtension.ExtensionMethods;

namespace Looplex.DotNet.Middlewares.OAuth2.Services
{
    public class ClientCredentialService : IClientCredentialService
    {
        private static readonly IList<IClient> _clients = [];

        public Task CreateAsync(IDefaultContext context)
        {
            context.Plugins.Execute<IHandleInput>(context);
            IClient client = context.GetRequiredValue<IClient>("Client");

            context.Plugins.Execute<IValidateInput>(context);

            context.Plugins.Execute<IDefineActors>(context);

            context.Plugins.Execute<IBind>(context);

            context.Plugins.Execute<IBeforeAction>(context);

            if (!context.SkipDefaultAction)
            {
                var clientId = Guid.NewGuid();
                _clients.Add(new Client
                {
                    DisplayName = client.DisplayName,
                    ClientId = clientId.ToString(),
                    Secret = client.Secret,
                    ExpirationTime = client.ExpirationTime,
                    NotBefore = client.NotBefore,
                });

                context.Result = clientId;
            }

            context.Plugins.Execute<IAfterAction>(context);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(IDefaultContext context)
        {
            context.Plugins.Execute<IHandleInput>(context);
            Guid id = context.GetRequiredValue<Guid>("Id");

            context.Plugins.Execute<IValidateInput>(context);

            context.Plugins.Execute<IDefineActors>(context);

            context.Plugins.Execute<IBind>(context);

            context.Plugins.Execute<IBeforeAction>(context);

            if (!context.SkipDefaultAction)
            {
                var deleted = false;
                var client = _clients.FirstOrDefault(c => Guid.Parse(c.ClientId) == id);
                if (client != null)
                {
                    _clients.Remove(client);
                    deleted = true;
                }

                context.Result = deleted;
            }

            context.Plugins.Execute<IAfterAction>(context);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context);

            return Task.CompletedTask;
        }

        public Task GetAll(IDefaultContext context)
        {
            context.Plugins.Execute<IHandleInput>(context);
            var page = context.GetRequiredValue<int>("Pagination.Page");
            var perPage = context.GetRequiredValue<int>("Pagination.PerPage");

            context.Plugins.Execute<IValidateInput>(context);

            context.Plugins.Execute<IDefineActors>(context);

            context.Plugins.Execute<IBind>(context);

            context.Plugins.Execute<IBeforeAction>(context);

            if (!context.SkipDefaultAction)
            {
                var records = _clients
                    .Skip(PaginationUtils.GetOffset(perPage, page))
                    .Take(perPage)
                    .ToList();

                var result = new PaginatedCollection<IClient>
                {
                    Records = records,
                    Page = page,
                    PerPage = perPage,
                    TotalCount = _clients.Count
                };

                context.Result = result;
            }

            context.Plugins.Execute<IAfterAction>(context);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context);
                        
            return Task.CompletedTask;
        }

        public Task GetAsync(IDefaultContext context)
        {
            context.Plugins.Execute<IHandleInput>(context);
            Guid id = context.GetRequiredValue<Guid>("Id");

            context.Plugins.Execute<IValidateInput>(context);

            context.Plugins.Execute<IDefineActors>(context);

            context.Plugins.Execute<IBind>(context);

            context.Plugins.Execute<IBeforeAction>(context);

            if (!context.SkipDefaultAction)
            {
                var client = _clients.FirstOrDefault(c => Guid.Parse(c.ClientId) == id);

                if (client == null)
                {
                    throw new EntityNotFoundException(nameof(IClient), id.ToString());
                }

                context.Result = client;
            }

            context.Plugins.Execute<IAfterAction>(context);

            context.Plugins.Execute<IReleaseUnmanagedResources>(context);

            return Task.CompletedTask;
        }

        public Task<IClient?> GetByIdAndSecretOrDefaultAsync(Guid id, string secret)
        {
            var client = _clients.FirstOrDefault(c => Guid.Parse(c.ClientId) == id && c.Secret == secret);

            return Task.FromResult(client);
        }
    }
}
