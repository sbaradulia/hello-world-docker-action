﻿namespace Vogen.Tests.Types;

[ValueObject(typeof(string))]
public partial class MyString
{
    private static Validation Validate(string value)
    {
        if (value.Length > 0)
            return Validation.Ok;
        
        return Validation.Invalid("length must be greater than zero");
    }
}