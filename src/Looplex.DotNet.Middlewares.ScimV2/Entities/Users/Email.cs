using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users
{
    public class Email
    {
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Value { get; set; }

        [RegularExpression("work|home|other", ErrorMessage = "Type must be either 'work', 'home', or 'other'.")]
        public string Type { get; set; }

        public bool Primary { get; set; } = false;
    }
}
