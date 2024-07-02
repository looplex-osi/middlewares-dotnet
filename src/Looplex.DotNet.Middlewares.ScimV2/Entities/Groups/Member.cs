using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Groups
{
    public class Member
    {
        public string Value { get; set; }

        [Url(ErrorMessage = "The URI must be a valid URL.")]
        public string Ref { get; set; }

        [RegularExpression("User|Group", ErrorMessage = "Type must be either 'User' or 'Group'.")]
        public string Type { get; set; }
    }
}
