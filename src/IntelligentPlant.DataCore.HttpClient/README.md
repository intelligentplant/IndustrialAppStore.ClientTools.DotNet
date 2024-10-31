# IntelligentPlant.DataCore.HttpClient

A strongly-typed C# client for Intelligent Plant's Data Core API.


# Installation

Install the [IntelligentPlant.DataCore.HttpClient](https://www.nuget.org/packages/IntelligentPlant.DataCore.HttpClient) package by following the instructions on NuGet.org.


# Getting Started

The abstract [DataCoreHttpClient<TContext, TOptions>](./DataCoreHttpClient.cs) class is used to perform Data Core API calls. The class itself has a set of properties (e.g. `DataSources`, `EventSources`) that expose API methods for different feature areas. Additional concrete subclasses (`DataCoreHttpClient<TContext>` and `DataCoreHttpClient`) are available to simplify configuration depending on the type of authentication used by the app.

Note that, if you are writing an app that interfaces with the Industrial App Store, you should use the client class from the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication/IndustrialAppStoreHttpClient.cs) or [IntelligentPlant.IndustrialAppStore.HttpClient](/src/IntelligentPlant.IndustrialAppStore.HttpClient/IndustrialAppStoreHttpClientT.cs) library instead. Please refer to the documentation for those libraries for details of how to configure their respective clients.


## Creating a Client

_This section describes creating clients for querying standalone Data Core API instances. To create a client for an Industrial App Store app, refer to the documentation in the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) or [IntelligentPlant.IndustrialAppStore.HttpClient](/src/IntelligentPlant.IndustrialAppStore.HttpClient) library instead._


### Windows Authentication

To create a Data Core API client for querying a standalone Data Core API instance using Windows authentication, use the `DataCoreHttpClient` class, ensuring that the `HttpClient` you provide has the appropriate credentials configured:

```csharp
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

The third option when creating a client is to dynamically add an authentication header to outgoing requests. This can be done by adding a [DelegatingHandler](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler) to the request pipeline for the `HttpClient` that we pass to the Data Core client constructor. The handler can be created with a reference to a service type that can provide the necessary context.

This is the approach used when creating API clients in [ASP.NET Core applications](../IntelligentPlant.IndustrialAppStore.Authentication/Http/TokenStoreAuthenticationHandler.cs).


# Calling API Methods

Please refer to the [documentation](/docs/data-core-api-client).
