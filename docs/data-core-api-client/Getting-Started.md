# Getting Started


## Concepts


### Authentication

All Data Core and Industrial App Store API requests require authentication. When querying the Industrial App Store, bearer tokens are used for authentication. In an on-premises Data API authentication is typically handled using Windows authentication.

The `HttpClient` that is passed to the API client constructor must be configured with the appropriate authentication mechanism. When using the Industrial App Store authentication library in an ASP.NET Core application, the `HttpClient` will be configured to use a bearer token for the calling user.

When using the API client in an on-premises application, you must ensure that the `HttpClient` is configured with the appropriate authentication mechanism for the API instance that you are querying. This is typically Windows authentication, but may be different depending on the configuration of the API instance.


### Client Options

When creating a client object, an options object must be passed to the constructor. This is used to define e.g. the base URL for the Data Core API endpoint. When creating a client for an Industrial App Store app, the constructor for the options class will set appropriate default endpoint values; endpoints must be manually configured when creating a client for a standalone API instance.


## Basic Client Usage

> You should ensure that you import the `IntelligentPlant.DataCore.Client` namespace in any classes that use the client. This namespace contains extensions methods to simplify the invocation of most API methods. When working with the Industrial App Store, you should also import the `IntelligentPlant.IndustrialAppStore.Client` namespace.


```csharp
using System;
...
using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client;
```

Once you have created the API client, you can use it to call a variety of API methods. API methods are broken down by feature area. For example, functions for reading (or writing) process data are separated from functions for browsing asset models, and so on. Each feature area is represented by a property on the client (e.g. the `DataSources` property is used to expose the API methods for reading process data from/writing process data to data sources such as industrial historians).


## Handling Exceptions

All calls made by the API client will throw a [DataCoreHttpClientException](/src/IntelligentPlant.DataCore.HttpClient/DataCoreHttpClientException.cs) (derived from [System.Net.Http.HttpRequestException](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httprequestexception)) if a non-good HTTP status code is returned. This exception type contains properties describing the HTTP method, URL, and so on.

All API routes return an [RFC 7807 problem details](https://tools.ietf.org/html/rfc7807) object describing the issue when returning a response with a non-good status code; this is assigned to the `ProblemDetails` property on the `DataCoreHttpClientException` type. The `Detail` property on the problem details object is typically used to provide a human-readable explanation of the problem that occurred.

You can view the definition for the `ProblemDetails` type [here](https://github.com/intelligentplant/ProblemDetails.WebApi/blob/master/ProblemDetails.Core/ProblemDetails.cs).

> Note that the `Detail` property on a problem details object is not guaranteed to be non-null. You should use `string.IsNullOrWhiteSpace(string?)` to determine if there is a value that you can display to an end user. 


## Handling Retries

We recommend using a library such as [Microsoft.Extensions.Http.Resilience](https://www.nuget.org/packages/Microsoft.Extensions.Http.Resilience) to create HTTP clients capable of handling scenarios such as transient network interruptions and API rate limits. When using the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) library in your application, a standard resilience handler is automatically added to the `HttpClient` injected into the API client.

Note that, in the event of a `429/Too Many Requests` response being received from the server, the resulting `DataCoreHttpClientException` will have its `UtcRetryAfter` property set to the UTC time that the request can be attempted again at.
