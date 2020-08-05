# IntelligentPlant.DataCore.HttpClient

A strongly-typed C# client for Intelligent Plant's Data Core API.


## What is Data Core?

Data Core is Intelligent Plant's data access API for industrial process data. It is primarily used by apps on the [Industrial App Store](https://appstore.intelligentplant.com) (IAS), but can also be used to integrate with standalone Intelligent Plant applications. Note that IAS apps should use the [IntelligentPlant.IndustrialAppStore.HttpClient](/src/IntelligentPlant.IndustrialAppStore.HttpClient) or [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) libraries, which extend this library by providing additional IAS-specific API methods, and hooks for authenticating all outgoing API calls.


## What is the Industrial App Store?

The [Industrial App Store](https://appstore.intelligentplant.com) (IAS) is a cloud-based platform offering apps for analysing and visualising real-time process data, and alarm & event data. Instead of being uploaded to the cloud for storage, data is stored on client networks, and connected to the IAS using a tool called [App Store Connect](https://appstore.intelligentplant.com/Home/AppProfile?appId=a73c453df5f447a6aa8a08d2019037a5).


# Installation

Install the [IntelligentPlant.DataCore.HttpClient](https://www.nuget.org/packages/IntelligentPlant.DataCore.HttpClient) package by following the instructions on NuGet.org.


# Getting Started

The abstract [DataCoreHttpClient<TContext, TOptions>](./DataCoreHttpClient.cs) class is used to perform Data Core API calls. The class itself has a set of properties (e.g. `DataSources`, `EventSources`) that expose API methods for different feature areas. Additional concrete subclasses (`DataCoreHttpClient<TContext>` and `DataCoreHttpClient`) are available to simplify configuration depending on the type of authentication used by the app.

Note that, if you are writing an app that interfaces with the Industrial App Store, you should use the client class from the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication/IndustrialAppStoreHttpClient.cs) or [IntelligentPlant.IndustrialAppStore.HttpClient](/src/IntelligentPlant.IndustrialAppStore.HttpClient/IndustrialAppStoreHttpClientT.cs) library instead. Please refer to the documentation for those libraries for details of how to configure their respective clients.


## Concepts


### Call Context

All API methods defined by the client include a `context` parameter, whose type is defined via the generic `TContext` type used when creating the client object. The purpose of this context is to allow the client to use per-call authentication when making API calls. The Industrial App Store requires that all API calls include an access token used to authenticate and authorise the calling user and app combination; the context object provided to an API method can be used to identify which access token to attach to an outgoing request. For example, when using the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) library in an ASP.NET Core app, the context object is an ASP.NET Core [HttpContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext) object, which is used to obtain the caller's access token from their session data that is set when they log into the app.

Note that, when querying a standalone Data Core API instance, Windows authentication is typically used. In thse circumstances, the context parameter is typically ignored because credentials must be set directly on the `HttpClient` passed to the Data Core API client's constructor.


### Client Options

When creating a client object, an options object must be passed to the constructor. This is used to define e.g. the base URL for the Data Core API endpoint. When creating a client for an Industrial App Store app, the constructor for the options class will set appropriate default endpoint values; endpoints must be manually configured when creating a client for a standalone Data Core API instance.


## Creating a Client

_This section describes creating clients for querying standalone Data Core API instances. To create a client for an Industrial App Store app, refer to the documentation in the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication/IndustrialAppStoreHttpClient.cs) or [IntelligentPlant.IndustrialAppStore.HttpClient](/src/IntelligentPlant.IndustrialAppStore.HttpClient/IndustrialAppStoreHttpClientT.cs) library instead._


### Windows Authentication

To create a Data Core API client for querying a standalone Data Core API instance using Windows authentication, use the `DataCoreHttpClient` class, ensuring that the `HttpClient` you provide has the appropriate credentials configured:

```csharp
// In .NET Core 2.1 or later, use SocketsHttpHandler instead of HttpClientHandler.
var handler = new HttpClientHandler() {
    Credentials = new NetworkCredential("username", "password")
};
var httpClient = new HttpClient(handler);

var options = new DataCoreHttpClientOptions() {
    // Remember the trailing / at the end of the URL!
    DataCoreUrl = new Uri("https://path/to/data/core/")
};

var client = new DataCoreHttpClient(httpClient, options);

var dataSources = await client.DataSources.GetDataSourcesAsync();
```


### Static Authentication Header

When creating a Data Core API client for querying a Data Core API instance using a fixed authentication header (e.g. using Basic authentication), it is not necessary to manually create the inner HTTP message handler for the `HttpClient` instance. Instead, default request headers can be specified on the `HttpClient`:

```csharp
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
    "Basic",
    Convert.ToBase64String(Encoding.ASCII.GetBytes("username:password"))
);

var options = new DataCoreHttpClientOptions() {
    // Remember the trailing / at the end of the URL!
    DataCoreUrl = new Uri("https://path/to/data/core/")
};

var client = new DataCoreHttpClient(httpClient, options);

var dataSources = await client.DataSources.GetDataSourcesAsync();
```


### Dynamic Authentication Header

The third option when creating a client is to dynamically add an authentication header to outgoing requests based on the `context` passed to an API method call. We do this by adding a [DelegatingHandler](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler) to the request pipeline for the `HttpClient` that we pass to the Data Core client constructor. The handler will receive the `context` parameter passed to the API method call.

In this example, we will use the `ClaimsPrincipal` class as our context, and attach a different authorization header to requests based on the group memberships of the principal.

```csharp
// Our delegate that will return an authentication header based on a ClaimsPrincipal instance.
AuthenticationCallback<ClaimsPrincipal> authCallback = async (req, ctx, ct) => {
    if (ctx == null) {
        return null;
    }

    if (ctx.IsInRole("Administrators")) {
        return new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:password_1"))
        );
    }

    if (ctx.IsInRole("Users")) {
        return new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes("user:password_2"))
        );
    }

    return null;
};

// We'll use the Microsoft.Extensions.DependencyInjection library to build our client.
var services = new ServiceCollection();

services
    .AddHttpClient<DataCoreHttpClient<ClaimsPrincipal>>()
    .AddHttpMessageHandler(() => DataCoreHttpClient.CreateAuthenticationMessageHandler(authCallback));

services.AddSingleton(new DataCoreHttpClientOptions() {
    // Remember the trailing / at the end of the URL!
    DataCoreUrl = new Uri("https://path/to/data/core/")
});

var serviceProvider = services.BuildServiceProvider();

// Create our client using the service provider.
var client = serviceProvider.GetService<DataCoreHttpClient<ClaimsPrincipal>>();
```

When calling API methods using clients created in this way, you just pass in the `ClaimsPrincipal` associated with the call, and the callback will return the authentication header to be added to the outgoing HTTP request. For example:

```csharp
var dataSources = await client.DataSources.GetDataSourcesAsync(WindowsPrincipal.Current);
```


# Basic Client Usage

_You should ensure that you import the `IntelligentPlant.DataCore.Client` namespace in any classes that use the client. This namespace contains extensions methods to simplify the invocation of a lot of API methods._

Once you have created the API client, you can use it to call a variety of API methods. API methods are broken down by feature area. For example, functions for reading (or writing) process data are separated from functions for browsing asset models, and so on. Each feature area is represented by a property on the client (e.g. the `DataSources` property is used to expose the API methods for reading process data from/writing process data to data sources such as industrial historians).

All API methods allow a context and a cancellation token to be specified for the operation. These parameters are optional; the default value for each will be used if not specified (i.e. `default(TContext)` and `default(CancellationToken)`).


## Getting Available Data Sources

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


## Searching for Tags

A tag on a data source represents the value of a single instrument in an industrial process, or e.g. the result of a calculation, recorded over time. To search for available tags on a data source, use the `FindTagsAsync` method on the client's `DataSources` property. The `IntelligentPlant.DataCore.Client` namespace contains extension methods to simplify the invocation of this operation.

```csharp
// Use an extension method to specify the filter properties.
var tags = await client.DataSources.FindTagsAsync(
    "MyDataSource",
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


## Reading Tag Values

Data sources allow you to request the values of tags; depending on the capabilities of the data source, you may be able to request snapshot (current) values, raw historical values, and/or computed historical values calculated using some sort of aggregation. The following sections describe how to perform different query types.


### Specifying Query Time Ranges and Intervals

Queries for historical tag values require you to specify a query time range, with the start and end time for the query being specified using `DateTime` instances. Each historical query type also has extension methods that allow the start and end times to be specified as `string` objects. When `string` timestamps are specified, they can be absolute ISO 8601 timestamps, or they can be relative timestamps (e.g. _"3 hours before the start of the current minute"_).

Similarly, some historical queries require you to specify a sample interval, so that a calculation can be performed on historical values over the query time range (e.g. requesting the average value of a tag at one hour intervals over the last 24 hours). Intervals can be specified as `TimeSpan` instances, or as `string` objects. When a `string` is used, it must be parsable using `TimeSpan.Parse`, or it must be a valid short-hand duration.

Rules for specifying relative timestamps and short-hand durations can be found [here](https://github.com/intelligentplant/IntelligentPlant.Relativity#parsing-timestamps).

`DateTime` instances are always assumed to be specified in UTC. When parsing timestamps from `string` objects, the resulting `DateTime` will always be converted to UTC.


### Reading Snapshot Tag Values

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

When using an overload that queries a single data source, the return type will be a `SnapshotTagValueDictionary` object i.e. a dictionary that maps from tag name to snapshot value. When using an overload where multiple data sources can be specified, the return type will be a dictionary that maps from data source name to a `SnapshotTagValueDictionary` object (i.e. results are indexed by data source name, and then sub-indexed by tag name).


### Reading Raw Historical Tag Values

Raw values are the unprocessed historical values for a tag that are stored in a historican's archive, and are queried using the `ReadRawTagValuesAsync` method on the client's `DataSources` property. In addition to specifying the tag names to query, you also specify a time range for the query, and the maximum number of samples to retrieve per tag. Most historians will place an absolute limit on the number of samples to retrieve per tag, and also on the overall maximum number of samples that will be returned in a single query.

```csharp
// Get raw values from a single data source using DateTime instances to specify the time range (extension method).
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

// Get raw values from a single data source using absolute timestamp strings to specify the time range (extension method).
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

// Get raw values from a single data source using relative timestamp strings to specify the time range (extension method).
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

// Get raw values from multiple data sources using relative timestamp strings to specify the time range (extension method).
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

When using an overload that queries a single data source, the return type will be a `HistoricalTagValuesDictionary` object i.e. a dictionary that maps from tag name to `HistoricalTagValues` objects. A `HistoricalTagValues` object has properties containing the actual tag values, and a hint that recommends how the values should be visualised on a chart (e.g. trailing edge, interpolation between points). When using an overload where multiple data sources can be specified, the return type will be a dictionary that maps from data source name to a `HistoricalTagValuesDictionary` object (i.e. results are indexed by data source name, and then sub-indexed by tag name).


### Reading Plot Tag Values

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

As with raw data queries, when using an overload that queries a single data source, the return type will be a `HistoricalTagValuesDictionary` object i.e. a dictionary that maps from tag name to `HistoricalTagValues` objects. A `HistoricalTagValues` object has properties containing the actual tag values, and a hint that recommends how the values should be visualised on a chart (e.g. trailing edge, interpolation between points). When using an overload where multiple data sources can be specified, the return type will be a dictionary that maps from data source name to a `HistoricalTagValuesDictionary` object (i.e. results are indexed by data source name, and then sub-indexed by tag name).


### Reading Processed/Aggregated Tag Values

Processed data queries (also referred to as aggregated data queries) use data functions to aggregate the raw data in a historian, and are performed using the `ReadProcessedTagValuesAsync` methods on the client's `DataSources` property. When asking for aggregated data, you specify the function name and a sample interval that you want to perform the aggregation at. For example, you might want to compute the average value of a tag at hourly sample intervals in the 24 hours leading up to the current time.

The available data functions vary by historian, but most drivers will typically support the following functions:

- `INTERP` - at each time interval, interpolate a value based on the values immediately before and immediately after the interval start and/or end times.
- `AVG` - average value for each time interval specified in the query.
- `MIN` - minimum tag value in each time interval. depending on the historian, this may return the actual raw timestamp of the minimum value, or it may return a value with the timestamp set to the start time of the interval.
- `MAX` - maximum tag value in each time interval. depending on the historian, this may return the actual raw timestamp of the minimum value, or it may return a value with the timestamp set to the start time of the interval. 

Additional data functions may be supported; please refer to the vendor's documentation. In the future, we plan to implement a discovery feature to allow the supported functions to be retrieved programatically.

```csharp
// Retrieve hourly average values over the last 24 hours for a single data source (extension method).
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

// Retrieve the maximum value each minute over the last 5 minutes for multiple data sources using a 
// ReadProcessedTagValuesRequest object.
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

As with other historical data queries, when using an overload that queries a single data source, the return type will be a `HistoricalTagValuesDictionary` object i.e. a dictionary that maps from tag name to `HistoricalTagValues` objects. A `HistoricalTagValues` object has properties containing the actual tag values, and a hint that recommends how the values should be visualised on a chart (e.g. trailing edge, interpolation between points). When using an overload where multiple data sources can be specified, the return type will be a dictionary that maps from data source name to a `HistoricalTagValuesDictionary` object (i.e. results are indexed by data source name, and then sub-indexed by tag name).
