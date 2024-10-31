# IntelligentPlant.IndustrialAppStore.DependencyInjection

Types and extension methods to assist with registering Industrial App Store services with Microsoft.Extensions.DependencyInjection.


# Installation

Install the [IntelligentPlant.IndustrialAppStore.DependencyInjection](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.DependencyInjection) package by following the instructions on NuGet.org.


# Getting Started

> For ASP.NET Core applications, use the [IntelligentPlant.IndustrialAppStore.Authentication](../IntelligentPlant.IndustrialAppStore.Authentication) library instead. This includes additional ASP.NET Core-specific features for signing users into your application using the Industrial App Store.

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
