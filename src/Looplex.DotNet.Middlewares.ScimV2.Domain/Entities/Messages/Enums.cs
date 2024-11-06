namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;

/// <summary>
/// The HTTP method of the current operation.
/// </summary>
public enum Method
{
    Delete, 
    Patch, 
    Post, 
    Put
};

public enum ErrorScimType
{
    InvalidFilter, 
    InvalidPath, 
    InvalidSyntax, 
    InvalidValue, 
    InvalidVers, 
    Mutability, 
    NoTarget, 
    Sensitive, 
    TooMany, 
    Uniqueness
};
