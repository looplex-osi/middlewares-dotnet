using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users
{
    public class Photo
    {
        [Url(ErrorMessage = "Value must be a valid URL.")]
        public string Value { get; set; }

        [RegularExpression("photo|thumbnail", ErrorMessage = "Type must be either 'photo' or 'thumbnail'.")]
        public string Type { get; set; }

        public bool Primary { get; set; } = false;
    }
}
