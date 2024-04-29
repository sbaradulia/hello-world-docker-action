using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateEqualsAndHashCodes
{
    public static string GenerateEqualsForAClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var className = tds.Identifier;
        
        string virtualKeyword = item is { IsRecordClass: true, IsSealed: false } ? "virtual " : " ";

        var ret = item.UserProvidedOverloads.EqualsForWrapper.WasProvided ? string.Empty : 
$$"""

            public {{virtualKeyword}}global::System.Boolean Equals({{className}} other)
            {
              if (ReferenceEquals(null, other))
              {
                  return false;
              }

              // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
              // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
              if(!_isInitialized || !other._isInitialized) return false;

              if (ReferenceEquals(this, other))
              {
                  return true;
              }

              return GetType() == other.GetType() && {{$"global::System.Collections.Generic.EqualityComparer<{item.UnderlyingTypeFullName}>.Default.Equals(Value, other.Value)"}};
            }
""";

        ret += 
$$"""

             public global::System.Boolean Equals({{className}} other, global::System.Collections.Generic.IEqualityComparer<{{className}}> comparer)
             {
                 return comparer.Equals(this, other);
             }

             {{GenerateEqualsForUnderlyingType(item, isReadOnly: false)}}
""";

        if (!item.IsRecordClass)
        {
            ret += $$"""

                      public override global::System.Boolean Equals(global::System.Object obj)
                      {
                          return Equals(obj as {{className}});
                      }
                     """;
        }

        return ret;
    }

    public static string GenerateEqualsForAStruct(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var structName = tds.Identifier;

        var ret = item.UserProvidedOverloads.EqualsForWrapper.WasProvided ? string.Empty :
$$"""
            public readonly global::System.Boolean Equals({{structName}} other)
            {
              // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
              // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
              if(!_isInitialized || !other._isInitialized) return false;

              return {{$"global::System.Collections.Generic.EqualityComparer<{item.UnderlyingTypeFullName}>.Default.Equals(Value, other.Value)"}};
            }
  """;

        ret +=
$$"""

            public global::System.Boolean Equals({{structName}} other, global::System.Collections.Generic.IEqualityComparer<{{structName}}> comparer)
            {
              return comparer.Equals(this, other);
            }

            {{GenerateEqualsForUnderlyingType(item, isReadOnly: true)}}
  """;

        if (!item.IsRecordStruct)
        {
            ret +=
$$"""

            public readonly override global::System.Boolean Equals(global::System.Object obj)
            {
              return obj is {{structName}} && Equals(({{structName}}) obj);
            }
  """;
        }

        return ret;
    }

    public static string GenerateGetHashCodeForAClass(VoWorkItem item)
    {
        if (item.UserProvidedOverloads.HashCodeInfo.WasProvided)
        {
            return string.Empty;
        }
        
        string itemUnderlyingType = item.UnderlyingTypeFullName;

        return
$$"""

            public override global::System.Int32 GetHashCode()
            {
              unchecked // Overflow is fine, just wrap
              {
                  global::System.Int32 hash = (global::System.Int32) 2166136261;
                  hash = (hash * 16777619) ^ GetType().GetHashCode();
                  hash = (hash * 16777619) ^ global::System.Collections.Generic.EqualityComparer<{{itemUnderlyingType}}>.Default.GetHashCode(Value);
                  return hash;
              }
            }
  """;
    }

    public static string GenerateGetHashCodeForAStruct(VoWorkItem item)
    {
        if (item.UserProvidedOverloads.HashCodeInfo.WasProvided)
        {
            return string.Empty;
        }

        string itemUnderlyingType = item.UnderlyingTypeFullName;

        return
$$"""

            public readonly override global::System.Int32 GetHashCode()
            {
              return global::System.Collections.Generic.EqualityComparer<{{itemUnderlyingType}}>.Default.GetHashCode(Value);
            }
  """;
    }

    private static string GenerateEqualsForUnderlyingType(VoWorkItem item, bool isReadOnly)
    {
        string itemUnderlyingType = item.UnderlyingTypeFullName;

        bool isString = item.IsUnderlyingAString;

        string readonlyOrEmpty = isReadOnly ? " readonly" : string.Empty;

        string output = item.UserProvidedOverloads.EqualsForUnderlying.WasProvided ? string.Empty :
$$"""

            public{{readonlyOrEmpty}} global::System.Boolean Equals({{itemUnderlyingType}} primitive)
            {
              return Value.Equals(primitive);
            }

""";

        if(isString)
        {
            output += 
$$"""

            public{{readonlyOrEmpty}} global::System.Boolean Equals({{itemUnderlyingType}} primitive, global::System.StringComparer comparer) 
            {
                return comparer.Equals(Value, primitive);
            }
""";
        }

        return output;
    }

    public static string GenerateStringComparersIfNeeded(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        if (!item.IsUnderlyingAString) return string.Empty;
        
        if(item.StringComparersGeneration != StringComparersGeneration.Generate) return string.Empty;

        return $$"""
                            public static class Comparers
                            {
                                 private class __StringEqualityComparer : global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}>
                                 {
                                     readonly global::System.StringComparer _comparer;
                                 
                                     public __StringEqualityComparer(global::System.StringComparer comparer) {
                                         _comparer = comparer;
                                    }
                                 
                                     public bool Equals({{tds.Identifier}} x, {{tds.Identifier}} y) { 
                                        return _comparer.Equals(x._value, y._value);
                                    }
                                 
                                     public int GetHashCode({{tds.Identifier}} obj) {
                                        return _comparer.GetHashCode();
                                     }
                                 }
                                 
                                 public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> Ordinal =>
                                        new __StringEqualityComparer(global::System.StringComparer.Ordinal); 

                                 public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> OrdinalIgnoreCase =>
                                        new __StringEqualityComparer(global::System.StringComparer.OrdinalIgnoreCase); 

                                 public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> CurrentCulture =>
                                        new __StringEqualityComparer(global::System.StringComparer.CurrentCulture); 

                                 public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> CurrentCultureIgnoreCase =>
                                        new __StringEqualityComparer(global::System.StringComparer.CurrentCultureIgnoreCase); 

                                 public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> InvariantCulture =>
                                        new __StringEqualityComparer(global::System.StringComparer.InvariantCulture); 

                                 public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> InvariantCultureIgnoreCase =>
                                        new __StringEqualityComparer(global::System.StringComparer.InvariantCultureIgnoreCase);
                            } 
                 """;
    }
}
