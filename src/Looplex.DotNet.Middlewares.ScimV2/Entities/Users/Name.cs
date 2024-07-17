using System.ComponentModel.DataAnnotations;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users
{
    public class Name
    {
        public string Formatted => $"{HonorificPrefix} {GivenName} {MiddleName} {FamilyName}, {HonorificSuffix}";

        [LooplexRequired]
        [LooplexMinLength(2)]
        public required string FamilyName { get; set; }
        
        [LooplexRequired]
        [LooplexMinLength(2)]
        public required string GivenName { get; set; }
        
        [LooplexNullOrNotEmpty]
        public string? MiddleName { get; set; }

        [LooplexNullOrNotEmpty]
        public string? HonorificPrefix { get; set; }

        [LooplexNullOrNotEmpty]
        public string? HonorificSuffix { get; set; }
    }
}
