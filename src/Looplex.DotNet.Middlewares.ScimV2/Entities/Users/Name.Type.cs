using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

/// <summary>
///     The full name, including all middle names, titles, and suffixes as appropriate, formatted
///     for display (e.g., 'Ms. Barbara J Jensen, III').
/// </summary>
public partial class Name
{
    /// <summary>
    ///     The family name of the User, or last name in most Western languages (e.g., 'Jensen' given
    ///     the full name 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("familyName", NullValueHandling = NullValueHandling.Ignore)]
    public string? FamilyName { get; set; }

    /// <summary>
    ///     The full name, including all middle names, titles, and suffixes as appropriate, formatted
    ///     for display (e.g., 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("formatted", NullValueHandling = NullValueHandling.Ignore)]
    public string? Formatted { get; set; }

    /// <summary>
    ///     The given name of the User, or first name in most Western languages (e.g., 'Barbara'
    ///     given the full name 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("givenName", NullValueHandling = NullValueHandling.Ignore)]
    public string? GivenName { get; set; }

    /// <summary>
    ///     The honorific prefix(es) of the User, or title in most Western languages (e.g., 'Ms.'
    ///     given the full name 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("honorificPrefix", NullValueHandling = NullValueHandling.Ignore)]
    public string? HonorificPrefix { get; set; }

    /// <summary>
    ///     The honorific suffix(es) of the User, or suffix in most Western languages (e.g., 'III'
    ///     given the full name 'Ms. Barbara J Jensen, III').
    /// </summary>
    [JsonProperty("honorificSuffix", NullValueHandling = NullValueHandling.Ignore)]
    public string? HonorificSuffix { get; set; }

    /// <summary>
    ///     The middle name(s) of the User (e.g., 'Jane' given the full name 'Ms. Barbara J Jensen,
    ///     III').
    /// </summary>
    [JsonProperty("middleName", NullValueHandling = NullValueHandling.Ignore)]
    public string? MiddleName { get; set; }
}