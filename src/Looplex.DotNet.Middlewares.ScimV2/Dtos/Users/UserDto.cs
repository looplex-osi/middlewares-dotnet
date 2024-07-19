using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.Dtos.Users
{
    public class UserDto
    {
        #region Common

        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("externalId")]
        public string? ExternalId { get; set; }

        #endregion

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("nickName")]
        public string? NickName { get; set; }

        [JsonPropertyName("profileUrl")]
        public string? ProfileUrl { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("userType")]
        public string? UserType { get; set; }

        [JsonPropertyName("preferredLanguage")]
        public string? PreferredLanguage { get; set; }

        [JsonPropertyName("locale")]
        public string? Locale { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("active")]
        public bool? Active { get; set; }

        [JsonPropertyName("emails")]
        public List<EmailDto> Emails { get; set; } = [];

        [JsonPropertyName("phoneNumbers")]
        public List<PhoneNumberDto> PhoneNumbers { get; set; } = [];

        [JsonPropertyName("ims")]
        public List<InstantMessagingDto> Ims { get; set; } = [];

        [JsonPropertyName("photos")]
        public List<PhotoDto> Photos { get; set; } = [];

        [JsonPropertyName("addresses")]
        public List<AddressDto> Addresses { get; set; } = [];

        [JsonPropertyName("entitlements")]
        public List<EntitlementDto> Entitlements { get; set; } = [];

        [JsonPropertyName("roles")]
        public List<RoleDto> Roles { get; set; } = [];
    }
}
