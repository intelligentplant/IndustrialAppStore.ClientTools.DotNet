# IntelligentPlant.IndustrialAppStore.DependencyInjection

This package defines types and extension methods to assist with registering Industrial App Store services with [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection).

Please consider using one of the following packages instead of using this package directly:

| Package | App Type | Description |
|---------|----------|-------------|
| [IntelligentPlant.IndustrialAppStore.AspNetCore](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.AspNetCore) | ASP.NET Core | Provides a strongly-typed client for querying the Industrial App Store Data API, an authentication handler that is pre-configured to use the Industrial App Store for authentication, and additional services and middlewares for implementing features such as Content Security Policies. |
| [IntelligentPlant.IndustrialAppStore.CommandLine](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.CommandLine) | CLI | Provides a strongly-typed client for querying the Industrial App Store Data API, and services for authenticating with the Industrial App Store using the OAuth 2.0 device code authorization flow. |
| [IntelligentPlant.IndustrialAppStore.Templates](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.Templates) | | Provides project templates for `dotnet new` and Visual Studio for creating apps that are pre-configured to use one of the two above packages. |

> The packages can also be used to write on-premises apps that interface with a local Data Core API instance.


# Getting Started

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

