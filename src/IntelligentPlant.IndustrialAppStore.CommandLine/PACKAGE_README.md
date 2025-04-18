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
