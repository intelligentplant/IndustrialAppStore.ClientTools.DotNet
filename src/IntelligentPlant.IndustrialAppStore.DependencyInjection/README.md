# IntelligentPlant.IndustrialAppStore.DependencyInjection

Types and extension methods to assist with registering Industrial App Store services with Microsoft.Extensions.DependencyInjection.


# Installation

Install the [IntelligentPlant.IndustrialAppStore.DependencyInjection](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.DependencyInjection) package by following the instructions on NuGet.org.


# Getting Started

> For ASP.NET Core applications, use the [IntelligentPlant.IndustrialAppStore.AspNetCore](../IntelligentPlant.IndustrialAppStore.AspNetCore) library instead. This includes additional ASP.NET Core-specific features for signing users into your application using the Industrial App Store.

> For command-line applications, use the [IntelligentPlant.IndustrialAppStore.CommandLine](../IntelligentPlant.IndustrialAppStore.CommandLine) library instead. This includes additional features for authenticating with the Industrial App Store using the OAuth 2.0 device code authorization flow.

To register a scoped `IndustrialAppStoreHttpClient` service with an `IServiceCollection`, call the `AddIndustrialAppStoreServices` extension method:

```csharp
var builder = services.AddIndustrialAppStoreApiServices();
```

This returns an `IIndustrialAppStoreBuilder` object that can be used to customise the registered services via additional extension method calls. For example, to customise the `HttpClient` used by the `IndustrialAppStoreHttpClient` service:

```csharp
var builder = services.AddIndustrialAppStoreApiServices(configureHttpBuilder: http => http.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler() {
    EnableMultipleHttp2Connections = true
}));
```

Once you have built your container, you can inject the `IndustrialAppStoreHttpClient` service into your application components as required.

> Remember that `IndustrialAppStoreHttpClient` is registered as a _scoped_ service.


# Authentication

By default, the scoped `AccessTokenProvider` service is used to provide the Industrial App Store access token for the `IndustrialAppStoreHttpClient` service.

The default `AccessTokenProvider` service registration does not return an access token, meaning that calls to Industrial App Store APIs will fail. To configure an `AccessTokenProvider` service that returns an access token, you can do one of the following:


## Replacing the default `AccessTokenProvider` registration

You can replace the default `AccessTokenProvider` registration with a custom implementation that returns an access token. For example:

```csharp
var builder = services
    .AddIndustrialAppStoreApiServices()
    .AddAccessTokenProvider((IServiceProvider provider) => {
        var accessTokenService = provider.GetRequiredService<MyTokenService>();
        return (CancellationToken ct) => accessTokenService.GetAccessTokenAsync(ct);
    });
```

## Manually setting the access token factory

After creating a new `IServiceScope`, you can retrieve the `AccessTokenProvider` service from the service provider and set the `AccessTokenProvider.Factory` property to a factory method that returns an access token. For example:

```csharp
using var scope = serviceProvider.CreateScope();

var accessTokenProvider = scope.ServiceProvider.GetRequiredService<AccessTokenProvider>();
accessTokenProvider.Factory = (CancellationToken ct) => new ValueTask<string?>("my-access-token");

var client = scope.ServiceProvider.GetRequiredService<IndustrialAppStoreHttpClient>();
var dataSources = await client.DataSources.GetDataSourcesAsync();
```
