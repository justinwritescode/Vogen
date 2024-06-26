# Use in Swagger

You can use Vogen types as parameters in ASP.NET Core Web API endpoints. 
That is because the .NET runtime will notice the `TypeConverter` that is generated by default, and use that to try
to convert the underlying type (e.g. `string` or `int`) to the value object. 

However, Swagger treats value object fields as JSON, e.g. given this method:

```C#
[HttpGet("/WeatherForecast/{cityName}")]
public IEnumerable<WeatherForecast> Get(CityName cityName)
{
    ...
}
```

... You can use it (e.g. at `http://localhost:5053/WeatherForecast/London`) without having to do anything else, 
but Swagger will show the field as JSON instead of the underlying type:

<img border-effect="rounded" alt="swagger-json-parameter.png" src="swagger-json-parameter.png"/>

To fix this, add this to your project:

```C#
public class VogenSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.GetCustomAttribute<ValueObjectAttribute>() is not { } attribute)
            return;

        // Since we don't hold the actual type, we ca only use the generic attribute
        var type = attribute.GetType();
        
        if (!type.IsGenericType || type.GenericTypeArguments.Length != 1)
        {
            return;
        }

        var schemaValueObject = context.SchemaGenerator.GenerateSchema(
            type.GenericTypeArguments[0], 
            context.SchemaRepository, 
            context.MemberInfo, context.ParameterInfo);
        
        CopyPublicProperties(schemaValueObject, schema);
    }

    private static void CopyPublicProperties<T>(T oldObject, T newObject) where T : class
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

        if (ReferenceEquals(oldObject, newObject))
        {
            return;
        }

        var type = typeof(T);
        var propertyList = type.GetProperties(flags);
        if (propertyList.Length <= 0) return;

        foreach (var newObjProp in propertyList)
        {
            var oldProp = type.GetProperty(newObjProp.Name, flags)!;
            if (!oldProp.CanRead || !newObjProp.CanWrite) continue;

            var value = oldProp.GetValue(oldObject);
            newObjProp.SetValue(newObject, value);
        }
    }
}
```

... and then, when you register Swagger, tell it to use this new filter:

```C#
builder.Services.AddSwaggerGen(opt => opt.SchemaFilter<VogenSchemaFilter>());
```
Many thanks to [Vitalii Mikhailov](https://github.com/Aragas) for this contribution.

<note>
Although this is currently a manual step, it is intended for this to be put into a NuGet package at some point.
</note>