# IntelligentPlant.IndustrialAppStore.Templates

This project defines [Industrial App Store](https://appstore.intelligentplant.com) templates for the [dotnet new](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new) command.


# Installing Templates

You can install the [Industrial App Store templates](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.Templates) for `dotnet new` as follows:

__Latest Version:__

```
dotnet new --install IntelligentPlant.IndustrialAppStore.Templates
```

__Specific Version:__

```
dotnet new --install IntelligentPlant.IndustrialAppStore.Templates::1.2.3
```

__From Source:__

Alternatively, you can install the template from source by checking out this repository, building the solution, navigating to the [root templates folder](/src/IntelligentPlant.IndustrialAppStore.Templates) from the command line, and running the following command:

```
dotnet new --install .\
```

Note that, when installing the template from source, the reference to the `IntelligentPlant.IndustrialAppStore.Authentication` NuGet package in projects generated using the template may use an incorrect version. You will have to update generated projects to use the correct package version, or replace the package reference with a project reference to the `IntelligentPlant.IndustrialAppStore.Authentication` project in this repository.


# Creating an App using a Template

Before creating a new app, you should [create an app registration](https://appstore.intelligentplant.com/Developer/AddApplication) on the Industrial App Store. After your app registration has been completed, you can create a new C# project using the `dotnet new` command from the command line as follows:

```
mkdir MyNewApp.Web
cd MyNewApp.Web
dotnet new iasmvc
```

This will create a new ASP.NET Core MVC application that is pre-configured to use the Industrial App Store for authentication. The `README.md` file for the new project provides additional instructions for completing the setup.

You can open the project in Visual Studio by double clicking it. In the future, you will be able to [use the template from inside Visual Studio](https://devblogs.microsoft.com/dotnet/net-cli-templates-in-visual-studio/).


## Creating a Visual Studio Solution

The above steps create a `.csproj` file and associated source files that can be compiled and run using the `dotnet` command. To create a Visual Studio solution file containing the project, you can follow these steps instead:

Create solution file:

```
mkdir MyNewApp
cd MyNewApp
dotnet new sln
```

Create project:

```
mkdir MyNewApp.Web
cd MyNewApp.Web
dotnet new iasmvc
```

Add project to solution:

```
cd ..
dotnet sln add ./MyNewApp.Web/MyNewApp.Web.csproj
```

## Specifying Project Parameters

When creating the project, you can provide several command line parameters to pre-populate items such as the display name used in the app, and the client ID used for authentication. Run `dotnet new iasmvc --help` to see all of the available options. 

Examples:

```
# Sets the app display name and client ID

dotnet new iasmvc --app-name "My First App" --client-id "abcdef0123456789"
```

```
# Specifies the local HTTPS port to use instead of randomly choosing a port.

dotnet new iasmvc --port 43789
```

Note that it is not possible to specify the client secret when creating the project; the `README.md` file created by the template in the project folder contains instructions for setting the client secret.
