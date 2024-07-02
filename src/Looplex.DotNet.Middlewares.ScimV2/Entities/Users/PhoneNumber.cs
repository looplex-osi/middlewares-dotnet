using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users
{
    public class PhoneNumber
    {
        public string Value { get; set; }

        [RegularExpression("work|home|mobile|fax|pager|other", ErrorMessage = "Type must be one of 'work', 'home', 'mobile', 'fax', 'pager', or 'other'.")]
        public string Type { get; set; }

        public bool Primary { get; set; } = false;
    }
}
