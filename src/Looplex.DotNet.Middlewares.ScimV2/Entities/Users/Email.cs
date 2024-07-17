using Looplex.DotNet.Middlewares.ScimV2.Entities.Users.Enums;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users
{
    public class Email
    {
        /// <summary>
        /// The value SHOULD be specified according to [RFC5321].
        /// <see cref="https://datatracker.ietf.org/doc/html/rfc5321"/>
        /// </summary>
        [LooplexRequired]
        [LooplexEmailAddress]
        public required string Value { get; set; }

        [LooplexEnumDataType(typeof(EmailType))]
        public string? Type { get; set; }

        public bool Primary { get; set; } = false;
    }
}
