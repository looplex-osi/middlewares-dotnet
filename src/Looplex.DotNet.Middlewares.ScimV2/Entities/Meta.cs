using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities
{
    public class Meta
    {
        [Required(ErrorMessage = "ResourceType is required.")]
        public string ResourceType { get; set; }

        [Required(ErrorMessage = "Created date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Created must be a valid date.")]
        public DateTime Created { get; set; }

        [Required(ErrorMessage = "LastModified date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "LastModified must be a valid date.")]
        public DateTime LastModified { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [DataType(DataType.Url, ErrorMessage = "Location must be a valid URL.")]
        public string Location { get; set; }
    }
}
