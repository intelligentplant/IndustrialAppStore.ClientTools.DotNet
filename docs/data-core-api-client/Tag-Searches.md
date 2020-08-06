# Tag Searches

> **NOTE:** 
> IAS apps will be unable to see any data sources unless they have been granted the `DataRead` or `DataWrite` scope.


A tag on a data source represents the value of a single instrument in an industrial process, or e.g. the result of a calculation, recorded over time. To search for available tags on a data source, use the `FindTagsAsync` method on the client's `DataSources` property. The `IntelligentPlant.DataCore.Client` namespace contains extension methods to simplify the invocation of this operation.

```csharp
// Use an extension method to specify the filter properties.
var tags = await client.DataSources.FindTagsAsync(
    // Data source name must be the fully-qualified name of the data source!
    "MyDataSource",
    // Use * as a wildcard in tag name
    "PT-*",
    page: 3,
    pageSize: 20,
    context: Request.HttpContext,
    cancellationToken: cancellationToken    
);

// Specify the filter details using a FindTagsRequest object.
tags = await client.DataSources.FindTagsAsync(new FindTagsRequest() {
    // DataSourceName must be the fully-qualified name of the data source!
    DataSourceName = "MyDataSource",
    // Use * as a wildcard in tag name
    Filter = new TagSearchFilter("PT-*") {
        Page = 3,
        PageSize = 20
    }
}, Request.HttpContext, cancellationToken);
```

The query response is a collection of [TagSearchResult](/src/IntelligentPlant.DataCore.HttpClient/Model/TagSearchResult.cs) objects. Each result contains properties for the tag's name, description, unit of measure, and so on. Additional, data source-specific properties are specified in the tag's `Properties` collection.
