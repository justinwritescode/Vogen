using System.Threading.Tasks;
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
                     using Vogen;

                     public class Ratio<T> : IParsable<Ratio<T>> where T : IParsable<T>
                     {
                         public T Value { get; }
                         public Ratio(T value) => Value = value;

                         public static Ratio<T> Parse(string s, IFormatProvider? provider)
                             => new Ratio<T>(T.Parse(s, provider));

                         public static bool TryParse(string? s, IFormatProvider? provider, out Ratio<T> result)
                         {
                             if (s is not null && T.TryParse(s, provider, out var v))
                             {
                                 result = new Ratio<T>(v);
                                 return true;
                             }
                             result = default!;
                             return false;
                         }
                     }

                     [ValueObject<Ratio<int>>]
                     public partial class ImageAspectRatio
                     {
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }
}
