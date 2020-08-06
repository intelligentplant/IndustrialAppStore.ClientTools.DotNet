# Data Core API Client


## What is Data Core?

Data Core is Intelligent Plant's data access API for industrial process data. It is primarily used by apps on the [Industrial App Store](https://appstore.intelligentplant.com) (IAS), but can also be used to integrate with standalone Intelligent Plant applications. Note that IAS apps should use the [IntelligentPlant.IndustrialAppStore.HttpClient](/src/IntelligentPlant.IndustrialAppStore.HttpClient) or [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) libraries, which extend this library by providing additional IAS-specific API methods, and hooks for authenticating all outgoing API calls.


## What is the Industrial App Store?

The [Industrial App Store](https://appstore.intelligentplant.com) (IAS) is a cloud-based platform offering apps for analysing and visualising real-time process data, and alarm & event data. Instead of being uploaded to the cloud for storage, data is stored on client networks, and connected to the IAS using a tool called [App Store Connect](https://appstore.intelligentplant.com/Home/AppProfile?appId=a73c453df5f447a6aa8a08d2019037a5).


## Configuration

Please refer to the list below for details on how to configure the API client:

- IAS applications written with ASP.NET Core should use the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) library. Apps created using the [IAS ASP.NET Core template](/src/IntelligentPlant.IndustrialAppStore.Templates) are pre-configured to use this library.
- IAS applications written in .NET without ASP.NET Core should use the [IntelligentPlant.IndustrialAppStore.HttpClient](/src/IntelligentPlant.IndustrialAppStore.HttpClient) library.
- Applications for querying standalone Data Core API nodes should use the [IntelligentPlant.DataCore.HttpClient](/src/IntelligentPlant.DataCore.HttpClient) library.


## API Client Operations

- [Getting Started](./Getting Started.md)
- [Requesting Data Sources](./Requesting Data Sources.md)
- [Tag Searches](./Tag Searches.md)
- [Reading Tag Values](./Reading Tag Values.md)
- [Writing Tag Values](./Writing Tag Values.md)
- [Industrial App Store Operations](./Industrial App Store Operations.md)
