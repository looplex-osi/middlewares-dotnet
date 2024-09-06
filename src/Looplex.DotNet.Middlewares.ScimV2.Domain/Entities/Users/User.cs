using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

/// <summary>
///     SCIM provides a resource type for "User" resources.  The core schema
///     for "User" is identified using the following schema URI:
///     "urn:ietf:params:scim:schemas:core:2.0:User".
///     <see cref="https://datatracker.ietf.org/doc/html/rfc7643#section-4.1" />
/// </summary>
public partial class User : Resource
{
    /// <summary>
    ///     A Boolean value indicating the User's administrative status.
    /// </summary>
    [JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
    public virtual bool? Active { get; set; }

    /// <summary>
    ///     A physical mailing address for this user.
    /// </summary>
    [JsonProperty("addresses", NullValueHandling = NullValueHandling.Ignore)]
    public virtual IList<AddressElement> Addresses { get; set; } = new ObservableCollection<AddressElement>();

    /// <summary>
    ///     The name of the User, suitable for display to end-users. The name SHOULD be the full
    ///     name of the User being described, if known.
    /// </summary>
    [JsonProperty("displayName", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? DisplayName { get; set; }

    /// <summary>
    ///     Email addresses for the User. The value SHOULD be specified according to [RFC5321].
    /// </summary>
    [JsonProperty("emails", NullValueHandling = NullValueHandling.Ignore)]
    public virtual IList<EmailElement> Emails { get; set; } = new ObservableCollection<EmailElement>();

    /// <summary>
    ///     A list of entitlements for the user that represent a thing the user has. An entitlement
    ///     may be an additional right to a thing, object, or service. No vocabulary or syntax is
    ///     specified; service providers and clients are expected to encode sufficient information in
    ///     the value so as to accurately and without ambiguity determine what the user has access
    ///     to. This value has no canonical types, although a type may be useful as a means to scope
    ///     entitlements.
    /// </summary>
    [JsonProperty("entitlements", NullValueHandling = NullValueHandling.Ignore)]
    public virtual IList<EntitlementElement> Entitlements { get; set; } = new ObservableCollection<EntitlementElement>();

    /// <summary>
    ///     A list of groups to which the user belongs, either through direct membership, through
    ///     nested groups, or dynamically calculated.
    /// </summary>
    [JsonProperty("groups", NullValueHandling = NullValueHandling.Ignore)]
    public virtual IList<GroupElement> Groups { get; set; } = new List<GroupElement>();

    /// <summary>
    ///     Instant messaging address for the user. No official canonicalization rules exist for all
    ///     instant messaging addresses, but service providers SHOULD, when appropriate, remove all
    ///     whitespace and convert the address to lowercase.
    /// </summary>
    [JsonProperty("ims", NullValueHandling = NullValueHandling.Ignore)]
    public virtual IList<ImElement> Ims { get; set; } = new ObservableCollection<ImElement>();

    /// <summary>
    ///     Used to indicate the User's default location for purposes of localizing items such as
    ///     currency, date time format, or numerical representations. A valid value is a language
    ///     tag as defined in [RFC5646].
    /// </summary>
    [JsonProperty("locale", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? Locale { get; set; }

    /// <summary>
    ///     The full name, including all middle names, titles, and suffixes as appropriate, formatted
    ///     for display (e.g., 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public virtual Name? Name { get; set; }

    /// <summary>
    ///     The casual way to address the user in real life, e.g., 'Bob' or 'Bobby' instead of
    ///     'Robert'. This attribute SHOULD NOT be used to represent a User's username (e.g.,
    ///     'bjensen' or 'mpepperidge').
    /// </summary>
    [JsonProperty("nickName", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? NickName { get; set; }

    /// <summary>
    ///     The User's cleartext password. This attribute is intended to be used as a means to
    ///     specify an initial password when creating a new User or to reset an existing User's
    ///     password.
    /// </summary>
    [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? Password { get; set; }

    /// <summary>
    ///     Phone numbers for the user. The value SHOULD be specified according to the format
    ///     defined in [RFC3966].
    /// </summary>
    [JsonProperty("phoneNumbers", NullValueHandling = NullValueHandling.Ignore)]
    public virtual IList<PhoneNumberElement> PhoneNumbers { get; set; } = new ObservableCollection<PhoneNumberElement>();

    /// <summary>
    ///     A URI that is a uniform resource locator (as defined in Section 1.1.3 of [RFC3986]) that
    ///     points to a resource location representing the user's image.
    /// </summary>
    [JsonProperty("photos", NullValueHandling = NullValueHandling.Ignore)]
    public virtual IList<PhotoElement> Photos { get; set; } = new ObservableCollection<PhotoElement>();

    /// <summary>
    ///     Indicates the User's preferred written or spoken language. Generally used for selecting
    ///     a localized user interface; e.g., 'en_US' specifies the language English and country as
    ///     defined in [RFC7231].
    /// </summary>
    [JsonProperty("preferredLanguage", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? PreferredLanguage { get; set; }

    /// <summary>
    ///     A fully qualified URL pointing to a page representing the User's online profile.
    /// </summary>
    [JsonProperty("profileUrl", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? ProfileUrl { get; set; }

    /// <summary>
    ///     A list of roles for the user that collectively represent who the user is, e.g.,
    ///     'Student', 'Faculty'.
    /// </summary>
    [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
    public virtual IList<RoleElement> Roles { get; set; } = new ObservableCollection<RoleElement>();

    /// <summary>
    ///     The User's time zone, in IANA Time Zone database format [RFC6557].
    /// </summary>
    [JsonProperty("timezone", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? Timezone { get; set; }

    /// <summary>
    ///     The user's title, such as 'Vice President.'
    /// </summary>
    [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? Title { get; set; }

    /// <summary>
    ///     Unique identifier for the User, typically used by the user to directly authenticate to
    ///     the service provider. Each User MUST include a non-empty userName value.  This identifier
    ///     MUST be unique across the service provider's entire set of Users. REQUIRED.
    /// </summary>
    [JsonProperty("userName")]
    public virtual string? UserName { get; set; }

    /// <summary>
    ///     Used to identify the relationship between the organization and the user.  Typical values
    ///     used might be 'Contractor', 'Employee', 'Intern', 'Temp', 'External', and 'Unknown', but
    ///     any value may be used.
    /// </summary>
    [JsonProperty("userType", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? UserType { get; set; }
}