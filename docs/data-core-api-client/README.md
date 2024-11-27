# Industrial App Store/Data Core API Client

This README describes how to query the Industrial App Store using the .NET client for Intelligent Plant's Data Core API. 


# What is the Industrial App Store?

The [Industrial App Store](https://appstore.intelligentplant.com) (IAS) is a cloud-based platform offering apps for analysing and visualising real-time process data, and alarm & event data. Instead of being uploaded to the cloud for storage, data is stored on client networks, and connected to the IAS using a tool called [App Store Connect](https://appstore.intelligentplant.com/Home/AppProfile?appId=a73c453df5f447a6aa8a08d2019037a5).


# What is Data Core?

Data Core is Intelligent Plant's data access API for industrial process and alarm & event data. It is primarily used by apps on the [Industrial App Store](https://appstore.intelligentplant.com), but can also be used by on-premises applications to access data via the local network (see below for details).


# Configuration

Please refer to the list below for details on how to configure the API client:

- IAS applications written with ASP.NET Core should use the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) library. Apps created using the [IAS ASP.NET Core template](/src/IntelligentPlant.IndustrialAppStore.Templates) are pre-configured to use this library.
- IAS applications written in .NET without ASP.NET Core should use the [IntelligentPlant.IndustrialAppStore.HttpClient](/src/IntelligentPlant.IndustrialAppStore.HttpClient) library.

You can also create applications that run against an on-premises instance of the Data Core API. The easiest way to do this is to create a project using the [IAS ASP.NET Core template](/src/IntelligentPlant.IndustrialAppStore.Templates) and following the instructions in your project's README file. See below for important differences between IAS apps and on-premises apps.

> If you want to develop an on-premises app (or an app that can be deployed both on-premises and through the Industrial App Store), please [contact Intelligent Plant](https://www.intelligentplant.com/contact-us) to request an on-premises Data Core API installation.


# API Client Operations

Documentation about specific API operations can be found in the following pages:

- [Getting Started](./Getting-Started.md)
- [Requesting Data Sources](./Requesting-Data-Sources.md)
- [Tag Searches](./Tag-Searches.md)
- [Reading Tag Values](./Reading-Tag-Values.md)
- [Writing Tag Values](./Writing-Tag-Values.md)
- [Industrial App Store Operations](./Industrial-App-Store-Operations.md)


# Differences Between Industrial App Store and On-Premises Apps

> The [IAS ASP.NET Core template](/src/IntelligentPlant.IndustrialAppStore.Templates) makes it easy to build, debug and deploy Industrial App Store and on-premises versions of the same app. We strongly recommend that you use this template.


## App Authentication

When running in on-premises mode, you must supply your own authentication mechanism. The app template referenced above makes it easy to switch between using the Industrial App Store for authentication when running in IAS mode, and using Windows authentication via IIS when running in on-premises mode.


## API Availability

The Industrial App Store defines APIs for organisation and user information queries. These APIs are not available when running in on-premises mode, and attempts to call these APIs will throw errors. You must account for these differences yourself.


## API Authentication and Authorization

When running in Industrial App Store mode, your app will automatically add a bearer token to outgoing requests made on behalf of the calling user, allowing the Industrial App Store to authenticate and authorize the request on a per-user basis. When running in on-premises mode, you must manually add an authentication mechanism to the `HttpClient` used by the API client. This is typically Windows authentication, but may be different depending on the configuration of the Data Core API instance you are querying.

> Note that the differences in API authentication mean that it may not be possible to perform per-user authentication when calling the on-premises Data Core API. You should be prepared to apply your own authorization scheme when running in on-premises mode in order to restrict access to the data sources configured in the Data Core API where appropriate.
