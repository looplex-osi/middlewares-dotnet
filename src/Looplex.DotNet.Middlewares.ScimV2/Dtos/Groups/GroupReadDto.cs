using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.Dtos.Groups
{
    public class GroupReadDto
    {
        #region Common

        [JsonPropertyName("schemas")]
        public string[] Schemas { get; set; } = ["urn:ietf:params:scim:schemas:core:2.0:Group"];

        [JsonPropertyName("meta")]
        public required MetaDto Meta { get; set; }

        #endregion

        [JsonPropertyName("members")]
        public List<MemberDto> Members { get; set; } = [];
    }
}
