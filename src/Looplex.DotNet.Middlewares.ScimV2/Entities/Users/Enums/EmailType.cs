using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users.Enums;

public enum EmailType
{
    [Display(Name = "work")]
    Work,
    
    [Display(Name = "home")]
    Home,
    
    [Display(Name = "other")]
    Other
}