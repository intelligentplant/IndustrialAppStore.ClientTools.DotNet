# IntelligentPlant.IndustrialAppStore.CommandLine

This package contains types for authenticating with and calling the Intelligent Plant Industrial App Store API from command-line applications.

Please consider using the [IntelligentPlant.IndustrialAppStore.Templates](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.Templates) package to simplify creation of projects that are pre-configured to use this package.


# Getting Started

First, visit the [Industrial App Store](https://appstore.intelligentplant.com) and create a registration for your app. When you register your app, you can configure the default scopes (i.e. permissions) that the app will request (user info, reading user data sources, etc). You must also enable the Device Code Authorization Flow for your app.

Next, register CLI services with the dependency injection container:

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
