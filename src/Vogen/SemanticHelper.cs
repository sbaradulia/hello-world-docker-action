﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

static class SemanticHelper
{
    public static string? FullName(this INamedTypeSymbol? symbol)
    {
        if (symbol is null)
            return null;

        var prefix = FullNamespace(symbol);
        var suffix = "";
        
        if (symbol.Arity > 0)
        {
            suffix = $"<{string.Join(", ", symbol.TypeArguments.Select(a => FullName(a as INamedTypeSymbol)))}>";
        }

        if (prefix != "")
            return $"{prefix}.{Util.EscapeIfRequired(symbol.Name)}{suffix}";
        else
            return Util.EscapeIfRequired(symbol.Name) + suffix;
    }

    public static string FullNamespace(this ISymbol symbol)
    {
        var parts = new Stack<string>();
        INamespaceSymbol? iterator = (symbol as INamespaceSymbol) ?? symbol.ContainingNamespace;
        while (iterator is not null)
        {
            if (!string.IsNullOrEmpty(iterator.Name))
            {
                parts.Push(Util.EscapeIfRequired(iterator.Name));
            }

            iterator = iterator.ContainingNamespace;
        }

        return string.Join(".", parts);
    }

    public static bool HasDefaultConstructor(this INamedTypeSymbol symbol)
    {
        return symbol.Constructors.Any(c => c.Parameters.Count() == 0);
    }

    public static IEnumerable<IPropertySymbol> ReadWriteScalarProperties(this INamedTypeSymbol symbol)
    {
        return symbol.GetMembers().OfType<IPropertySymbol>()
            .Where(p => p.CanRead() && p.CanWrite() && !p.HasParameters());
    }

    public static IEnumerable<IPropertySymbol> ReadableScalarProperties(this INamedTypeSymbol symbol)
    {
        return symbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.CanRead() && !p.HasParameters());
    }

    public static IEnumerable<IPropertySymbol> WritableScalarProperties(this INamedTypeSymbol symbol)
    {
        return symbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.CanWrite() && !p.HasParameters());
    }

    public static bool CanRead(this IPropertySymbol symbol) => symbol.GetMethod is not null;

    public static bool CanWrite(this IPropertySymbol symbol) => symbol.SetMethod is not null;

    public static bool HasParameters(this IPropertySymbol symbol) => symbol.Parameters.Any();

    public static bool ImplementsInterfaceOrBaseClass(this INamedTypeSymbol? typeSymbol, Type typeToCheck)
    {
        if (typeSymbol is null)
        {
            return false;
        }

        if (typeSymbol.MetadataName == typeToCheck.Name)
        {
            return true;
        }

        if (typeSymbol.BaseType?.MetadataName == typeToCheck.Name)
        {
            return true;
        }

        foreach (INamedTypeSymbol? @interface in typeSymbol.AllInterfaces)
        {
            if (@interface.MetadataName == typeToCheck.Name)
            {
                return true;
            }
        }

        if (typeSymbol.BaseType is not null)
        {
            return ImplementsInterfaceOrBaseClass(typeSymbol.BaseType, typeToCheck);
        }

        return false;
    }
}
