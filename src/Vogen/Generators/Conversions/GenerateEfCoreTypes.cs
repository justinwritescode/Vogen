using Microsoft.CodeAnalysis;

namespace Vogen.Generators.Conversions;

internal static class GenerateEfCoreTypes
{
    public static string GenerateInner(INamedTypeSymbol primitiveType, bool isWrapperAValueType, INamedTypeSymbol voSymbol) =>
        Generate(isWritingAsInnerClass: true, primitiveType, isWrapperAValueType, voSymbol);

    public static string GenerateBodyForAMarkerClass(ConversionMarker markerClass) => 
        Generate(isWritingAsInnerClass: false, markerClass.UnderlyingTypeSymbol, markerClass.VoSymbol.IsValueType,  markerClass.VoSymbol);

    private static string Generate(bool isWritingAsInnerClass, INamedTypeSymbol primitiveType, bool isWrapperAValueType, INamedTypeSymbol voSymbol)
    {
        string sectionToCut = isWritingAsInnerClass ? "__WHEN_OUTER__" : "__WHEN_INNER__";
        string sectionToKeep = isWritingAsInnerClass ? "__WHEN_INNER__" : "__WHEN_OUTER__";
        string classPrefix = isWritingAsInnerClass ? string.Empty : voSymbol.Name;
        string voTypeName = isWritingAsInnerClass ? voSymbol.Name : voSymbol.EscapedFullName();
        
        var isKnownPrimitive = Templates.IsKnownPrimitive(primitiveType);

        var primitiveFullName =  primitiveType.EscapedFullName();

        string code = isKnownPrimitive ? _converterForKnownTypes : _converterForAnyOtherType;

        if (isWrapperAValueType)
        {
            code += _comparerForValuesTypes;
        }
        else
        {
            code += _comparerForReferenceTypes;
        }

        code = CodeSections.CutSection(code, sectionToCut);
        code = CodeSections.KeepSection(code, sectionToKeep);

        code = code.Replace("__CLASS_PREFIX__", classPrefix);
        code = code.Replace("__NEEDS_REF__", isWrapperAValueType ? "ref" : string.Empty);
        code = code.Replace("VOTYPE", voTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", primitiveFullName);

        return code;
    }

    public static string GenerateMarkerExtensionMethod(ConversionMarker marker)
    {
        var voSymbol = marker.VoSymbol;
        var markerClassSymbol = marker.MarkerClassSymbol;

        string voTypeName = voSymbol.EscapedFullName();
        
        string code = _extensionMethodForOuter;
        
        var isPublic = markerClassSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        
        string generatedConverter = $"{markerClassSymbol.FullNamespace()}.{markerClassSymbol.Name}.{voSymbol.Name}EfCoreValueConverter";
        string generatedComparer = $"{markerClassSymbol.FullNamespace()}.{markerClassSymbol.Name}.{voSymbol.Name}EfCoreValueComparer";

        code = code.Replace("__CLASS_PREFIX__", voSymbol.Name);
        code = code.Replace("__GENERATED_CONVERTER_NAME__", generatedConverter);
        code = code.Replace("__GENERATED_COMPARER_NAME__", generatedComparer);
        code = code.Replace("__ACCESSIBILITY_OF_MARKER_CLASS__", accessor);
        code = code.Replace("VOTYPE", voTypeName);

        return code;
    }
    
    private const string _extensionMethodForOuter = 
        """
        __ACCESSIBILITY_OF_MARKER_CLASS__ static class __CLASS_PREFIX____Ext 
        {
                __ACCESSIBILITY_OF_MARKER_CLASS__ static global::Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<VOTYPE> HasVogenConversion(this global::Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<VOTYPE> propertyBuilder) =>
                    propertyBuilder.HasConversion<__GENERATED_CONVERTER_NAME__, __GENERATED_COMPARER_NAME__>();
        }
        
        """;

    private const string _converterForKnownTypes = 
        """
                public class __CLASS_PREFIX__EfCoreValueConverter : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, VOUNDERLYINGTYPE>
                {
                  public __CLASS_PREFIX__EfCoreValueConverter() : this(null) { }
                  public __CLASS_PREFIX__EfCoreValueConverter(global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                      : base(
                          vo => vo.Value,
        __WHEN_INNER__                                                                      value => VOTYPE.__Deserialize(value),
        __WHEN_OUTER__                                                                      value => Deserialize(value),
                        mappingHints
                      ) { }
        
        __WHEN_OUTER__      static VOTYPE Deserialize(VOUNDERLYINGTYPE value) => UnsafeDeserialize(default, value);
        __WHEN_OUTER__ 
        __WHEN_OUTER__      [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
        __WHEN_OUTER__      static extern VOTYPE UnsafeDeserialize(VOTYPE @this, VOUNDERLYINGTYPE value);      
                }
        """;

    private const string _converterForAnyOtherType =
        """
        
            public class __CLASS_PREFIX__EfCoreValueConverter : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, global::System.String>
            {
                public __CLASS_PREFIX__EfCoreValueConverter() : this(null) { }
                public __CLASS_PREFIX__EfCoreValueConverter(global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                    : base(
                        vo => global::System.Text.Json.JsonSerializer.Serialize(vo.Value, default(global::System.Text.Json.JsonSerializerOptions)),
        __WHEN_INNER__            text => VOTYPE.__Deserialize(global::System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(text, default(global::System.Text.Json.JsonSerializerOptions))),
        __WHEN_OUTER__            text => ProxyForCall__Deserialize(global::System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(text, default(global::System.Text.Json.JsonSerializerOptions))),
                        mappingHints
                    ) { }
        __WHEN_OUTER__  static VOTYPE ProxyForCall__Deserialize(VOUNDERLYINGTYPE value) => Call__Deserialize(default, value);
        __WHEN_OUTER__ 
        __WHEN_OUTER__  [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
        __WHEN_OUTER__  static extern VOTYPE Call__Deserialize(VOTYPE @this, VOUNDERLYINGTYPE value);      
            }
        """;
    
    private const string _comparerForValuesTypes =
        """
        
                public class __CLASS_PREFIX__EfCoreValueComparer : global::Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<VOTYPE>
                {
                    public __CLASS_PREFIX__EfCoreValueComparer() : base(
                        (left, right) => DoCompare(left, right), 
                        instance => instance.IsInitialized() ? instance.GetHashCode() : 0) 
                    { 
                    }
                    
                    static bool DoCompare(VOTYPE left, VOTYPE right)
                    {
                        // if neither are initialized, then they're equal
                        if(!left.IsInitialized() && !right.IsInitialized()) return true;
                        
        __WHEN_INNER__                return left.IsInitialized() && right.IsInitialized() && left._value.Equals(right._value);
        __WHEN_OUTER__                return left.IsInitialized() && right.IsInitialized() && UnderlyingValue(left).Equals(UnderlyingValue(right));
                    }
        __WHEN_OUTER__ private static VOUNDERLYINGTYPE UnderlyingValue(VOTYPE i) => UnsafeValueField(__NEEDS_REF__ i);
        __WHEN_OUTER__
        __WHEN_OUTER__  [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_value")]
        __WHEN_OUTER__  static extern ref VOUNDERLYINGTYPE UnsafeValueField(__NEEDS_REF__ VOTYPE @this);                
                }
        """;

    private const string _comparerForReferenceTypes = $$"""
                                                        
                                                               public class __CLASS_PREFIX__EfCoreValueComparer : global::Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<VOTYPE>
                                                               {
                                                                   public __CLASS_PREFIX__EfCoreValueComparer() : base(
                                                                       (left, right) => DoCompare(left, right), 
                                                        __WHEN_INNER__                instance => instance.IsInitialized() ? instance._value.GetHashCode() : 0) 
                                                        __WHEN_OUTER__                instance => instance.IsInitialized() ? UnderlyingValue(instance).GetHashCode() : 0) 
                                                                   { 
                                                                   }
                                                                       
                                                                   static bool DoCompare(VOTYPE left, VOTYPE right)
                                                                   {
                                                                       // if both null, then they're equal
                                                                       if (left is null) return right is null;
                                                                       
                                                                       // if only right is null, then they're not equal
                                                                       if (right is null) return false;
                                                                       
                                                                       // if they're both the same reference, then they're equal
                                                                       if (ReferenceEquals(left, right)) return true;
                                                                       
                                                                       // if neither are initialized, then they're equal
                                                                        if(!left.IsInitialized() && !right.IsInitialized()) return true;
                                                                       
                                                        __WHEN_INNER__                return left.IsInitialized() && right.IsInitialized() && left._value.Equals(right._value);            
                                                        __WHEN_OUTER__                return left.IsInitialized() && right.IsInitialized() && UnderlyingValue(left).Equals(UnderlyingValue(right));            
                                                                   }                
                                                        __WHEN_OUTER__ private static VOUNDERLYINGTYPE UnderlyingValue(VOTYPE i) => UnsafeValueField(__NEEDS_REF__ i);
                                                        __WHEN_OUTER__
                                                        __WHEN_OUTER__  [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_value")]
                                                        __WHEN_OUTER__  static extern ref VOUNDERLYINGTYPE UnsafeValueField(__NEEDS_REF__ VOTYPE @this);                
                                                        }
                                                        """;        

}