﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.JsonNumberCustomizations;

/// <summary>
/// These tests verify that types containing <see cref="Customizations.TreatNumberAsStringInSystemTextJson"/> are written correctly.
/// </summary>
[UsesVerify] 
public class CustomizeNumbersAsStringsInSystemTextJson
{
    public class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in Factory.TypeVariations)
            {
                foreach (string conversion in _conversions)
                {
                    foreach (string underlyingType in Factory.UnderlyingTypes)
                    {
                        var qualifiedType = "public " + type;
                        yield return new object[]
                        {
                            qualifiedType, conversion, underlyingType,
                            CreateClassName(qualifiedType, conversion, underlyingType, customized: true), true
                        };

                        yield return new object[]
                        {
                            qualifiedType, conversion, underlyingType,
                            CreateClassName(qualifiedType, conversion, underlyingType, customized: false), false
                        };
                    }
                }
            }
        }

        private static string CreateClassName(string type, string conversion, string underlyingType, bool customized) =>
            Normalize($"stj_number_as_string_{type}{conversion}{underlyingType}{(customized ? "_customized" : "")}");

        private static string Normalize(string input) => input.Replace(" ", "_").Replace("|", "_").Replace(".", "_");

        // for each of the types above, create classes for each one of these attributes
        private readonly string[] _conversions = new[]
        {
            "Conversions.None",
            "Conversions.NewtonsoftJson",
            "Conversions.SystemTextJson",
            "Conversions.NewtonsoftJson | Conversions.SystemTextJson"
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task GenerationTest(string type, string conversions, string underlyingType, string className, bool treatNumberAsString)
    {
        var customization = treatNumberAsString
            ? "Customizations.TreatNumberAsStringInSystemTextJson"
            : "Customizations.None";

        string declaration = "";
        
        if (underlyingType.Length == 0)
        {
            declaration = $@"
  [ValueObject(conversions: {conversions}, customizations: {customization})]
  {type} {className} {{ }}";
        }
        else
        {
            declaration = $@"
  [ValueObject(conversions: {conversions}, underlyingType: typeof({underlyingType}), customizations: {customization})]
  {type} {className} {{ }}";

        }

        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(className)))
            .RunOnAllFrameworks();
    }
}