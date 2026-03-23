namespace ConsumerTests;


[ValueObject]
public partial class ArticleIdClass;

[ValueObject]
public partial struct ArticleIdStruct;

[ValueObject<string>]
public partial class ArticleNameClass;

[ValueObject<string>]
public partial struct ArticleNameStruct;

public class FromNullableTests
{
    [Fact]
    public void WrapperIsAClass_wrapping_a_class_primitive()
    {
        // Arrange
        int? nullableValue  = null;

        // Act
        ArticleIdClass? articleId = ArticleIdClass.FromNullable(nullableValue);

        // Assert
        articleId.Should().BeNull();
    }

    [Fact]
    public void WrapperIsAClass_wrapping_a_struct_primitive()
    {
        // Arrange
        int? nullableValue  = null;

        // Act
        ArticleIdStruct? articleId = ArticleIdStruct.FromNullable(nullableValue);

        // Assert
        articleId.Should().BeNull();
    }

    [Fact]
    public void WrapperIsAStruct_wrapping_a_class_primitive()
    {
        // Arrange
        string? nullableValue  = null;

        // Act
        ArticleNameClass? articleId = ArticleNameClass.FromNullable(nullableValue);

        // Assert
        articleId.Should().BeNull();
    }

    [Fact]
    public void WrapperIsAStruct_wrapping_a_struct_primitive()
    {
        // Arrange
        string? nullableValue  = null;

        // Act
        ArticleNameStruct? articleId = ArticleNameStruct.FromNullable(nullableValue);

        // Assert
        articleId.Should().BeNull();
    }
}

public static class VogenStructExtensions
{
    extension<TW, TP>(IVogen<TW, TP>) where TW : struct, IVogen<TW, TP> where TP : struct
    {
        public static TW? FromNullable(TP? value) => value is null ? null : TW.From(value.Value);
    }

    extension<TW, TP>(IVogen<TW, TP>) where TW : struct, IVogen<TW, TP> where TP : class
    {
        public static TW? FromNullable(TP? value) => value is null ? null : TW.From(value);
    }
}

public static class VogenClassExtensions
{
    extension<TW, TP>(IVogen<TW, TP>) where TW : class, IVogen<TW, TP> where TP : struct
    {
        public static TW? FromNullable(TP? value) => value is null ? null : TW.From(value.Value);
    }

    extension<TW, TP>(IVogen<TW, TP>) where TW : class, IVogen<TW, TP> where TP : class
    {
        public static TW? FromNullable(TP? value) => value is null ? null : TW.From(value);
    }
}
