using System;
using FluentAssertions;
using Vogen.Tests.Types;
using Xunit; //using tests.Types.AnotherNamespace.AndAnother;

namespace Vogen.Tests;

[ValueObject(typeof(int))]
public partial class MyInt
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}

public class CreationTests
{
    [Fact]
    public void Creation_Happy_Path_MyInt()
    {
        MyInt vo1 = MyInt.From(123);
        MyInt vo2 = MyInt.From(123);
    
        vo1.Should().Be(vo2);
        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void Creation_Happy_Path_MyString()
    {
        MyString vo1 = MyString.From("123");
        MyString vo2 = MyString.From("123");

        vo1.Should().Be(vo2);
        (vo1 == vo2).Should().BeTrue();
    }

    // doesn't work - need to handle nested types
    // [Fact]
    // public void Creation_Happy_Path_EmbeddedType()
    // {
    //     var embedded = new TopLevelClass.AnotherClass.AndAnother.NestedType();
    //     embedded.From(123);
    // }

    [Fact]
    public void Creation_Unhappy_Path_MyString()
    {
        Action action = () => MyString.From(null!);
        
        action.Should().Throw<ValueObjectValidationException>().WithMessage("Cannot create a value object with null.");
    }

    [Fact]
    public void Creation_Unhappy_Path_MyInt()
    {
        Action action = () => MyInt.From(-1);
        
        action.Should().Throw<ValueObjectValidationException>().WithMessage("must be greater than zero");
    }
}