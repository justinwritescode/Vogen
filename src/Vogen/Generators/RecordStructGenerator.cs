﻿using Microsoft.CodeAnalysis;
using Vogen.Generators.Conversions;

namespace Vogen.Generators;

public class RecordStructGenerator : IGenerateValueObjectSourceCode
{
    public string Generate(GenerationParameters parameters)
    {
        var item = parameters.WorkItem;
        var tds = parameters.WorkItem.TypeToAugment;

        SyntaxToken wrapperName = tds.Identifier;

        string itemUnderlyingType = item.UnderlyingTypeFullName;

        string underlyingNullAnnotation = item.Nullable.QuestionMarkForUnderlying;
        string underlyingBang = item.Nullable.BangForUnderlying;
        
        string readonlyForValueAndIsInitialized = Util.GetModifiersForValueAndIsInitializedFields(parameters.WorkItem); 

        var code = GenerateCode();
        
        return item.Nullable.WrapBlock(code);
        
        string GenerateCode() => $@"

{Util.WriteStartNamespace(item.FullNamespace)}
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] 
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Util.GenerateYourAssemblyName()}"", ""{Util.GenerateYourAssemblyVersion()}"")]
    {Util.GenerateAnyConversionAttributes(tds, item)}
    {DebugGeneration.GenerateDebugAttributes(item, wrapperName, itemUnderlyingType)}
    { Util.GenerateModifiersFor(tds)} record struct {wrapperName} : 
        global::System.IEquatable<{wrapperName}>
        {GenerateCodeForEqualsMethodsAndOperators.GenerateInterfaceDefinitionsIfNeeded(", ", item)}
        {GenerateCodeForComparables.GenerateInterfaceDefinitionsIfNeeded(", ", item)}
        {GenerateCodeForIParsableInterfaceDeclarations.GenerateIfNeeded(", ", item)}
        {GenerateCodeForTryFormat.GenerateInterfaceDefinitionsIfNeeded(", ", parameters)}
        {GenerateCodeForStaticAbstracts.GenerateInterfaceDefinitionIfNeeded(", ", item)}
        {GenerateCodeForXmlSerializable.GenerateInterfaceDefinitionIfNeeded(", ", item)}
    {{
{DebugGeneration.GenerateStackTraceFieldIfNeeded(item)}

#if !VOGEN_NO_VALIDATION
        private {readonlyForValueAndIsInitialized} global::System.Boolean _isInitialized;
#endif
        
        private {readonlyForValueAndIsInitialized} {itemUnderlyingType}{underlyingNullAnnotation} _value;

        {Util.GenerateCommentForValueProperty(item)}
        public readonly {itemUnderlyingType} Value
        {{
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {{
                EnsureInitialized();
                return _value{underlyingBang};
            }}
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            init
            {{
                {Util.GenerateNullCheckAndThrowIfNeeded(item)}

                {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

                {GenerateCodeForCallingValidation.CallAndThrowIfRequired(item)}

                _value = value;
            }}
        }}

{GenerateCodeForStaticConstructors.GenerateIfNeeded(item)}
        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public {wrapperName}()
        {{
#if DEBUG
            {DebugGeneration.SetStackTraceIfNeeded(item)}
#endif

#if !VOGEN_NO_VALIDATION
            _isInitialized = false;
#endif
            _value = default;
        }}

        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        private {wrapperName}({itemUnderlyingType} value) 
        {{
            _value = value;
#if !VOGEN_NO_VALIDATION
            _isInitialized = true;
#endif
        }}

        /// <summary>
        /// Builds an instance from the provided underlying type.
        /// </summary>
        /// <param name=""value"">The underlying type.</param>
        /// <returns>An instance of this type.</returns>
        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static {wrapperName} From({itemUnderlyingType} value)
        {{
            {Util.GenerateNullCheckAndThrowIfNeeded(item, generateReturnDefault: true)}

            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {GenerateCodeForCallingValidation.CallAndThrowIfRequired(item)}

            return new {wrapperName}(value);
        }}

        {GenerateCodeForTryFrom.GenerateForAStruct(item, wrapperName, itemUnderlyingType)}

{Util.GenerateIsInitializedMethod(@readonly: true, item)}

{Util.GenerateLinqPadDump(item)}

{GenerateCodeForStringComparers.GenerateIfNeeded(item, tds)}        

{GenerateCodeForCastingOperators.GenerateImplementations(parameters,tds)}{Util.GenerateGuidFactoryMethodIfNeeded(item)}
        // only called internally when something has been deserialized into
        // its primitive type.
        private static {wrapperName} __Deserialize({itemUnderlyingType} value)
        {{
            {Util.GenerateNullCheckAndThrowIfNeeded(item, generateReturnDefault: true)}

            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateChecksForKnownInstancesIfRequired(item)}

            {GenerateCodeForCallingValidation.CallWhenDeserializingAndCheckStrictnessFlag(item)}

            return new {wrapperName}(value);
        }}
        {GenerateCodeForEqualsMethodsAndOperators.GenerateEqualsMethodsForAStruct(item, tds)}

        {GenerateCodeForEqualsMethodsAndOperators.GenerateEqualsOperatorsForPrimitivesIfNeeded(itemUnderlyingType, wrapperName, item)}

        {GenerateCodeForComparables.GenerateIComparableImplementationIfNeeded(item, tds)}

        {GenerateCodeForTryParse.GenerateAnyHoistedTryParseMethods(item)}{GenerateCodeForParse.GenerateAnyHoistedParseMethods(item)}

        {GenerateCodeForTryFormat.GenerateAnyHoistedTryFormatMethods(parameters)}

        {GenerateCodeForHashCodes.GenerateForAStruct(item)}

        // record enumerates fields - we just want our Value and to throw if it's not initialized.
        {GenerateCodeForToString.GenerateForAStruct(parameters)}

        {Util.GenerateEnsureInitializedMethod(item, readOnly: true)}

        {InstanceGeneration.GenerateAnyInstances(tds, item)}

        {Util.GenerateAnyConversionBodies(tds, item)}

        {GenerateCodeForXmlSerializable.GenerateBodyIfNeeded(parameters)}

        {Util.GenerateDebuggerProxyForStructs(item)}

        {Util.GenerateThrowHelper(item)}
}}

{GenerateEfCoreExtensions.GenerateInnerIfNeeded(item)}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }
}