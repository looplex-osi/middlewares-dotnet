using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

/// <summary>
///     The full name, including all middle names, titles, and suffixes as appropriate, formatted
///     for display (e.g., 'Ms. Barbara J Jensen, III').
/// </summary>
public partial class Name 
{
    private string? _familyName;
    private string? _formatted;
    private string? _givenName;
    private string? _honorificPrefix;
    private string? _honorificSuffix;
    private string? _middleName;

    /// <summary>
    ///     The family name of the User, or last name in most Western languages (e.g., 'Jensen' given
    ///     the full name 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("familyName", NullValueHandling = NullValueHandling.Ignore)]
    public string? FamilyName
    {
        get => _familyName;
        set
        {
            if (value != _familyName)
            {
                _familyName = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The full name, including all middle names, titles, and suffixes as appropriate, formatted
    ///     for display (e.g., 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("formatted", NullValueHandling = NullValueHandling.Ignore)]
    public string? Formatted
    {
        get => _formatted;
        set
        {
            if (value != _formatted)
            {
                _formatted = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The given name of the User, or first name in most Western languages (e.g., 'Barbara'
    ///     given the full name 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("givenName", NullValueHandling = NullValueHandling.Ignore)]
    public string? GivenName
    {
        get => _givenName;
        set
        {
            if (value != _givenName)
            {
                _givenName = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The honorific prefix(es) of the User, or title in most Western languages (e.g., 'Ms.'
    ///     given the full name 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("honorificPrefix", NullValueHandling = NullValueHandling.Ignore)]
    public string? HonorificPrefix
    {
        get => _honorificPrefix;
        set
        {
            if (value != _honorificPrefix)
            {
                _honorificPrefix = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The honorific suffix(es) of the User, or suffix in most Western languages (e.g., 'III'
    ///     given the full name 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("honorificSuffix", NullValueHandling = NullValueHandling.Ignore)]
    public string? HonorificSuffix
    {
        get => _honorificSuffix;
        set
        {
            if (value != _honorificSuffix)
            {
                _honorificSuffix = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The middle name(s) of the User (e.g., 'Jane' given the full name 'Ms. Barbara J Jensen,
    ///     III').
    /// </summary>
    [JsonProperty("middleName", NullValueHandling = NullValueHandling.Ignore)]
    public string? MiddleName
    {
        get => _middleName;
        set
        {
            if (value != _middleName)
            {
                _middleName = value;
                OnPropertyChanged();
            }
        }
    }
}