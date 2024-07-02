using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users
{
    public class UserGroup
    {
        public string Value { get; set; }

        [Url(ErrorMessage = "Ref must be a valid URL.")]
        public string Ref { get; set; }

        [RegularExpression("direct|indirect", ErrorMessage = "Type must be either 'direct' or 'indirect'.")]
        public string Type { get; set; }
    }
}
