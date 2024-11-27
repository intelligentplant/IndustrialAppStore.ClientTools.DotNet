# Requesting Data Sources

> **NOTE:** 
> IAS apps will be unable to see any data sources unless they have been granted the `DataRead` or `DataWrite` scope.
> Additionally, users must grant an IAS app access to a data source before it becomes visible.


To get the data sources that are visible to the calling user, call the `GetDataSourcesAsync` method on the client's `DataSources` property:

```csharp
var dataSources = await client.DataSources.GetDataSourcesAsync();

// Specifying a cancellation token.
dataSources = await client.DataSources.GetDataSourcesAsync(cancellationToken: someCancellationToken);
```

The response will be a collection of [DataSourceInfo](/src/IntelligentPlant.DataCore.HttpClient/Model/DataSourceInfo.cs) objects. The `Name` property on each object defines a `DisplayName` and a `QualifiedName` property; the `QualifiedName` is always used when performing queries on the data source.
