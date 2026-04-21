using System.ComponentModel;

namespace Vogen;

/// <summary>
/// Controls whether <c>INumber&lt;T&gt;</c> and related numeric interfaces are generated
/// for value objects whose underlying type implements <c>INumber&lt;T&gt;</c>.
/// Requires C# 11 or later.
/// </summary>
public enum NumericsGeneration
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    Unspecified = -1,

    /// <summary>
    /// Do not implement <c>INumber&lt;T&gt;</c>. This is the default.
    /// </summary>
    Omit = 0,

    /// <summary>
    /// Implement <c>INumber&lt;T&gt;</c> and all required numeric interfaces if the underlying type
    /// supports it and C# 11 or later is targeted.
    /// </summary>
    GenerateINumberInterfaceAndMethods = 1,
}
