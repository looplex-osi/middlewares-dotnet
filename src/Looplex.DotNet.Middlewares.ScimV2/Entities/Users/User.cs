using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users
{
    public class User : Resource
    {
        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { get; set; }

        public Name Name { get; set; }

        public string DisplayName { get; set; }

        public string NickName { get; set; }

        [Url(ErrorMessage = "Profile URL must be a valid URL.")]
        public string ProfileUrl { get; set; }

        public string Title { get; set; }

        public string UserType { get; set; }

        public string PreferredLanguage { get; set; }

        public string Locale { get; set; }

        public string Timezone { get; set; }

        public bool Active { get; set; } = true;

        public string Password { get; set; }

        public List<Email> Emails { get; set; }

        public List<PhoneNumber> PhoneNumbers { get; set; }

        public List<IM> Ims { get; set; }

        public List<Photo> Photos { get; set; }

        public List<Address> Addresses { get; set; }

        public List<UserGroup> Groups { get; set; }

        public List<Entitlement> Entitlements { get; set; }

        public List<Role> Roles { get; set; }
    }
}
