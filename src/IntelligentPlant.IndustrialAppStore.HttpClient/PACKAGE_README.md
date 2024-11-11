# IntelligentPlant.IndustrialAppStore.HttpClient

Extends the [IntelligentPlant.DataCore.HttpClient](https://www.nuget.org/packages/IntelligentPlant.DataCore.HttpClient) library to allow Industrial App Store-specific APIs to be called.

Please consider using one of the following packages instead to simplify configuration of Data Core API client applications:

| Package | App Type | Description |
|---------|----------|-------------|
| [IntelligentPlant.IndustrialAppStore.AspNetCore](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.AspNetCore) | ASP.NET Core | Provides a strongly-typed client for querying the Industrial App Store Data API, an authentication handler that is pre-configured to use the Industrial App Store for authentication, and additional services and middlewares for implementing features such as Content Security Policies. |
| [IntelligentPlant.IndustrialAppStore.CommandLine](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.CommandLine) | CLI | Provides a strongly-typed client for querying the Industrial App Store Data API, and services for authenticating with the Industrial App Store using the OAuth 2.0 device code authorization flow. |
| [IntelligentPlant.IndustrialAppStore.Templates](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.Templates) | | Provides project templates for `dotnet new` and Visual Studio for creating apps that are pre-configured to use one of the two above packages. |


# Getting Started

When creating an Industrial App Store API client that will use a fixed authentication header (e.g. in a [LINQPad](https://www.linqpad.net/) script), the authentication header can be set using the default headers collection of the `HttpClient` that the IAS API client will use:

```csharp
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
    "Bearer",
    "<SOME_TOKEN>"
);

var options = new IndustrialAppStoreHttpClientOptions();
var client = new IndustrialAppStoreHttpClient(httpClient, options);

var dataSources = await client.DataSources.GetDataSourcesAsync();
