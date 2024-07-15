using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.Dtos.Users
{
    public class UserReadDto : UserDto
    {
        #region Common
        
        [JsonPropertyName("schemas")]
        public string[] Schemas { get; set; } = ["urn:ietf:params:scim:schemas:core:2.0:User"];

        [JsonPropertyName("meta")]
        public required MetaDto Meta { get; set; }

        #endregion

        [JsonPropertyName("name")]
        public required NameReadDto Name { get; set; }
        
        [JsonPropertyName("userName")]
        public required string UserName { get; set; }

        [JsonPropertyName("groups")]
        public List<UserGroupDto> Groups { get; set; } = [];
    }
}
