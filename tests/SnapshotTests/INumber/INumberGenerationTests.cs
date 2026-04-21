using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.INumber;

public class INumberGenerationTests
{
    [Fact]
    public Task Generates_INumber_for_double_struct()
    {
        string source = """
                        using Vogen;
                        namespace Whatever;
                        [ValueObject<double>(numericsGeneration: NumericsGeneration.GenerateINumberInterfaceAndMethods)]
                        public partial struct Distance { }
                        """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Generates_INumber_for_int_struct()
    {
        string source = """
                        using Vogen;
                        namespace Whatever;
                        [ValueObject<int>(numericsGeneration: NumericsGeneration.GenerateINumberInterfaceAndMethods)]
                        public partial struct Count { }
                        """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Generates_INumber_for_double_class()
    {
        string source = """
                        using Vogen;
                        namespace Whatever;
                        [ValueObject<double>(numericsGeneration: NumericsGeneration.GenerateINumberInterfaceAndMethods)]
                        public partial class Distance { }
                        """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Custom_operator_prevents_duplicate_generation()
    {
        // User provides operator+; the generator must not emit a second one.
        // The body uses `default` to avoid referencing generated members (From/Value)
        // during the initial compilation that the snapshot runner performs.
        string source = """
                        using Vogen;
                        namespace Whatever;
                        [ValueObject<double>(numericsGeneration: NumericsGeneration.GenerateINumberInterfaceAndMethods)]
                        public partial struct Angle
                        {
                            public static Angle operator +(Angle left, Angle right) => default;
                        }
                        """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }
}
