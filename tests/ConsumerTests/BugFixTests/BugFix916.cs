using Newtonsoft.Json;

namespace ConsumerTests.BugFixTests.BugFix916;

/// <summary>
/// Tests for bug #916: "[ValueObject] gives an exception when deserialized by newtonsoft.Json (Version 8.0.5)"
/// 
/// Root cause: The generated Newtonsoft.Json converter templates were missing the `global::` prefix
/// when referencing Newtonsoft.Json types. This caused namespace collision when users had a namespace
/// named `Newtonsoft.Json` or similar in their project.
/// 
/// Fix: Added `global::` prefix to all Newtonsoft.Json type references in all 17 converter templates,
/// aligning them with the pattern already used in System.Text.Json converters.
/// 
/// User confirmation: "I have a namespace xxxx.Newtonsoft.Json, I renamed this to NewtonsoftJson and it worked..."
/// This proved the issue was namespace collision, not visibility or version requirements.
/// </summary>
public class BugFix916Tests
{
    /// <summary>
    /// Test for issue #916: Newtonsoft.Json deserialization with [Instance] attributes on string-backed ValueObject
    /// Expected: Should deserialize to TestValueObject.Eur when value is "EUR"
    /// </summary>
    [Fact]
    public void Should_deserialize_string_value_object_with_instances()
    {
        // Arrange
        var json = "\"EUR\"";

        // Act
        var result = JsonConvert.DeserializeObject<TestValueObject>(json);

        // Assert
        result.Should().Be(TestValueObject.Eur);
    }

    /// <summary>
    /// Test that other instance values also deserialize correctly
    /// </summary>
    [Fact]
    public void Should_deserialize_different_instance_value()
    {
        // Arrange
        var json = "\"GBP\"";

        // Act
        var result = JsonConvert.DeserializeObject<TestValueObject>(json);

        // Assert
        result.Should().Be(TestValueObject.Gbp);
    }

    /// <summary>
    /// Test deserialization of non-instance string value
    /// </summary>
    [Fact]
    public void Should_deserialize_non_instance_string_value()
    {
        // Arrange
        var json = "\"USD\"";

        // Act
        var result = JsonConvert.DeserializeObject<TestValueObject>(json);

        // Assert
        result.Value.Should().Be("USD");
    }
}

// Test namespace collision scenario
// This namespace mimics the user's situation where they had a namespace called "Newtonsoft.Json"
namespace ConsumerTests.BugFixTests.BugFix916.Newtonsoft
{
    namespace Json
    {
        // Empty namespace - this should NOT interfere with global Newtonsoft.Json types
    }

    /// <summary>
    /// This test verifies the fix for namespace collision.
    /// The generated converters must use `global::Newtonsoft.Json.JsonConverter` and related types
    /// to avoid resolving to the local `Newtonsoft.Json` namespace defined above.
    /// </summary>
    public class NamespaceCollisionTests
    {
        [Fact]
        public void Should_work_even_with_local_newtonsoft_json_namespace_defined()
        {
            // Arrange
            var json = "\"EUR\"";

            // Act
            var result = JsonConvert.DeserializeObject<TestValueObject>(json);

            // Assert
            result.Should().Be(TestValueObject.Eur);
        }

        [Fact]
        public void Should_serialize_correctly_with_local_namespace()
        {
            // Arrange
            var valueObject = TestValueObject.Gbp;

            // Act
            var json = JsonConvert.SerializeObject(valueObject);

            // Assert
            json.Should().Be("\"GBP\"");
        }
    }
}

[ValueObject(typeof(string), conversions: Conversions.NewtonsoftJson)]
[Instance("Eur", "EUR")]
[Instance("Gbp", "GBP")]
public readonly partial struct TestValueObject
{
}
