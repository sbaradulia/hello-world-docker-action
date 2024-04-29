﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Diagnostics;

internal static class DiagnosticsCatalogue
{
    private static readonly DiagnosticDescriptor _typeCannotBeNested = CreateDescriptor(
        RuleIdentifiers.TypeCannotBeNested,
        "Types cannot be nested",
        "Type '{0}' cannot be nested - remove it from inside {1}");

    private static readonly DiagnosticDescriptor _typeCannotBeAbstract = CreateDescriptor(
        RuleIdentifiers.TypeCannotBeAbstract,
        "Types cannot be abstract",
        "Type '{0}' cannot be abstract");

    private static readonly DiagnosticDescriptor _recordToStringOverloadShouldBeSealed = CreateDescriptor(
        RuleIdentifiers.RecordToStringOverloadShouldBeSealed,
        "Overrides of ToString on records should be sealed to differentiate it from the C# compiler-generated method. See https://github.com/SteveDunn/Vogen/wiki/Records#tostring for more information.",
        "ToString overrides should be sealed on records. See https://github.com/SteveDunn/Vogen/wiki/Records#tostring for more information.");

    private static readonly DiagnosticDescriptor _typeShouldBePartial = CreateDescriptor(
        RuleIdentifiers.TypeShouldBePartial,
        "Value Objects should be declared in partial types.",
        "Type {0} is decorated as a Value Object and should be in a partial type.");

    private static readonly DiagnosticDescriptor _duplicateTypesFound = CreateDescriptor(
        RuleIdentifiers.DuplicateTypesFound,
        "Duplicate Value Object found.",
        "Type {0} is decorated as a Value Object but is declared multiple times. Remove the duplicate definition or differentiate with a namespace.");

    private static readonly DiagnosticDescriptor _cannotHaveUserConstructors = CreateDescriptor(
        RuleIdentifiers.CannotHaveUserConstructors,
        "Cannot have user defined constructors",
        "Cannot have user defined constructors, please use the From method for creation.");

    private static readonly DiagnosticDescriptor _underlyingTypeCannotBeCollection = CreateDescriptor(
        RuleIdentifiers.UnderlyingTypeCannotBeCollection,
        "Underlying type cannot be collection",
        "Type '{0}' has an underlying type of {1} which is not valid");

    private static readonly DiagnosticDescriptor _invalidConversions = CreateDescriptor(
        RuleIdentifiers.InvalidConversions,
        "Invalid Conversions",
        "The Conversions specified do not match any known conversions - see the Conversions type");

    private static readonly DiagnosticDescriptor _invalidCustomizations = CreateDescriptor(
        RuleIdentifiers.InvalidCustomizations,
        "Invalid Customizations",
        "The Customizations specified do not match any known customizations - see the Customizations type");

    private static readonly DiagnosticDescriptor _invalidDeserializationStrictness = CreateDescriptor(
        RuleIdentifiers.InvalidDeserializationStrictness,
        "Invalid Deserialization Strictness",
        $"The Deserialization Strictness specified does not match any known customizations - see the {nameof(DeserializationStrictness)} type for valid values");

    private static readonly DiagnosticDescriptor _mismatchedParsableGeneration = CreateDescriptor(
        RuleIdentifiers.InvalidDeserializationStrictness,
        "Invalid Parsable Generation",
        $"The Parsable Generation specified does not match the underlying primitive");

    private static readonly DiagnosticDescriptor _underlyingTypeMustNotBeSameAsValueObject = CreateDescriptor(
        RuleIdentifiers.UnderlyingTypeMustNotBeSameAsValueObject,
        "Invalid underlying type",
        "Type '{0}' has the same underlying type - must specify a primitive underlying type");

    private static readonly DiagnosticDescriptor _validationMustReturnValidationType = CreateDescriptor(
        RuleIdentifiers.ValidationMustReturnValidationType,
        "Validation returns incorrect type",
        "{0} must return a Validation type");

    private static readonly DiagnosticDescriptor _normalizeInputMethodMustReturnSameUnderlyingType = CreateDescriptor(
        RuleIdentifiers.NormalizeInputMethodMustReturnSameUnderlyingType,
        "NormalizeInput returns incorrect type",
        "{0} must return the same underlying type");
    
    private static readonly DiagnosticDescriptor _normalizeInputMethodTakeOneParameterOfUnderlyingType = CreateDescriptor(
        RuleIdentifiers.NormalizeInputMethodTakeOneParameterOfUnderlyingType,
        "NormalizeInput accepts wrong type",
        "{0} must accept one parameter of the same type as the underlying type");

    private static readonly DiagnosticDescriptor _validationMustBeStatic = CreateDescriptor(
        RuleIdentifiers.ValidationMustBeStatic,
        "Validation must be static",
        "{0} must be static");

    private static readonly DiagnosticDescriptor _normalizeInputMethodMustBeStatic = CreateDescriptor(
        RuleIdentifiers.NormalizeInputMethodMustBeStatic,
        "NormalizeInput must be static",
        "{0} must be static");

    private static readonly DiagnosticDescriptor _instanceMethodCannotHaveNullArgumentName = CreateDescriptor(
        RuleIdentifiers.InstanceMethodCannotHaveNullArgumentName,
        "Instance attribute cannot have null name",
        "{0} cannot have a null name");

    private static readonly DiagnosticDescriptor _instanceMethodCannotHaveNullArgumentValue = CreateDescriptor(
        RuleIdentifiers.InstanceMethodCannotHaveNullArgumentValue,
        "Instance attribute cannot have null value",
        "{0} cannot have a null value");

    private static readonly DiagnosticDescriptor _instanceValueCannotBeConverted = CreateDescriptor(
        RuleIdentifiers.InstanceValueCannotBeConverted,
        "Instance attribute has value that cannot be converted",
        "{0} cannot be converted. {1}");

    private static readonly DiagnosticDescriptor _customExceptionMustDeriveFromException = CreateDescriptor(
        RuleIdentifiers.CustomExceptionMustDeriveFromException,
        "Invalid custom exception",
        "{0} must derive from System.Exception");

    private static readonly DiagnosticDescriptor _customExceptionMustHaveValidConstructor = CreateDescriptor(
        RuleIdentifiers.CustomExceptionMustHaveValidConstructor,
        "Invalid custom exception",
        "{0} must have at least 1 public constructor with 1 parameter of type System.String");

    public static Diagnostic TypeCannotBeNested(INamedTypeSymbol typeModel, INamedTypeSymbol container) => 
        Create(_typeCannotBeNested, typeModel.Locations, typeModel.Name, container.Name);

    public static Diagnostic TypeCannotBeAbstract(INamedTypeSymbol typeModel) => 
        Create(_typeCannotBeAbstract, typeModel.Locations, typeModel.Name);

    public static Diagnostic ValidationMustReturnValidationType(MethodDeclarationSyntax member) => 
        Create(_validationMustReturnValidationType, member.GetLocation(), member.Identifier);

    public static Diagnostic NormalizeInputMethodMustReturnUnderlyingType(MethodDeclarationSyntax member) => 
        Create(_normalizeInputMethodMustReturnSameUnderlyingType, member.GetLocation(), member.Identifier);

    public static Diagnostic NormalizeInputMethodTakeOneParameterOfUnderlyingType(MethodDeclarationSyntax member) => 
        Create(_normalizeInputMethodTakeOneParameterOfUnderlyingType, member.GetLocation(), member.Identifier);

    public static Diagnostic ValidationMustBeStatic(MethodDeclarationSyntax member) => 
        Create(_validationMustBeStatic, member.GetLocation(), member.Identifier);

    public static Diagnostic NormalizeInputMethodMustBeStatic(MethodDeclarationSyntax member) => 
        Create(_normalizeInputMethodMustBeStatic, member.GetLocation(), member.Identifier);

    public static Diagnostic RecordToStringOverloadShouldBeSealed(Location location, string voClassName) => 
        BuildDiagnostic(_recordToStringOverloadShouldBeSealed, voClassName, location);

    public static Diagnostic TypeShouldBePartial(Location location, string voClassName) => 
        BuildDiagnostic(_typeShouldBePartial, voClassName, location);

    // Just in case the 'AllowMultiple' is taken off the attribute definition.
    public static Diagnostic DuplicateTypesFound(Location location, string voClassName) => 
        BuildDiagnostic(_duplicateTypesFound, voClassName, location);

    public static Diagnostic CannotHaveUserConstructors(IMethodSymbol constructor) => 
        Create(_cannotHaveUserConstructors, constructor.Locations);

    public static Diagnostic UnderlyingTypeMustNotBeSameAsValueObjectType(INamedTypeSymbol underlyingType) => 
        Create(_underlyingTypeMustNotBeSameAsValueObject, underlyingType.Locations, underlyingType.Name);

    public static Diagnostic UnderlyingTypeCannotBeCollection(INamedTypeSymbol voClass, INamedTypeSymbol underlyingType) => 
        Create(_underlyingTypeCannotBeCollection, voClass.Locations, voClass.Name, underlyingType);

    public static Diagnostic InvalidConversions(Location location) => Create(_invalidConversions, location);
    
    public static Diagnostic InvalidCustomizations(Location location) => Create(_invalidCustomizations, location);
    
    public static Diagnostic InvalidDeserializationStrictness(Location location) => Create(_invalidDeserializationStrictness, location);
    
    public static Diagnostic InvalidParsableGeneration(Location location) => Create(_mismatchedParsableGeneration, location);

    public static Diagnostic InstanceMethodCannotHaveNullArgumentName(INamedTypeSymbol voClass) => 
        Create(_instanceMethodCannotHaveNullArgumentName, voClass.Locations, voClass.Name);

    public static Diagnostic InstanceMethodCannotHaveNullArgumentValue(INamedTypeSymbol voClass) => 
        Create(_instanceMethodCannotHaveNullArgumentValue, voClass.Locations, voClass.Name);

    public static Diagnostic InstanceValueCannotBeConverted(INamedTypeSymbol voClass, string message) => 
        Create(_instanceValueCannotBeConverted, voClass.Locations, voClass.Name, message);

    public static Diagnostic CustomExceptionMustDeriveFromException(INamedTypeSymbol symbol) => 
        Create(_customExceptionMustDeriveFromException, symbol.Locations, symbol.Name);

    public static Diagnostic CustomExceptionMustHaveValidConstructor(INamedTypeSymbol symbol) => 
        Create(_customExceptionMustHaveValidConstructor, symbol.Locations, symbol.Name);

    private static DiagnosticDescriptor CreateDescriptor(string code, string title, string messageFormat, DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        string[] tags = severity == DiagnosticSeverity.Error ? new[] { WellKnownDiagnosticTags.NotConfigurable } : Array.Empty<string>();

        return new DiagnosticDescriptor(code, title, messageFormat, "Vogen", severity, isEnabledByDefault: true, customTags: tags);
    }

    public static Diagnostic BuildDiagnostic(DiagnosticDescriptor descriptor, string name, Location location) => 
        Diagnostic.Create(descriptor, location, name);

    public static Diagnostic BuildDiagnostic(DiagnosticDescriptor descriptor, string name) => 
        Diagnostic.Create(descriptor, null, name);

    private static Diagnostic Create(DiagnosticDescriptor descriptor, IEnumerable<Location> locations, params object?[] args)
    {
        var locationsList = (locations as IReadOnlyList<Location>) ?? locations.ToList();

        Diagnostic diagnostic = Diagnostic.Create(
            descriptor, 
            locationsList.Count == 0 ? Location.None : locationsList[0],
            locationsList.Skip(1), 
            args);

        return diagnostic;
    }

    private static Diagnostic Create(DiagnosticDescriptor descriptor, Location? location, params object?[] args) =>
        Diagnostic.Create(descriptor, location ?? Location.None, args);
}