using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.DTOs
{
    public class MetaDTO
    {
        [JsonPropertyName("resourceType")]
        [Required(ErrorMessage = "ResourceType is required.")]
        public string ResourceType { get; set; }

        [JsonPropertyName("created")]
        [Required(ErrorMessage = "Created date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Created must be a valid date.")]
        public DateTime Created { get; set; }

        [JsonPropertyName("lastModified")]
        [Required(ErrorMessage = "LastModified date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "LastModified must be a valid date.")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("location")]
        [Required(ErrorMessage = "Location is required.")]
        [DataType(DataType.Url, ErrorMessage = "Location must be a valid URL.")]
        public string Location { get; set; }
    }
}
