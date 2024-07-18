namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

/// <summary>
///     A label indicating the attribute's function, e.g., 'work' or 'home'.
/// </summary>
public enum AddressType
{
    Home,
    Other,
    Work
}

/// <summary>
///     A label indicating the attribute's function, e.g., 'work' or 'home'.
/// </summary>
public enum EmailType
{
    Home,
    Other,
    Work
}

/// <summary>
///     A label indicating the attribute's function, e.g., 'direct' or 'indirect'.
/// </summary>
public enum GroupType
{
    Direct,
    Indirect
}

/// <summary>
///     A label indicating the attribute's function, e.g., 'work', 'home', 'mobile'.
/// </summary>
public enum PhoneNumberType
{
    Fax,
    Home,
    Mobile,
    Other,
    Pager,
    Work
}

/// <summary>
///     A label indicating the attribute's function, i.e., 'photo' or 'thumbnail'.
/// </summary>
public enum PhotoType
{
    Photo,
    Thumbnail
}