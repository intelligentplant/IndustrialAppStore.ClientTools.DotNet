# Getting Started


## Concepts


### Call Context

All API methods defined by the API client include a `context` parameter, whose type is defined via the generic `TContext` type used when creating the client object. The purpose of this context is to allow the client to use per-call authentication when making API calls. The Industrial App Store requires that all API calls include an access token used to authenticate and authorise the calling user and app combination; the context object provided to an API method can be used to identify which access token to attach to an outgoing request. For example, when using the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) library in an ASP.NET Core app, the context object is an ASP.NET Core [HttpContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext) object, which is used to obtain the caller's access token from their session data that is set when they log into the app.

Note that, when querying a standalone Data Core API instance, Windows authentication is typically used. In thse circumstances, the context parameter is typically ignored because credentials must be set directly on the `HttpClient` passed to the API client's constructor.


### Client Options

When creating a client object, an options object must be passed to the constructor. This is used to define e.g. the base URL for the Data Core API endpoint. When creating a client for an Industrial App Store app, the constructor for the options class will set appropriate default endpoint values; endpoints must be manually configured when creating a client for a standalone Data Core API instance.


## Basic Client Usage

_You should ensure that you import the `IntelligentPlant.DataCore.Client` namespace in any classes that use the client. This namespace contains extensions methods to simplify the invocation of most API methods. When working with the Industrial App Store, you should also import the `IntelligentPlant.IndustrialAppStore.Client` namespace._

```csharp
using System;
...
using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client;
```

Once you have created the API client, you can use it to call a variety of API methods. API methods are broken down by feature area. For example, functions for reading (or writing) process data are separated from functions for browsing asset models, and so on. Each feature area is represented by a property on the client (e.g. the `DataSources` property is used to expose the API methods for reading process data from/writing process data to data sources such as industrial historians).

All API methods allow a context and a cancellation token to be specified for the operation, as described above. These parameters are optional; the default value for each will be used if not specified (i.e. `default(TContext)` and `default(CancellationToken)`).
