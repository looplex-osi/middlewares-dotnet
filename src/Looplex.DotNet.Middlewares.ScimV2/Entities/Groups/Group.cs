using Looplex.DotNet.Middlewares.ScimV2.DTOs.Groups;
using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Groups
{
    public class Group : Resource
    {
        [Required(ErrorMessage = "DisplayName is required.")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Members are required.")]
        public List<MemberDTO> Members { get; set; }
    }
}
