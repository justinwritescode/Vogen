# Handling nulls

Vogen is primarily designed for the domain layer where nulls just add confusion. For instance, what does a null `SettlmentAmount` mean? Does it mean there's nothing to settle, or does it mean something else?

But sometimes, in other layers, where nulls do exist, it can be useful to have convenience methods, for instance, `FromNullable(decimal?)` or `FromNullable(int?)`.

This how-to describes how to extend Vogen with new methods using extension members in C# 14 and onwards.

In your assembly (normally in your 'Infrastructure' layer if you're using Onion/Clean Architecture), specify that Vogen creates the `IVogen<TWrapper, TPrimitive>` on all types. 

```c#
[assembly: VogenDefaults(
    staticAbstractsGeneration: StaticAbstractsGeneration.InstancesHaveInterfaceDefinition | StaticAbstractsGeneration.FactoryMethods)]
```

Now, each value object will implement `IVogen<TWrapper, TPrimitive>`, allowing you to extend it.

Now, add the following:

```c#
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
```

You can now have:

```c#
string? primitive = null;
CustomerName? n = CustomerName.FromNullable(primitive);
```
                        
There is a [sample provided showing how to do this](https://github.com/SteveDunn/Vogen/tree/main/samples/ExampleExtensions).
