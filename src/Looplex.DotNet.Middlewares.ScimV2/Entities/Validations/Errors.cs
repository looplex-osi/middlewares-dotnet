using System.ComponentModel;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

public enum ErrorType
{
    /* Application errors */
    [Description("Unknown error.")] Unknown = 0,
    [Description("No error.")] None = 1,

    [Description("An internal server error occurred.")]
    InternalServerError = 2,

    /* Middleware errors */
    [Description("Unauthorized access.")] Unauthorized = 200,

    /* Model validation */
    [Description("The property {0} is required.")]
    PropertyRequired = 400,

    [Description("The property {0} is invalid.")]
    PropertyInvalid = 401,

    [Description("The string {0} is required to have min length of {1}.")]
    StringRequiredMinLengh = 402,

    [Description("The string {0} is required to have max length of {1}.")]
    StringRequiredMaxLengh = 403,

    [Description("The string {0} cannot be empty.")]
    StringEmpty = 404,
    
    [Description("The property {0} is an invalid URL.")]
    UrlInvalid = 405,
    
    [Description("The property {0} is an invalid email address.")]
    EmailAddressInvalid = 406,
    
    [Description("The property {0} is an invalid option.")]
    OptionInvalid = 407,
    
    /* Database errors */
    [Description("The entity with id {0} was not found.")]
    NotFound = 600,

    [Description("The entity property {0} must be unique.")]
    NotUnique = 601
}

public static class ErrorTypeExtensions
{
    public static string GetDescription(this ErrorType value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return $"{value:D4} - {attribute?.Description}";
    }
}