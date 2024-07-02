using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.DTOs.Groups
{
    public class GroupReadDTO
    {
        public GroupReadDTO() 
        {
            Schemas = ["urn:ietf:params:scim:schemas:core:2.0:Group"];
        }

        #region Common

        [JsonPropertyName("schemas")]
        public string[] Schemas { get; set; }

        [JsonPropertyName("meta")]
        public MetaDTO Meta { get; set; }

        #endregion

        [JsonPropertyName("members")]
        public List<MemberDTO> Members { get; set; }
    }
}
