using System.ComponentModel;

namespace Vogen;

/// <summary>
/// Controls whether numeric interfaces are generated for value objects.
/// Requires C# 11 or later.
/// </summary>
public enum NumericsGeneration
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    Unspecified = -1,

    /// <summary>
    /// Do not implement any numeric interfaces. This is the default.
    /// </summary>
    Omit = 0,

    /// <summary>
    /// Automatically implement the richest numeric interface the underlying type supports,
    /// if C# 11 or later is targeted.
    /// Implements <c>INumber&lt;T&gt;</c> for types such as <c>double</c> or <c>int</c>,
    /// or <c>INumberBase&lt;T&gt;</c> for types such as <c>System.Complex</c>.
    /// </summary>
    Generate = 1,
}
