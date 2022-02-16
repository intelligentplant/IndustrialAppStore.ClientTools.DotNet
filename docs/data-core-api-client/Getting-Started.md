# Getting Started


## Concepts


### Call Context

All API methods defined by the API client include a `context` parameter, whose type is defined via the generic `TContext` type used when creating the client object. The purpose of this context is to allow the client to use per-call authentication when making API calls. The Industrial App Store requires that all API calls include an access token used to authenticate and authorise the calling user and app combination; the context object provided to an API method can be used to identify which access token to attach to an outgoing request. For example, when using the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) library in an ASP.NET Core app, the context object is an ASP.NET Core [HttpContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext) object, which is used to obtain the caller's access token from their session data that is set when they log into the app.

Note that, when querying a standalone Data Core API instance, Windows authentication is typically used. In thse circumstances, the context parameter is typically ignored because credentials must be set directly on the `HttpClient` passed to the API client's constructor.


### Client Options

When creating a client object, an options object must be passed to the constructor. This is used to define e.g. the base URL for the Data Core API endpoint. When creating a client for an Industrial App Store app, the constructor for the options class will set appropriate default endpoint values; endpoints must be manually configured when creating a client for a standalone Data Core API instance.


## Basic Client Usage

> You should ensure that you import the `IntelligentPlant.DataCore.Client` namespace in any classes that use the client. This namespace contains extensions methods to simplify the invocation of most API methods. When working with the Industrial App Store, you should also import the `IntelligentPlant.IndustrialAppStore.Client` namespace.


```csharp
using System;
...
using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client;
```

Once you have created the API client, you can use it to call a variety of API methods. API methods are broken down by feature area. For example, functions for reading (or writing) process data are separated from functions for browsing asset models, and so on. Each feature area is represented by a property on the client (e.g. the `DataSources` property is used to expose the API methods for reading process data from/writing process data to data sources such as industrial historians).

All API methods allow a context and a cancellation token to be specified for the operation, as described above. These parameters are optional; the default value for each will be used if not specified (i.e. `default(TContext)` and `default(CancellationToken)`).


## Handling Exceptions

All calls made by the API client will throw a [DataCoreHttpClientException](/src/IntelligentPlant.DataCore.HttpClient/DataCoreHttpClientException.cs) (derived from [System.Net.Http.HttpRequestException](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httprequestexception)) if a non-good HTTP status code is returned. This exception type contains properties describing the HTTP method, URL, and so on.

All Data Core API routes return an [RFC 7807 problem details](https://tools.ietf.org/html/rfc7807) object describing the issue when returning a response with a non-good status code; this is assigned to the `ProblemDetails` property on the `DataCoreHttpClientException` type. The `Detail` property on the problem details object is typically used to provide a human-readable explanation of the problem that occurred.

You can view the definition for the `ProblemDetails` type [here](https://github.com/intelligentplant/ProblemDetails.WebApi/blob/master/ProblemDetails.Core/ProblemDetails.cs).

> Note that the `Detail` property on a problem details object is not guaranteed to be non-null. You should use `string.IsNullOrWhiteSpace(string?)` to determine if there is a value that you can display to an end user. 


## Handling Retries

We recommend using a library such as [Polly](https://github.com/App-vNext/Polly) to create HTTP clients capable of handling scenarios such as transient network interruptions and [API rate limits](https://github.com/App-vNext/Polly/issues/414). Note that, in the event of a `429/Too Many Requests` response being received from the server, the resulting `DataCoreHttpClientException` will have its `UtcRetryAfter` property set to the UTC time that the request can be attempted again at.
