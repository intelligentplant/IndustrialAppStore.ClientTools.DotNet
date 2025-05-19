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
    // Base folder for application data such as encrypted authentication 
    // tokens. Relative paths are resolved using the user's local 
    // application data folder.
    options.AppDataPath = "MyIasApp";
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


# Persisting Application Data

If your application persists data to disk, you can use the `AppDataFolderProvider` service to get the base path for your app's data folder:

```csharp
var appDataDir = scope.ServiceProvider.GetRequiredService<AppDataFolderProvider>().AppDataFolder;
var myDataDir = appDataDir.CreateSubdirectory("MyData");
```

Encrypted authentication tokens are saved to a sub-folder of the base folder provided by `AppDataFolderProvider`.


## Specifying an Application Discriminator for Encryption

The `IndustrialAppStoreSessionManager` type uses [ASP.NET Core Data Protection](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction) to encrypt authentication tokens at rest. When using Microsoft.Extensions.Hosting, the application discriminator for the data protection system (used to isolate protected data even when apps share the same keys) is derived from the content root path for the host by default. 

The content root path defaults to the directory containing the application assembly. This means that if you run multiple instances of your app from different directories, they will not be able to decrypt each other's tokens. If you want to share tokens between different instances of your app regardless of the content root path, you can configure the application name to use with the data protection system:

```csharp
builder.Services
    .AddDataProtection()
    .SetApplicationName("MyIasApp");
```

If you are not using Microsoft.Extensions.Hosting and are instead manually creating an `IServiceCollection`, no application discriminator is set by default. You should set an application name for your app as shown above to ensure that the data protection system isolates protected data specific to your application.
