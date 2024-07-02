using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.DTOs.Users
{
    public class UserReadDTO : UserDTO
    {
        public UserReadDTO()
        {
            Schemas = ["urn:ietf:params:scim:schemas:core:2.0:User"];
        }

        #region Common

        [JsonPropertyName("schemas")]
        public string[] Schemas { get; set; }

        [JsonPropertyName("meta")]
        public MetaDTO Meta { get; set; }

        #endregion

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("groups")]
        public List<UserGroupDTO> Groups { get; set; }
    }
}
