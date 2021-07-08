# IntelligentPlant.Json.Serialization

This project defines extensions for `System.Text.Json` to allow serialization of types such as `TimeSpan` that are not natively implemented in `System.Text.Json`, and to allow serialization of NaN and infinity `double` and `float` values.


# Getting Started

Add the [IntelligentPlant.Json.Serialization](https://www.nuget.org/packages/IntelligentPlant.Json.Serialization) NuGet package to your project.

You can register the converters that you require manually, or you can call an extension method to register all of the converters:

```csharp
// using System.Text.Json;

public void ConfigureJsonOptions(JsonSerializerOptions options) {
    options.AddIntelligentPlantConverters();
}
```
