using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users.Enums;

public enum UserGroupType
{
    [Display(Name = "direct")]
    Direct,
    
    [Display(Name = "indirect")]
    Indirect,
}