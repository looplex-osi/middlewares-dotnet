using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.DTOs.Groups
{
    public class MemberDTO
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("$ref")]
        [Url(ErrorMessage = "The URI must be a valid URL.")]
        public string Ref { get; set; }

        [JsonPropertyName("type")]
        [RegularExpression("User|Group", ErrorMessage = "Type must be either 'User' or 'Group'.")]
        public string Type { get; set; }
    }
}
