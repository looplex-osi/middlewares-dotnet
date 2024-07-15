using System.ComponentModel.DataAnnotations;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users
{
    public class Name
    {
        public string Formatted => $"{HonorificPrefix} {GivenName} {MiddleName} {FamilyName}, {HonorificSuffix}";

        [Required(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "StringCannotBeEmpty", AllowEmptyStrings = false)]
        [MinLength(2, ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "StringDoesNotHaveMinLength")]
        public required string FamilyName { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "StringCannotBeEmpty", AllowEmptyStrings = false)]
        [MinLength(2, ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "StringDoesNotHaveMinLength")]
        public required string GivenName { get; set; }
        
        [NullOrNotEmpty(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "StringCannotBeEmpty")]
        public string? MiddleName { get; set; }

        [NullOrNotEmpty(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "StringCannotBeEmpty")]
        public string? HonorificPrefix { get; set; }

        [NullOrNotEmpty(ErrorMessageResourceType = typeof(Resources.ScimV2.Common), ErrorMessageResourceName = "StringCannotBeEmpty")]
        public string? HonorificSuffix { get; set; }
    }
}
