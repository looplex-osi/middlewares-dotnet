﻿using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.DTOs.Users
{
    public class UserDTO
    {
        #region Common

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("externalId")]
        public string ExternalId { get; set; }

        #endregion

        [JsonPropertyName("name")]
        public NameDTO Name { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("nickName")]
        public string NickName { get; set; }

        [JsonPropertyName("profileUrl")]
        public string ProfileUrl { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("userType")]
        public string UserType { get; set; }

        [JsonPropertyName("preferredLanguage")]
        public string PreferredLanguage { get; set; }

        [JsonPropertyName("locale")]
        public string Locale { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; } = true;

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("emails")]
        public List<EmailDTO> Emails { get; set; }

        [JsonPropertyName("phoneNumbers")]
        public List<PhoneNumberDTO> PhoneNumbers { get; set; }

        [JsonPropertyName("ims")]
        public List<IMDTO> Ims { get; set; }

        [JsonPropertyName("photos")]
        public List<PhotoDTO> Photos { get; set; }

        [JsonPropertyName("addresses")]
        public List<AddressDTO> Addresses { get; set; }

        [JsonPropertyName("entitlements")]
        public List<EntitlementDTO> Entitlements { get; set; }

        [JsonPropertyName("roles")]
        public List<RoleDTO> Roles { get; set; }
    }
}