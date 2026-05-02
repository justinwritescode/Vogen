using VerifyTests;

namespace SnapshotTests;

public static class Extensions
{
    public static void UseHashedParameters(this VerifySettings s, params object?[] parameters)
    {
        s.UseParameters(parameters);
        s.HashParameters();
    }
}