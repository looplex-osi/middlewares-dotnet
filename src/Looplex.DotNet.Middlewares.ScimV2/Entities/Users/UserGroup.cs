using Looplex.DotNet.Middlewares.ScimV2.Entities.Users.Enums;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users
{
    public class UserGroup
    {
        [LooplexRequired]
        public required string Value { get; set; }

        [LooplexUrl]
        public string? Ref { get; set; }

        [LooplexEnumDataType(typeof(UserGroupType))]
        public string? Type { get; set; }
    }
}
