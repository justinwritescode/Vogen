using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.INumber;

public class INumberGenerationTests
{
    [Fact]
    public Task Generates_INumberBase_for_Complex_struct()
    {
        string source = """
                        using System.Numerics;
                        using Vogen;
                        namespace Whatever;
                        [ValueObject<Complex>(numericsGeneration: NumericsGeneration.Generate)]
                        public partial struct ComplexQuantity { }
                        """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Generates_INumberBase_for_Complex_class()
    {
        string source = """
                        using System.Numerics;
                        using Vogen;
                        namespace Whatever;
                        [ValueObject<Complex>(numericsGeneration: NumericsGeneration.Generate)]
                        public partial class ComplexQuantity { }
                        """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }


    [Fact]
    public Task Generates_INumber_for_double_struct()
    {
        string source = """
                        using Vogen;
                        namespace Whatever;
                        [ValueObject<double>(numericsGeneration: NumericsGeneration.Generate)]
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
                        [ValueObject<int>(numericsGeneration: NumericsGeneration.Generate)]
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
                        [ValueObject<double>(numericsGeneration: NumericsGeneration.Generate)]
                        public partial class Distance { }
                        """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Generates_INumber_for_uint_struct()
    {
        string source = """
                        using Vogen;
                        namespace Whatever;
                        [ValueObject<uint>(numericsGeneration: NumericsGeneration.Generate)]
                        public partial struct Count { }
                        """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Generates_INumber_for_ulong_struct()
    {
        string source = """
                        using Vogen;
                        namespace Whatever;
                        [ValueObject<ulong>(numericsGeneration: NumericsGeneration.Generate)]
                        public partial struct BigCount { }
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
                        [ValueObject<double>(numericsGeneration: NumericsGeneration.Generate)]
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
