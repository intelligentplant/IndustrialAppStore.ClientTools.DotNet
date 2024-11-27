# IntelligentPlant.IndustrialAppStore.HttpClient

A strongly-typed C# client for Intelligent Plant's Industrial App Store API.


## Note

We strongly recommend that ASP.NET Core Industrial App Store applications use the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) library. This simplifies the configuration of the API client and allows easy per-call authentication to IAS APIs.


# Installation

Install the [IntelligentPlant.IndustrialAppStore.HttpClient](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.HttpClient) package by following the instructions on NuGet.org.


# Getting Started

The [IndustrialAppStoreHttpClient](./IndustrialAppStoreHttpClient.cs) class is used to perform Industrial App Store API calls. The class itself has a set of properties (e.g. `DataSources`, `EventSources`) that expose API methods for different feature areas.


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

The second option when creating a client is to dynamically add an authentication header to outgoing requests. This can be done by adding a [DelegatingHandler](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler) to the request pipeline for the `HttpClient` that is passed to the Industrial App Store API client constructor. 

When writing an ASP.NET Core application, the [IntelligentPlant.IndustrialAppStore.Authentication](../IntelligentPlant.IndustrialAppStore.Authentication) library can be used to create API client instances that authenticate outgoing requests using an access token issued to the current user.


# Calling API Methods

Please refer to the [documentation](/docs/data-core-api-client).

