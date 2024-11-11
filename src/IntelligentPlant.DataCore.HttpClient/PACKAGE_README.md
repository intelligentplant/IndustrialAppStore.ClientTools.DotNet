# IntelligentPlant.DataCore.HttpClient

This package defines types for calling the Intelligent Plant Data Core API, used by [Industrial App Store](https://appstore.intelligentplant.com) apps, and on-premises apps from Intelligent Plant.

Please consider using one of the following packages to simplify configuration of Data Core API client applications:

| Package | App Type | Description |
|---------|----------|-------------|
| [IntelligentPlant.IndustrialAppStore.AspNetCore](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.AspNetCore) | ASP.NET Core | Provides a strongly-typed client for querying the Industrial App Store Data API, an authentication handler that is pre-configured to use the Industrial App Store for authentication, and additional services and middlewares for implementing features such as Content Security Policies. |
| [IntelligentPlant.IndustrialAppStore.CommandLine](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.CommandLine) | CLI | Provides a strongly-typed client for querying the Industrial App Store Data API, and services for authenticating with the Industrial App Store using the OAuth 2.0 device code authorization flow. |
| [IntelligentPlant.IndustrialAppStore.Templates](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.Templates) | | Provides project templates for `dotnet new` and Visual Studio for creating apps that are pre-configured to use one of the two above packages. |

> The packages can also be used to write on-premises apps that interface with a local Data Core API instance.


# Getting Started

The following sections describe how to configure API clients for querying standalone Data Core API instances using static authentication:


## Windows Authentication

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


## Static Authentication Header

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

