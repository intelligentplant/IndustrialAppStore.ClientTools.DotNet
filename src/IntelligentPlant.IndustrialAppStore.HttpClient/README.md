# IntelligentPlant.IndustrialAppStore.HttpClient

A strongly-typed C# client for Intelligent Plant's Industrial App Store API.


## Note

We strongly recommend that ASP.NET Core Industrial App Store applications use the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) library. This simplifies the configuration of the API client and allows easy per-call authentication to IAS APIs.


# Installation

Install the [IntelligentPlant.IndustrialAppStore.HttpClient](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.HttpClient) package by following the instructions on NuGet.org.


# Getting Started

The [IndustrialAppStoreHttpClient<TContext>](./IndustrialAppStoreHttpClientT.cs) class is used to perform Industrial App Store API calls. The class itself has a set of properties (e.g. `DataSources`, `EventSources`) that expose API methods for different feature areas.


## Creating a Client

### Static Authentication Header

When creating an Industrial App Store API client that will use a fixed authentication header (e.g. in a [LINQPad](https://www.linqpad.net/) script, or a console application), the authentication header can be set using the default headers collection of the `HttpClient` that the IAS API client will use:

```csharp
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
    "Bearer",
    "<SOME_TOKEN>"
);

var options = new IndustrialAppStoreHttpClientOptions();
var client = new IndustrialAppStoreHttpClient(httpClient, options);

var dataSources = await client.DataSources.GetDataSourcesAsync();
```


### Dynamic Authentication Header

The second option when creating a client is to dynamically add an authentication header to outgoing requests based on some `context` parameter passed to an API method call. We do this by adding a [DelegatingHandler](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler) to the request pipeline for the `HttpClient` that we pass to the Industrial App Store API client constructor. The handler will receive the `context` parameter passed to the API method call.

> When using the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) library, the `context` that is passed to the API client method calls is the `Microsoft.AspNetCore.Http.HttpContext` for the calling user.

In this example, we will use the `ClaimsPrincipal` class as our context, and attach a different authorization header to requests based on the group memberships of the principal.

```csharp
// Our delegate that will return an authentication header based on a ClaimsPrincipal instance.
AuthenticationCallback<ClaimsPrincipal> authCallback = async (req, ctx, ct) => {
    if (ctx == null) {
        return null;
    }

    var accessToken = GetAccessTokenForPrincipal(ctx);
    if (!string.IsNullOrWhiteSpace(accessToken)) {
        return new AuthenticationHeaderValue(
            "Bearer",
            accessToken
        );
    }

    return null;
};

// We'll use the Microsoft.Extensions.DependencyInjection library to build our client.
var services = new ServiceCollection();

services
    .AddHttpClient<IndustrialAppStoreHttpClient<ClaimsPrincipal>>()
    .AddHttpMessageHandler(() => DataCoreHttpClient.CreateAuthenticationMessageHandler(authCallback));

services.AddSingleton(new IndustrialAppStoreHttpClientOptions());

var serviceProvider = services.BuildServiceProvider();

// Create our client using the service provider.
var client = serviceProvider.GetService<IndustrialAppStoreHttpClient<ClaimsPrincipal>>();
```

When calling API methods using clients created in this way, you just pass in the `ClaimsPrincipal` associated with the call, and the callback will return the authentication header to be added to the outgoing HTTP request. For example:

```csharp
var dataSources = await client.DataSources.GetDataSourcesAsync(WindowsPrincipal.Current);
```


# Calling API Methods

Please refer to the [documentation](/docs/data-core-api-client).

