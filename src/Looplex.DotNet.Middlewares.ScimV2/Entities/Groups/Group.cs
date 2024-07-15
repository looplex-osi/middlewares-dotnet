using System.ComponentModel.DataAnnotations;
using Looplex.DotNet.Middlewares.ScimV2.Dtos.Groups;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Groups
{
    public class Group : Resource
    {
        [Required(ErrorMessage = "DisplayName is required.")]
        public required string DisplayName { get; set; }

        [Required(ErrorMessage = "Members are required.")]
        public List<MemberDto> Members { get; set; } = [];

        public override bool IsValid(List<ValidationResult> validationResults)
        {
            throw new NotImplementedException();
        }
    }
}
