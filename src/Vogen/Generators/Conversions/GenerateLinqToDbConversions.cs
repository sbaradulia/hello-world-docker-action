﻿using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

internal class GenerateLinqToDbConversions : IGenerateConversion
{
    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        return string.Empty;
    }

    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!IsOurs(item.Conversions))
        {
            return string.Empty;
        }

        string code =
            Templates.TryGetForSpecificType(item.UnderlyingType, "LinqToDbValueConverter") ??
            Templates.GetForAnyType("LinqToDbValueConverter");

        code = code.Replace("VOTYPE", item.VoTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullName);
        
        return code;
    }

    private static bool IsOurs(Vogen.Conversions conversions) => conversions.HasFlag(Vogen.Conversions.LinqToDbValueConverter);
}