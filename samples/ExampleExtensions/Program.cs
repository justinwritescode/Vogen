using Vogen;

// Generates the IVogen<TWrapper, TPrimitive> interface, which can then be used in C# 14 extension members.
[assembly: VogenDefaults(
    staticAbstractsGeneration: StaticAbstractsGeneration.InstancesHaveInterfaceDefinition | StaticAbstractsGeneration.FactoryMethods)]

int suppliedId = 123;
int? nullId = null;

var non_null_id_1 = CustomerId.From(suppliedId);
var non_null_id_2 = CustomerId.FromNullable(suppliedId);
CustomerId? null_id_1 = CustomerId.FromNullable(nullId);

string suppliedName = "Frescho";
string? nullName = null;

var non_null_name_1 = CustomerName.From(suppliedName);
var non_null_name_2 = CustomerName.FromNullable(suppliedName);
CustomerName? null_name_1 = CustomerName.FromNullable(nullName);

WriteId(non_null_id_1);
WriteId(non_null_id_2);
WriteId(null_id_1);

WriteName(non_null_name_1);
WriteName(non_null_name_2);
WriteName(null_name_1);
return;

static void WriteId(CustomerId? vo) => Console.WriteLine(vo?.Value.ToString() ?? "null");
static void WriteName(CustomerName? vo) => Console.WriteLine(vo?.Value ?? "null");


[ValueObject<int>]
public partial struct CustomerId;

[ValueObject<string>]
public partial class CustomerName;


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
