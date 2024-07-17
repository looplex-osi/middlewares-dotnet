using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;
using Looplex.DotNet.Middlewares.ScimV2.Services;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users
{
    /// <summary>
    /// SCIM provides a resource type for "User" resources.  The core schema
    /// for "User" is identified using the following schema URI:
    /// "urn:ietf:params:scim:schemas:core:2.0:User". 
    /// </summary>
    /// <see cref="https://datatracker.ietf.org/doc/html/rfc7643#section-4.1"/>
    /// <param name="userService"></param>
    public class User(IUserService userService) : Resource
    {
        private readonly IUserService _userService = userService;
        
        [LooplexRequired]
        [LooplexMinLength(8)]
        public required string UserName { get; set; }
        
        [LooplexRequired]
        [LooplexValid]
        public required Name Name { get; set; }

        [LooplexNullOrNotEmpty]
        public string? DisplayName { get; set; }

        [LooplexNullOrNotEmpty]
        public string? NickName { get; set; }

        [LooplexUrl]
        public string? ProfileUrl { get; set; }

        [LooplexNullOrNotEmpty(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "StringCannotBeEmpty")]
        public string? Title { get; set; }

        [LooplexNullOrNotEmpty(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "StringCannotBeEmpty")]
        public string? UserType { get; set; }

        /// <summary>
        /// The format of the value is the same as the HTTP Accept-Language
        /// header field (not including "Accept-Language:") and is specified
        /// in Section 5.3.5 of [RFC7231].
        /// <see cref="https://datatracker.ietf.org/doc/html/rfc7231#section-5.3.5"/>
        /// </summary>
        [LooplexAcceptedLanguage(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "PropertyIsInvalid")]
        public string? PreferredLanguage { get; set; }

        /// <summary>
        /// A valid value is a language tag as defined in [RFC5646]
        /// <see cref="https://datatracker.ietf.org/doc/html/rfc5646"/>
        /// </summary>
        [LooplexLanguageTag(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "PropertyIsInvalid")]
        public string? Locale { get; set; }

        /// <summary>
        /// Must be in IANA Time Zone database format [RFC6557]
        /// <see cref="https://datatracker.ietf.org/doc/html/rfc6557"/>
        /// </summary>
        [LooplexIanaTimezone(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "PropertyIsInvalid")]
        public string? Timezone { get; set; }

        public bool Active { get; set; } = true;

        [LooplexValid(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "PropertyIsInvalid")]
        public List<Email> Emails { get; set; }= [];

        [LooplexValid(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "PropertyIsInvalid")]
        public List<PhoneNumber> PhoneNumbers { get; set; } = [];

        [LooplexValid(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "PropertyIsInvalid")]
        public List<InstantMessaging> InstantMessagings { get; set; } = [];

        [LooplexValid(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "PropertyIsInvalid")]
        public List<Photo> Photos { get; set; } = [];
        
        [LooplexValid(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "PropertyIsInvalid")]
        public List<Address> Addresses { get; set; } = [];

        [ReadOnly(true)]
        public List<UserGroup> Groups { get; set; } = [];

        [LooplexValid(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "PropertyIsInvalid")]
        public List<Entitlement> Entitlements { get; set; } = [];
        
        [LooplexValid(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "PropertyIsInvalid")]
        public List<Role> Roles { get; set; } = [];

        public override bool IsValid(List<ValidationResult> validationResults)
        {
            // TODO validate unique username ??
            throw new NotImplementedException();
        }
    }
}
