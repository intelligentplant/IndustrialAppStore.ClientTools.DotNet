# Requesting Data Sources

> 
> **NOTE:** 
> IAS apps will be unable to see any data sources unless they have been granted the `DataRead` or `DataWrite` scope.
> 


To get the data sources that are visible to the calling user, call the `GetDataSourcesAsync` method on the client's `DataSources` property:

```csharp
// Call without a context (e.g. if Windows authentication is being used).
var dataSources = await client.DataSources.GetDataSourcesAsync();

// Call with a context (e.g. from an ASP.NET Core app in the Industrial App Store).
dataSources = await client.DataSources.GetDataSourcesAsync(Request.HttpContext);

// Specifying a cancellation token.
dataSources = await client.DataSources.GetDataSourcesAsync(cancellationToken: someCancellationToken);
dataSources = await client.DataSources.GetDataSourcesAsync(Request.HttpContext, someCancellationToken);
```

The response will be a collection of [DataSourceInfo](/src/IntelligentPlant.DataCore.HttpClient/Model/DataSourceInfo.cs) objects. The `Name` property on each object defines a `DisplayName` and a `QualifiedName` property; the `QualifiedName` is always used when performing queries on the data source.
