﻿using System.Threading.Tasks;
using Shared;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.GeneralStuff;

[UsesVerify]
public class GeneralTests
{
    [Fact]
    public async Task EFCore_generated_stuff()
    {
        var source = @"using Vogen;
  [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(string))]
  public partial struct MyVo { }
";

        await RunTest(source);
    }

    [Fact]
    public async Task User_supplied_GetHashCode()
    {
        var source = @"using Vogen;
[ValueObject(typeof(string))]
public readonly partial record struct VoWithGetHashCode
{
    public override int GetHashCode()
    {
        // custom code
        return 0;
    }
}
";

        await RunTest(source);
    }

    [Fact]
    public async Task User_supplied_Equals()
    {
        var source = @"using Vogen;
[ValueObject(typeof(string))]
public readonly partial record struct MyVo
{
    public readonly bool Equals(MyVo other)
    {
        // custom code
        return Value == other.Value;
    }

    public readonly bool Equals(string other)
    {
        // custom code
        return Value == other;
    }
}
";

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }

    [SkippableFact]
    public async Task User_supplied_Equals_generic()
    {
        var source = @"using Vogen;
[ValueObject<string>]
public readonly partial record struct MyVo
{
    public readonly bool Equals(MyVo other)
    {
        // custom code
        return Value == other.Value;
    }

    public readonly bool Equals(string other)
    {
        // custom code
        return Value == other;
    }
}
";

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOn(TargetFramework.Net7_0);
    }

    [Fact]
    public async Task Bug_fix_516_user_can_provide_Equals_and_GetHashCode()
    {
        var source = @"using Vogen;
[ValueObject(typeof(string))]
public readonly partial record struct BugFix516Vo
{
    public readonly bool Equals(BugFix516Vo other)
    {
        // custom code
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        // custom code
        return 0;
    }
}";

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Partial_record_struct_created_successfully()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(typeof(string))]
public readonly partial record struct CustomerId
{
}";

        return RunTest(source);
    }

    [Fact]
    public Task No_stack_trace_recording()
    {
        var source = @"using Vogen;
[assembly: VogenDefaults(disableStackTraceRecordingInDebug: true)]

namespace Whatever;

[ValueObject(typeof(string))]
public readonly partial record struct CustomerId
{
}";

        return RunTest(source);
    }

    [Fact]
    public Task No_casting()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(typeof(string), toPrimitiveCasting: CastOperator.Implicit, fromPrimitiveCasting: CastOperator.None)]
public partial class MyVo { }
";

        return RunTest(source);
    }

    [Fact]
    public Task Partial_struct_created_successfully()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
}";

        return RunTest(source);
    }

    [Fact]
    public Task No_namespace() =>
        RunTest(@"using Vogen;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
}");


    [Fact]
    public Task Produces_instances() =>
        RunTest(@"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
[Instance(name: ""Unspecified"", value: -1, tripleSlashComment: ""a short description that'll show up in intellisense"")]
[Instance(name: ""Unspecified1"", value: -2)]
[Instance(name: ""Unspecified2"", value: -3, tripleSlashComment: ""<some_xml>whatever</some_xml"")]
[Instance(name: ""Unspecified3"", value: -4)]
[Instance(name: ""Cust42"", value: 42)]
public partial struct CustomerId
{
}
");

    [Fact]
    public Task Validation_with_PascalCased_validate_method() =>
        RunTest(@"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}
");

    [Fact]
    public Task Validation_with_camelCased_validate_method() =>
        RunTest(@"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}
");

    [Fact]
    public Task Validation_can_have_fully_qualified_return_type() =>
        RunTest(@"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static Vogen.Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}
");


    [Fact]
    public Task Namespace_names_can_have_reserved_keywords() =>
        RunTest(@"using Vogen;

namespace @double;

[ValueObject]
[Instance(name: ""@struct"", value: 42)]
[Instance(name: ""@double"", value: 52)]
[Instance(name: ""@event"", value: 69)]
[Instance(name: ""@void"", value: 666)]
public partial struct @class
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}
");

    [Fact]
    public Task Generates_IParsable()
    {
        return RunTest(
            """
            using Vogen;

            namespace Whatever;

            [ValueObject(typeof(bool))]
            public sealed partial class Foo { }
            """);
        
        static Task RunTest(string source) =>
            new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public Task Generates_IParsable_and_skip_user_provided_method_1()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass
            {
                public static MyClass Parse(string s, IFormatProvider provider)
                {
                    throw new NotImplementedException();
                }
            }

            """);
    }

    [Fact]
    public Task Generates_IParsable_and_skip_user_provided_method_2()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass
            {
                public static MyClass Parse(string s, IFormatProvider provider)
                {
                    throw new NotImplementedException();
                }
                
                public static MyClass Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) 
                {
                    throw new NotImplementedException();
                }
            }

            """);
    }
    
    [Fact]
    public Task Generates_IParsable_and_skip_user_provided_method_3()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass
            {
                public static MyClass Parse(string s)
                {
                    throw new NotImplementedException();
                }
            }

            """);
    }

    [Fact]
    public Task Generates_IParsable_and_skip_user_provided_method_4()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass
            {
                public static MyClass Parse(string s) => throw new NotImplementedException();
            }

            """);
    }
    
    

    [Fact]
    public Task Generates_IParsable_for_a_class_wrapping_an_int()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject(typeof(int))]
            public partial class MyVo
            {
            }
            """);
    }

    [Fact]
    public Task Does_not_generate_IParsable_in_versions_of_dotnet_prior_to_7()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject(typeof(int))]
            public partial class MyVo
            {
            }
            """);
    }

    [Fact]
    public Task Generates_IParsable_for_a_class__wrapping_a_string()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject(typeof(string))]
            public partial class MyVo
            {
            }
            """);
    }

    [Fact]
    public Task Generates_IParsable_for_record_structs_wrapping_a_string()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject(typeof(string))]
            public readonly partial record struct MyVo
            {
            }
            """);
    }

    [Fact]
    public Task Generates_IParsable_and_skip_user_provided_method_5()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass2
            {
                public static bool TryParse(string input, out MyClass2 s) => throw new NotImplementedException();
            }

            """);
        
        static Task RunTest(string source) =>
            new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
        
    }

    [Fact]
    public Task Ignores_TryParse_where_last_parameter_is_not_out()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass2
            {
                public static bool TryParse(string input, MyClass2 s) => throw new NotImplementedException();
            }

            """);
        
        static Task RunTest(string source) =>
            new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
        
    }

    private static Task RunTest(string source) =>
        new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
        
}