# Reading Tag Values

> **NOTE:** 
> IAS apps require the `DataRead` scope to read tag values.
> Additionally, users must grant an IAS app access to a data source before it becomes visible.


Data sources allow you to request the values of tags; depending on the capabilities of the data source, you may be able to request snapshot (current) values, raw historical values, and/or computed historical values calculated using some sort of aggregation. The following sections describe how to perform different query types.


## Specifying Query Time Ranges and Intervals

Queries for historical tag values require you to specify a query time range, with the start and end time for the query being specified using `DateTime` instances. Each historical query type also has extension methods that allow the start and end times to be specified as `string` objects. When `string` objects are used, they can be absolute ISO 8601 timestamps (e.g. `2020-08-05T07:31:53Z`), or they can be relative timestamps (e.g. _3 hours before the start of the current minute_). 

Rules for specifying relative timestamps can be found [here](https://github.com/intelligentplant/IntelligentPlant.Relativity#parsing-timestamps).

Similarly, some historical queries require you to specify a sample interval, so that a calculation can be performed on historical values over the query time range (e.g. requesting the average value of a tag at one hour intervals over the last 24 hours). Intervals can be specified as `TimeSpan` instances, or as `string` objects. When a `string` is used, it must be parsable using `TimeSpan.Parse` (e.g. `00:30:00`, `1.16:23:37.5543241`), or it must be a valid short-hand duration.

Rules for specifying short-hand durations can be found [here](https://github.com/intelligentplant/IntelligentPlant.Relativity#parsing-durations).

`DateTime` instances are always assumed to be specified in UTC. When parsing timestamps from `string` objects, the resulting `DateTime` will always be converted to UTC.


## Reading Snapshot Tag Values

A snapshot value is the current instantaneous value of a tag. To request snapshot values from a data source, use the `ReadSnapshotTagValuesAsync` method on the client's `DataSources` property. The `IntelligentPlant.DataCore.Client` namespace contains extension methods to simplify the invocation of this operation.

```csharp
// Get snapshot values from a single data source (extension method).
var snapshotValues = await client.DataSources.ReadSnapshotTagValuesAsync(
    "MyDataSource",
    new [] { "Tag1", "Tag2" },
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Get snapshot values from multiple data sources (extension method).
var multiDataSourceSnapshotValues = await client.DataSources.ReadSnapshotTagValuesAsync(
    new Dictionary<string, string[]>() {
        ["MyDataSource"] = new [] { "Tag1", "Tag2" },
        ["MyOtherDataSource"] = new [] { "Tag3" } 
    },
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Get snapshot values from multiple data sources using a ReadSnapshotTagValuesRequest object.
multiDataSourceSnapshotValues = await client.DataSources.ReadSnapshotTagValuesAsync(
    new ReadSnapshotTagValuesRequest() {
        Tags = new Dictionary<string, string[]>() {
            ["MyDataSource"] = new[] { "Tag1", "Tag2" },
            ["MyOtherDataSource"] = new[] { "Tag3" }
        }
    },
    Request.HttpContext, 
    cancellationToken
);
```

When using an overload that queries a single data source, the return type will be a [SnapshotTagValueDictionary](/src/IntelligentPlant.DataCore.HttpClient/Model/TagValueDictionary.cs) object i.e. a dictionary that maps from tag name to snapshot value. When using an overload where multiple data sources can be specified, the return type will be a dictionary that maps from data source name to a `SnapshotTagValueDictionary` object (i.e. results are indexed by data source name, and then sub-indexed by tag name).


## Reading Raw Historical Tag Values

Raw values are the unprocessed historical values for a tag that are stored in a historican's archive, and are queried using the `ReadRawTagValuesAsync` method on the client's `DataSources` property. In addition to specifying the tag names to query, you also specify a time range for the query, and the maximum number of samples to retrieve per tag. Most historians will place an absolute limit on the number of samples to retrieve per tag, and also on the overall maximum number of samples that will be returned in a single query.

```csharp
// Get raw values from a single data source using DateTime instances to specify the time range 
// (extension method).
var historicalValues = await client.DataSources.ReadRawTagValuesAsync(
    "MyDataSource",
    new [] { "Tag1", "Tag2" },
    DateTime.UtcNow.AddHours(-1),
    DateTime.UtcNow,
    // Get up to 100 samples per tag
    100,
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Get raw values from a single data source using absolute timestamp strings to specify the time 
// range (extension method).
historicalValues = await client.DataSources.ReadRawTagValuesAsync(
    "MyDataSource",
    new[] { "Tag1", "Tag2" },
    "2020-08-05T07:08:00Z",
    "2020-08-05T08:08:00Z",
    // Get up to 100 samples per tag
    100,
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Get raw values from a single data source using relative timestamp strings to specify the time 
// range (extension method).
historicalValues = await client.DataSources.ReadRawTagValuesAsync(
    "MyDataSource",
    new[] { "Tag1", "Tag2" },
    "*-1H",
    "*",
    // Get up to 100 samples per tag
    100,
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Get raw values from multiple data sources using relative timestamp strings to specify the time 
// range (extension method).
var multiDataSourceHistoricalValues = await client.DataSources.ReadRawTagValuesAsync(
    new Dictionary<string, string[]>() {
        ["MyDataSource"] = new[] { "Tag1", "Tag2" },
        ["MyOtherDataSource"] = new[] { "Tag3" }
    },
    "*-1H",
    "*",
    // Get up to 100 samples per tag
    100,
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Get raw values from multiple data sources using a ReadRawTagValuesRequest object.
multiDataSourceHistoricalValues = await client.DataSources.ReadRawTagValuesAsync(
    new ReadRawTagValuesRequest() {
        Tags = new Dictionary<string, string[]>() {
            ["MyDataSource"] = new[] { "Tag1", "Tag2" },
            ["MyOtherDataSource"] = new[] { "Tag3" }
        },
        StartTime = DateTime.UtcNow.AddHours(-1),
        EndTime = DateTime.UtcNow,
        PointCount = 100
    },
    Request.HttpContext,
    cancellationToken
);
```

When using an overload that queries a single data source, the return type will be a [HistoricalTagValuesDictionary](/src/IntelligentPlant.DataCore.HttpClient/Model/TagValueDictionary.cs) object i.e. a dictionary that maps from tag name to `HistoricalTagValues` objects. A `HistoricalTagValues` object has properties containing the actual tag values, and a hint that recommends how the values should be visualised on a chart (e.g. trailing edge, interpolation between points). When using an overload where multiple data sources can be specified, the return type will be a dictionary that maps from data source name to a `HistoricalTagValuesDictionary` object (i.e. results are indexed by data source name, and then sub-indexed by tag name).


## Reading Plot Tag Values

Many data sources support the concept of a "plot" query. This is a request for historical tag values that is optimised for retrieving values to display on a chart. The exact algorithm for selecting or calculating values varies from vendor to vendor, but a common approach is the split the query time range into a number of equally-sized intervals (specified by the caller) and then selecting the minimum, maximum, earliest, and latest raw values in each interval i.e. each tag will return a sample count up to 4x the interval count, depending on whether a selected sample matches more than one of the selection criteria.

Plot queries are performed using the `ReadPlotTagValuesAsync` method overloads on the client's `DataSources` property.

```csharp
// Retrieve plot values for a single data source (extension method).
historicalValues = await client.DataSources.ReadPlotTagValuesAsync(
    "MyDataSource",
    new[] { "Tag1", "Tag2" },
    DateTime.UtcNow.AddDays(-1),
    DateTime.UtcNow,
    500,
    context: Request.HttpContext,
    cancellationToken: cancellationToken
)

// Retrieve plot values for a single data source using absolute timestamps (extension method).
historicalValues = await client.DataSources.ReadPlotTagValuesAsync(
    "MyDataSource",
    new[] { "Tag1", "Tag2" },
    "2020-08-04T08:08:00Z",
    "2020-08-05T08:08:00Z",
    500,
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Retrieve plot values for a single data source using relative timestamps (extension method).
historicalValues = await client.DataSources.ReadPlotTagValuesAsync(
    "MyDataSource",
    new[] { "Tag1", "Tag2" },
    "*-1D",
    "*",
    500,
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Retrieve plot values for multiple data sources using relative timestamps (extension method).
multiDataSourceHistoricalValues = await client.DataSources.ReadPlotTagValuesAsync(
    new Dictionary<string, string[]>() {
        ["MyDataSource"] = new[] { "Tag1", "Tag2" },
        ["MyOtherDataSource"] = new[] { "Tag3" }
    },
    "*-1D",
    "*",
    500,
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Retrieve plot values for multiple data sources using a ReadPlotTagValuesRequest object.
multiDataSourceHistoricalValues = await client.DataSources.ReadPlotTagValuesAsync(
    new ReadPlotTagValuesRequest() {
        Tags = new Dictionary<string, string[]>() {
            ["MyDataSource"] = new[] { "Tag1", "Tag2" },
            ["MyOtherDataSource"] = new[] { "Tag3" }
        },
        StartTime = DateTime.UtcNow.AddDays(-1),
        EndTime = DateTime.UtcNow,
        Intervals = 500
    },
    Request.HttpContext,
    cancellationToken
);
```

As with raw data queries, when using an overload that queries a single data source, the return type will be a [HistoricalTagValuesDictionary](/src/IntelligentPlant.DataCore.HttpClient/Model/TagValueDictionary.cs) object i.e. a dictionary that maps from tag name to `HistoricalTagValues` objects. A `HistoricalTagValues` object has properties containing the actual tag values, and a hint that recommends how the values should be visualised on a chart (e.g. trailing edge, interpolation between points). When using an overload where multiple data sources can be specified, the return type will be a dictionary that maps from data source name to a `HistoricalTagValuesDictionary` object (i.e. results are indexed by data source name, and then sub-indexed by tag name).


## Reading Processed/Aggregated Tag Values

Processed data queries (also referred to as aggregated data queries) use data functions to aggregate the raw data in a historian, and are performed using the `ReadProcessedTagValuesAsync` methods on the client's `DataSources` property. When asking for aggregated data, you specify the function name and a sample interval that you want to perform the aggregation at. For example, you might want to compute the average value of a tag at hourly sample intervals in the 24 hours leading up to the current time.

The available data functions vary by historian, but most drivers will typically support the following functions:

- `INTERP` - at each time interval, interpolate a value based on the values immediately before and immediately after the interval start and/or end times.
- `AVG` - average value for each time interval specified in the query.
- `MIN` - minimum tag value in each time interval. depending on the historian, this may return the actual raw timestamp of the minimum value, or it may return a value with the timestamp set to the start time of the interval.
- `MAX` - maximum tag value in each time interval. depending on the historian, this may return the actual raw timestamp of the minimum value, or it may return a value with the timestamp set to the start time of the interval. 

Additional data functions may be supported; please refer to the vendor's documentation. In the future, we plan to implement a discovery feature to allow the supported functions to be retrieved programatically.

```csharp
// Retrieve hourly average values over the last 24 hours for a single data source (extension 
// method).
historicalValues = await client.DataSources.ReadProcessedTagValuesAsync(
    "MyDataSource",
    new[] { "Tag1", "Tag2" },
    DateTime.UtcNow.AddDays(-1),
    DateTime.UtcNow,
    "AVG"
    "1H",
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Retrieve the minimum value each minute over the last 5 minutes for a single data source 
// (extension method).
historicalValues = await client.DataSources.ReadProcessedTagValuesAsync(
    "MyDataSource",
    new[] { "Tag1", "Tag2" },
    "*-5M",
    "*",
    "MIN"
    "1M",
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Retrieve the maximum value each minute over the last 5 minutes for multiple data sources 
// (extension method).
multiDataSourceHistoricalValues = await client.DataSources.ReadProcessedTagValuesAsync(
    new Dictionary<string, string[]>() {
        ["MyDataSource"] = new[] { "Tag1", "Tag2" },
        ["MyOtherDataSource"] = new[] { "Tag3" }
    },
    "*-5M",
    "*",
    "MAX"
    "1M",
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Retrieve the maximum value each minute over the last 5 minutes for multiple data sources using 
// a ReadProcessedTagValuesRequest object.
multiDataSourceHistoricalValues = await client.DataSources.ReadProcessedTagValuesAsync(
    new ReadProcessedTagValuesRequest() {
        Tags = new Dictionary<string, string[]>() {
            ["MyDataSource"] = new[] { "Tag1", "Tag2" },
            ["MyOtherDataSource"] = new[] { "Tag3" }
        },
        StartTime = DateTime.UtcNow.AddMinutes(-5),
        EndTime = DateTime.UtcNow,
        DataFunction = "MAX",
        SampleInterval = TimeSpan.FromMinutes(1)
    },
    Request.HttpContext,
    cancellationToken
);
```

As with other historical data queries, when using an overload that queries a single data source, the return type will be a [HistoricalTagValuesDictionary](/src/IntelligentPlant.DataCore.HttpClient/Model/TagValueDictionary.cs) object i.e. a dictionary that maps from tag name to `HistoricalTagValues` objects. A `HistoricalTagValues` object has properties containing the actual tag values, and a hint that recommends how the values should be visualised on a chart (e.g. trailing edge, interpolation between points). When using an overload where multiple data sources can be specified, the return type will be a dictionary that maps from data source name to a `HistoricalTagValuesDictionary` object (i.e. results are indexed by data source name, and then sub-indexed by tag name).
