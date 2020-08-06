# Writing Tag Values

Some data sources (such as Intelligent Plant's Edge Historian) support writing tag values. This can be used to store e.g. calculation results computed by Industrial App Store apps. Note that writing values requires a higher level of permissions than reading values; write permissions are typically only granted to trusted actors.

The Data Core API supports two different types of tag value write operations: snapshot and archive. Both operations use the same method signatures, but have different intents that can be interpeted differently by the target data source.


## Writing Snapshot Tag Values

The intent of a snapshot tag value write is to update the instantaneous (current) value of a tag. If you attempt to write a snapshot value that is older than the current tag value, the target data source may choose to ignore the new value you have specified.

Writing snapshot tag values is performed using the `WriteSnapshotTagValuesAsync` and `WriteSnapshotTagValueAsync` methods on the client's `DataSources` property:

```csharp
// Write a numeric value to a single tag (extension method).
var writeResult = await client.DataSources.WriteSnapshotTagValueAsync(
    "MyDataSource",
    "Tag1",
    DateTime.UtcNow,
    100,
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Write a text value with non-good quality to a single tag (extension method).
writeResult = await client.DataSources.WriteSnapshotTagValueAsync(
    "MyDataSource",
    "Tag1",
    DateTime.UtcNow,
    "Calculation Error",
    status: TagValueStatus.Bad,
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Write multiple numeric values to a single tag (extension method).
writeResult = await client.DataSources.WriteSnapshotTagValuesAsync(
    "MyDataSource",
    "Tag1",
    new Dictionary<DateTime, double>() {
        [DateTime.UtcNow.AddMinutes(-5)] = 100,
        [DateTime.UtcNow.AddMinutes(-2.5)] = 50,
        [DateTime.UtcNow] = 0
    },
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Write multiple text values to a single tag (extension method).
writeResult = await client.DataSources.WriteSnapshotTagValuesAsync(
    "MyDataSource",
    "Tag1",
    new Dictionary<DateTime, string>() {
        [DateTime.UtcNow.AddMinutes(-5)] = "Industrial",
        [DateTime.UtcNow.AddMinutes(-2.5)] = "App",
        [DateTime.UtcNow] = "Store"
    },
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Write tag value objects to multiple tags (extension method).
var multiTagWriteResults = await client.DataSources.WriteSnapshotTagValuesAsync(
    "MyDataSource",
    new[] {
        new TagValue(
            "Tag1", 
            DateTime.UtcNow, 
            double.NaN, 
            "Calculation Error", 
            TagValueStatus.Bad, 
            null, 
            null, 
            "No data was available for the calculation time range"
        ),
        new TagValue(
            "Tag2",
            DateTime.UtcNow,
            67.5,
            null,
            TagValueStatus.Good,
            "deg C",
            null,
            null
        )
    },
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Write values to multiple tags using a WriteTagValuesRequest object.
multiTagWriteResults = await client.DataSources.WriteSnapshotTagValuesAsync(
    new WriteTagValuesRequest() {
        DataSourceName = "MyDataSource",
        Values = new[] {
            new TagValue(
                "Tag1",
                DateTime.UtcNow,
                double.NaN,
                "Calculation Error",
                TagValueStatus.Bad,
                null,
                null,
                "No data was available for the calculation time range"
            ),
            new TagValue(
                "Tag2",
                DateTime.UtcNow,
                67.5,
                null,
                TagValueStatus.Good,
                "deg C",
                null,
                null
            )
        }
    }, 
    Request.HttpContext, 
    cancellationToken
);
```

When calling an overload that writes to a single tag, the return value will be a `TagValueUpdateResponse` object describing if the write operation was successful. When calling an overload that writes to multiple tags, a collection of `TagValueUpdateResponse` objects is returned (one entry per tag that was written to).


## Writing Historical Tag Values

The intent of a historical tag value write is to insert values directly into a data source's history archive (e.g. to back-fill a gap in history caused by a data outage). When writing to a data source's snapshot, filters are sometimes applied by the data source to ensure that incoming value changes are only written to the history archive if they are identified as meaningful value changes. Writing directly to the data source's archive will usually bypass these filters entirely.

Historical tag value writes are performed using the `WriteHistoricalTagValuesAsync` method on the client's `DataSources` property:

```csharp
// Write multiple numeric values to a single tag (extension method).
var writeResult = await client.DataSources.WriteHistoricalTagValuesAsync(
    "MyDataSource",
    "Tag1",
    new Dictionary<DateTime, double>() {
        [DateTime.UtcNow.AddMinutes(-60)] = 100,
        [DateTime.UtcNow.AddMinutes(-55)] = 50,
        [DateTime.UtcNow.AddMinutes(-50)] = 0
    },
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Write multiple text values to a single tag (extension method).
writeResult = await client.DataSources.WriteHistoricalTagValuesAsync(
    "MyDataSource",
    "Tag1",
    new Dictionary<DateTime, string>() {
        [DateTime.UtcNow.AddMinutes(-60)] = "Industrial",
        [DateTime.UtcNow.AddMinutes(-55)] = "App",
        [DateTime.UtcNow.AddMinutes(-50)] = "Store"
    },
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Write tag value objects to multiple tags (extension method).
var multiTagWriteResults = await client.DataSources.WriteHistoricalTagValuesAsync(
    "MyDataSource",
    new[] {
        new TagValue(
            "Tag1", 
            DateTime.UtcNow.AddMinutes(-50), 
            double.NaN, 
            "Calculation Error", 
            TagValueStatus.Bad, 
            null, 
            null, 
            "No data was available for the calculation time range"
        ),
        new TagValue(
            "Tag2",
            DateTime.UtcNow.AddMinutes(-50),
            67.5,
            null,
            TagValueStatus.Good,
            "deg C",
            null,
            null
        )
    },
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Write values to multiple tags using a WriteTagValuesRequest object.
multiTagWriteResults = await client.DataSources.WriteHistoricalTagValuesAsync(
    new WriteTagValuesRequest() {
        DataSourceName = "MyDataSource",
        Values = new[] {
            new TagValue(
                "Tag1",
                DateTime.UtcNow.AddMinutes(-50),
                double.NaN,
                "Calculation Error",
                TagValueStatus.Bad,
                null,
                null,
                "No data was available for the calculation time range"
            ),
            new TagValue(
                "Tag2",
                DateTime.UtcNow.AddMinutes(-50),
                67.5,
                null,
                TagValueStatus.Good,
                "deg C",
                null,
                null
            )
        }
    }, 
    Request.HttpContext, 
    cancellationToken
);
```
