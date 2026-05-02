using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/907
// Bug: When the underlying type is a generic type (e.g. Ratio<int>), the generated
// <inheritdoc cref="..."/> for Parse/TryParse methods used concrete type arguments
// (Ratio{int}) instead of type parameters (Ratio{T}), causing CS1584/CS1658 warnings.
public class Bug907_GenericUnderlyingTypeInheritDocWarnings
{
    [Fact]
    public async Task Generic_underlying_type_does_not_generate_invalid_cref_in_parse_methods()
    {
        var source = """
                     #nullable enable
                     using System;
                     using System.Numerics;
                     using System.Diagnostics.CodeAnalysis;
                     using Vogen;
                     
                     /// <summary></summary>
                     [ValueObject<Ratio<int>>]
                     public readonly partial struct ImageAspectRatio;
                     
                     /// <summary></summary>
                     public readonly record struct Ratio<T>(
                     	T Numerator,
                     	T Denominator
                     ) : ISpanParsable<Ratio<T>>
                     	where T : IBinaryInteger<T>
                     {
                     	/// <summary></summary>
                     	public static Ratio<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
                     		throw new NotImplementedException();
                     
                     	/// <summary></summary>
                     	public static Ratio<T> Parse(string s, IFormatProvider? provider) =>
                     		throw new NotImplementedException();
                     
                     	/// <summary></summary>
                     	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Ratio<T> result) =>
                     		throw new NotImplementedException();
                     
                     	/// <summary></summary>
                     	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Ratio<T> result) =>
                     		throw new NotImplementedException();
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOn(TargetFramework.AspNetCore10_0);
    }
}
