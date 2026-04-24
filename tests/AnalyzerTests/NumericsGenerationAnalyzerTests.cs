using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;

namespace AnalyzerTests;

public class NumericsGenerationAnalyzerTests
{
    [Fact]
    public async Task Emits_VOG037_when_underlying_type_does_not_implement_INumberBase()
    {
        var source = """
                     using Vogen;
                     namespace Whatever;
                     [ValueObject<string>(numericsGeneration: NumericsGeneration.Generate)]
                     public partial class Name { }
                     """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            var vog037 = diagnostics.Where(d => d.Id == "VOG037").ToList();
            vog037.Should().HaveCount(1);
            vog037[0].Severity.Should().Be(DiagnosticSeverity.Warning);
            vog037[0].GetMessage().Should().Contain("Name").And.Contain("String");
        }
    }

    [Fact]
    public async Task Does_not_emit_VOG037_for_numeric_underlying_type()
    {
        var source = """
                     using Vogen;
                     namespace Whatever;
                     [ValueObject<double>(numericsGeneration: NumericsGeneration.Generate)]
                     public partial struct Distance { }
                     """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(diagnostics =>
                diagnostics.Where(d => d.Id == "VOG037").Should().BeEmpty())
            .RunOnAllFrameworks();
    }

    [Fact]
    public async Task Does_not_emit_VOG037_for_Complex_underlying_type()
    {
        var source = """
                     using System.Numerics;
                     using Vogen;
                     namespace Whatever;
                     [ValueObject<Complex>(numericsGeneration: NumericsGeneration.Generate)]
                     public partial struct ComplexQuantity { }
                     """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(diagnostics =>
                diagnostics.Where(d => d.Id == "VOG037").Should().BeEmpty())
            .RunOnAllFrameworks();
    }
}
