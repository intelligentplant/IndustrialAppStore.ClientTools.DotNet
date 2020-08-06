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


# Calling API Methods

Please refer to the [documentation](/docs/Data Core API Client).
