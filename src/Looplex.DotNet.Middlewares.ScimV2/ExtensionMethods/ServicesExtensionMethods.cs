using Looplex.DotNet.Middlewares.ScimV2.Entities.Groups;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Schemas;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Users;
using Looplex.DotNet.Middlewares.ScimV2.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods
{
    public static class ServicesExtensionMethods
    {
        public static void AddScimV2AutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ScimV2Profile));
        }

        public static void AddScimV2Services(this IServiceCollection services)
        {

        }

        public static void AddScimV2Schemas()
        {
            Schema.Add<User>(File.ReadAllText("./Entities/Schemas/User.1.0.schema.json"));
            Schema.Add<Group>(File.ReadAllText("./Entities/Schemas/Group.1.0.schema.json"));
        }
    }
}
