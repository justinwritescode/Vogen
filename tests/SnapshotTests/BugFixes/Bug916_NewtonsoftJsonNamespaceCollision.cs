using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/916
// Bug: Newtonsoft.Json converters were missing global:: prefix, causing namespace collision
// when users had a namespace named Newtonsoft.Json in their project.
// Fix: All Newtonsoft.Json type references now use global:: prefix
public class Bug916_NewtonsoftJsonNamespaceCollision
{
    [Fact]
    public async Task String_ValueObject_With_Newtonsoft_Conversion_And_Instances()
    {
        var source = """
                     using Vogen;

                     [ValueObject(typeof(string), conversions: Conversions.NewtonsoftJson)]
                     [Instance("Eur", "EUR")]
                     [Instance("Gbp", "GBP")]
                     public readonly partial struct Currency
                     {
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }

    [Fact]
    public async Task With_Namespace_Collision_Local_Newtonsoft_Json_Namespace()
    {
        // This test verifies the fix for namespace collision.
        // The generated converters must use `global::Newtonsoft.Json.JsonConverter` and related types
        // to avoid resolving to the local `Newtonsoft.Json` namespace defined below.
        var source = """
                     using Vogen;

                     namespace MyApp.Newtonsoft
                     {
                         namespace Json
                         {
                             // Empty namespace - this should NOT interfere with global Newtonsoft.Json types
                         }
                     }

                     namespace MyApp
                     {
                         [ValueObject(typeof(string), conversions: Conversions.NewtonsoftJson)]
                         [Instance("Eur", "EUR")]
                         [Instance("Gbp", "GBP")]
                         public readonly partial struct Currency
                         {
                         }
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }

    [Fact]
    public async Task Multiple_Types_With_Newtonsoft_Conversion()
    {
        var source = """
                     using Vogen;

                     [ValueObject(typeof(string), conversions: Conversions.NewtonsoftJson)]
                     public readonly partial struct UserId
                     {
                     }

                     [ValueObject(typeof(int), conversions: Conversions.NewtonsoftJson)]
                     public readonly partial struct ProductId
                     {
                     }

                     [ValueObject(typeof(decimal), conversions: Conversions.NewtonsoftJson)]
                     public readonly partial struct Price
                     {
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }
}
