using Looplex.DotNet.Middlewares.OAuth2.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Entities;
using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.Clients.Entities
{
    public class Client : Resource, IClient
    {
        public required string DisplayName { get; init; }

        [Required(ErrorMessage = "Secret is required.")]
        public required string Secret { get; init; }

        [Required(ErrorMessage = "ExpirationTime is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "ExpirationTime must be a valid date.")]
        public required DateTime ExpirationTime { get; init; }

        [Required(ErrorMessage = "NotBefore is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "NotBefore must be a valid date.")]
        public required DateTime NotBefore { get; init; }

        public override bool IsValid(List<ValidationResult> validationResults)
        {
            throw new NotImplementedException();
        }
    }
}
