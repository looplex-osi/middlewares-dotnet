using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities
{
    public abstract class Resource
    {
        [Required(ErrorMessage = "Id is required.")]
        public required string Id { get; set; }

        public string? ExternalId { get; set; }

        public required Meta Meta { get; set; }

        public bool IsValid(out List<ValidationResult> validationResults)
        {
            validationResults = [];
            var context = new ValidationContext(this, null, null);
            Validator.TryValidateObject(this, context, validationResults, true);
            
            var isValid = validationResults.Count == 0;
            if (isValid)
            {
                isValid = IsValid(validationResults);
            }
            
            return isValid;
        }

        public abstract bool IsValid(List<ValidationResult> validationResults);
    }
}
