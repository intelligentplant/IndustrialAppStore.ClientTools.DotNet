# IntelligentPlant.IndustrialAppStore.CommandLine

Defines types and extensions for building command-line applications that interact with the Intelligent Plant Industrial App Store.


# Installation

Install the [IntelligentPlant.IndustrialAppStore.CommandLine](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.CommandLine) package by following the instructions on NuGet.org.


# Getting Started

> The library assumes that your application is using Microsoft.Extensions.DependencyInjection. You can use the services without using the DI system, but you will need to manually create instances of the required types.

First, register CLI services with the dependency injection container:

```csharp
services.AddIndustrialAppStoreCliServices(options => {
    options.ClientId = "<YOUR CLIENT ID>";
    // Relative paths are resolved using the user's local application data folder.
    options.TokenPath = "MyIasApp/tokens";
});
```

Next, build the service provider and ensure that a valid Industrial App Store authentication session is available:

```csharp
var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var sessionManager = scope.ServiceProvider.GetRequiredService<IndustrialAppStoreSessionManager>();
await sessionManager.SignInAsync((request, ct) => {
    Console.WriteLine($"Please sign in by visiting {request.VerificationUri} and entering the following code: {request.UserCode}");
    return default;
});
```

Finally, use the `IndustrialAppStoreHttpClient` service to interact with the Industrial App Store APIs:

```csharp
var client = scope.ServiceProvider.GetRequiredService<IndustrialAppStoreHttpClient>();
var dataSources = await client.DataSources.GetDataSourcesAsync();
```
