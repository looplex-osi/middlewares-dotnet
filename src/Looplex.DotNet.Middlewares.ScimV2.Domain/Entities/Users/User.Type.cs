using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    private bool? _active;
    private IList<AddressElement> _addresses = new ObservableCollection<AddressElement>();
    private string? _displayName;
    private IList<EmailElement> _emails = new ObservableCollection<EmailElement>();
    private IList<EntitlementElement> _entitlements = new ObservableCollection<EntitlementElement>();
    private IList<GroupElement> _groups = new ObservableCollection<GroupElement>();
    private IList<ImElement> _ims = new ObservableCollection<ImElement>();
    private string? _locale;
    private Name? _name;
    private string? _nickName;
    private string? _password;
    private IList<PhoneNumberElement> _phoneNumbers = new ObservableCollection<PhoneNumberElement>();
    private IList<PhotoElement> _photos = new ObservableCollection<PhotoElement>();
    private string? _preferredLanguage;
    private string? _profileUrl;
    private IList<RoleElement> _roles = new ObservableCollection<RoleElement>();
    private string? _timezone;
    private string? _title;
    private string? _userName;
    private string? _userType;

    /// <summary>
    ///     A Boolean value indicating the User's administrative status.
    /// </summary>
    [JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Active
    {
        get => _active;
        set
        {
            if (value != _active)
            {
                _active = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     A physical mailing address for this user.
    /// </summary>
    [JsonProperty("addresses", NullValueHandling = NullValueHandling.Ignore)]
    public IList<AddressElement> Addresses
    {
        get => _addresses;
        set
        {
            if (_addresses != null) throw new InvalidOperationException();
            
            _addresses = value;
            if (value is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnCollectionChanged;
        }
    }

    /// <summary>
    ///     The name of the User, suitable for display to end-users. The name SHOULD be the full
    ///     name of the User being described, if known.
    /// </summary>
    [JsonProperty("displayName", NullValueHandling = NullValueHandling.Ignore)]
    public string? DisplayName
    {
        get => _displayName;
        set
        {
            if (value != _displayName)
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Email addresses for the User. The value SHOULD be specified according to [RFC5321].
    /// </summary>
    [JsonProperty("emails", NullValueHandling = NullValueHandling.Ignore)]
    public IList<EmailElement> Emails
    {
        get => _emails;
        set
        {
            if (_emails != null) throw new InvalidOperationException();
            
            _emails = value;
            if (value is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnCollectionChanged;
        }
    }

    /// <summary>
    ///     A list of entitlements for the user that represent a thing the user has. An entitlement
    ///     may be an additional right to a thing, object, or service. No vocabulary or syntax is
    ///     specified; service providers and clients are expected to encode sufficient information in
    ///     the value so as to accurately and without ambiguity determine what the user has access
    ///     to. This value has no canonical types, although a type may be useful as a means to scope
    ///     entitlements.
    /// </summary>
    [JsonProperty("entitlements", NullValueHandling = NullValueHandling.Ignore)]
    public IList<EntitlementElement> Entitlements
    {
        get => _entitlements;
        set
        {
            if (_entitlements != null) throw new InvalidOperationException();
            
            _entitlements = value;
            if (value is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnCollectionChanged;
        }
    }

    /// <summary>
    ///     A list of groups to which the user belongs, either through direct membership, through
    ///     nested groups, or dynamically calculated.
    /// </summary>
    [JsonProperty("groups", NullValueHandling = NullValueHandling.Ignore)]
    public IList<GroupElement> Groups
    {
        get => _groups;
        set
        {
            if (_groups != null) throw new InvalidOperationException();
            
            _groups = value;
            if (value is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnCollectionChanged;
        }
    }

    /// <summary>
    ///     Instant messaging address for the user. No official canonicalization rules exist for all
    ///     instant messaging addresses, but service providers SHOULD, when appropriate, remove all
    ///     whitespace and convert the address to lowercase.
    /// </summary>
    [JsonProperty("ims", NullValueHandling = NullValueHandling.Ignore)]
    public IList<ImElement> Ims
    {
        get => _ims;
        set
        {
            if (_ims != null) throw new InvalidOperationException();
            
            _ims = value;
            if (value is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnCollectionChanged;
        }
    }

    /// <summary>
    ///     Used to indicate the User's default location for purposes of localizing items such as
    ///     currency, date time format, or numerical representations. A valid value is a language
    ///     tag as defined in [RFC5646].
    /// </summary>
    [JsonProperty("locale", NullValueHandling = NullValueHandling.Ignore)]
    public string? Locale
    {
        get => _locale;
        set
        {
            if (value != _locale)
            {
                _locale = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The full name, including all middle names, titles, and suffixes as appropriate, formatted
    ///     for display (e.g., 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public Name? Name
    {
        get => _name;
        set
        {
            if (value != _name)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The casual way to address the user in real life, e.g., 'Bob' or 'Bobby' instead of
    ///     'Robert'. This attribute SHOULD NOT be used to represent a User's username (e.g.,
    ///     'bjensen' or 'mpepperidge').
    /// </summary>
    [JsonProperty("nickName", NullValueHandling = NullValueHandling.Ignore)]
    public string? NickName
    {
        get => _nickName;
        set
        {
            if (value != _nickName)
            {
                _nickName = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The User's cleartext password. This attribute is intended to be used as a means to
    ///     specify an initial password when creating a new User or to reset an existing User's
    ///     password.
    /// </summary>
    [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
    public string? Password
    {
        get => _password;
        set
        {
            if (value != _password)
            {
                _password = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Phone numbers for the user. The value SHOULD be specified according to the format
    ///     defined in [RFC3966].
    /// </summary>
    [JsonProperty("phoneNumbers", NullValueHandling = NullValueHandling.Ignore)]
    public IList<PhoneNumberElement> PhoneNumbers
    {
        get => _phoneNumbers;
        set
        {
            if (_phoneNumbers != null) throw new InvalidOperationException();
            
            _phoneNumbers = value;
            if (value is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnCollectionChanged;
        }
    }

    /// <summary>
    ///     A URI that is a uniform resource locator (as defined in Section 1.1.3 of [RFC3986]) that
    ///     points to a resource location representing the user's image.
    /// </summary>
    [JsonProperty("photos", NullValueHandling = NullValueHandling.Ignore)]
    public IList<PhotoElement> Photos
    {
        get => _photos;
        set
        {
            if (_photos != null) throw new InvalidOperationException();
            
            _photos = value;
            if (value is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnCollectionChanged;
        }
    }

    /// <summary>
    ///     Indicates the User's preferred written or spoken language. Generally used for selecting
    ///     a localized user interface; e.g., 'en_US' specifies the language English and country as
    ///     defined in [RFC7231].
    /// </summary>
    [JsonProperty("preferredLanguage", NullValueHandling = NullValueHandling.Ignore)]
    public string? PreferredLanguage
    {
        get => _preferredLanguage;
        set
        {
            if (value != _preferredLanguage)
            {
                _preferredLanguage = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     A fully qualified URL pointing to a page representing the User's online profile.
    /// </summary>
    [JsonProperty("profileUrl", NullValueHandling = NullValueHandling.Ignore)]
    public string? ProfileUrl
    {
        get => _profileUrl;
        set
        {
            if (value != _profileUrl)
            {
                _profileUrl = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     A list of roles for the user that collectively represent who the user is, e.g.,
    ///     'Student', 'Faculty'.
    /// </summary>
    [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
    public IList<RoleElement> Roles
    {
        get => _roles;
        set
        {
            if (_roles != null) throw new InvalidOperationException();
            
            _roles = value;
            if (value is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnCollectionChanged;
        }
    }

    /// <summary>
    ///     The User's time zone, in IANA Time Zone database format [RFC6557].
    /// </summary>
    [JsonProperty("timezone", NullValueHandling = NullValueHandling.Ignore)]
    public string? Timezone
    {
        get => _timezone;
        set
        {
            if (value != _timezone)
            {
                _timezone = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The user's title, such as 'Vice President.'
    /// </summary>
    [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
    public string? Title
    {
        get => _title;
        set
        {
            if (value != _title)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Unique identifier for the User, typically used by the user to directly authenticate to
    ///     the service provider. Each User MUST include a non-empty userName value.  This identifier
    ///     MUST be unique across the service provider's entire set of Users. REQUIRED.
    /// </summary>
    [JsonProperty("userName")]
    public string? UserName
    {
        get => _userName;
        set
        {
            if (value != _userName)
            {
                _userName = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Used to identify the relationship between the organization and the user.  Typical values
    ///     used might be 'Contractor', 'Employee', 'Intern', 'Temp', 'External', and 'Unknown', but
    ///     any value may be used.
    /// </summary>
    [JsonProperty("userType", NullValueHandling = NullValueHandling.Ignore)]
    public string? UserType 
    {
        get => _userType;
        set
        {
            if (value != _userType)
            {
                _userType = value;
                OnPropertyChanged();
            }
        }
    }
}