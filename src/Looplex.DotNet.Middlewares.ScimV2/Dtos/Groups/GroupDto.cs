using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.Dtos.Groups
{
    public class GroupDto
    {
        #region Common

        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("externalId")]
        public string? ExternalId { get; set; }

        #endregion
        
        [JsonPropertyName("displayName")]
        public required string DisplayName { get; set; }
    }
}
