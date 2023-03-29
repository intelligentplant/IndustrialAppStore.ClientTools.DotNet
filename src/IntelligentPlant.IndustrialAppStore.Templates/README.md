# IntelligentPlant.IndustrialAppStore.Templates

This project defines [Industrial App Store](https://appstore.intelligentplant.com) templates for Visual Studio 2022, Visual Studio 2019, and the [dotnet new](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new) command.

> Templates installed using the `dotnet new` command can be used in Visual Studio 2022 and Visual Studio 2019 (v16.9 or later).


# Quick Start

1. Run the following command from the command line: `dotnet new --install IntelligentPlant.IndustrialAppStore.Templates`
2. Open Visual Studio, create a new project, and search for "Industrial App Store" in the project templates list.
3. Follow the instructions in your project's `README.md` file to register your app with the Industrial App Store.


# Installing Templates

You can install the Industrial App Store [templates package](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.Templates) as follows:

__Latest Release Version from NuGet.org:__

```
dotnet new --install IntelligentPlant.IndustrialAppStore.Templates
```

__Specific Version from NuGet.org (including pre-release versions):__

```
dotnet new --install IntelligentPlant.IndustrialAppStore.Templates::1.2.3-alpha.4
```

__From Source:__

Alternatively, you can install the templates from source by checking out this repository, building the solution, navigating to the [root templates folder](..) from the command line, and running the following command:

```
dotnet new --install .\
```

Note that, when installing the templates from source, the reference to the `IntelligentPlant.IndustrialAppStore.Authentication` NuGet package in projects generated using templates will specify an incorrect version. You will have to update generated projects to use the correct package version, or replace the package reference with a project reference to the `IntelligentPlant.IndustrialAppStore.Authentication` project in this repository.


# Creating an App using Visual Studio

Before creating a new app, you should [create an app registration](https://appstore.intelligentplant.com/Developer/Settings/Create) on the Industrial App Store. 

Once the template package has been installed, search for `Industrial App Store` in Visual Studio's "Create a new project" window or choose `Industrial App Store` from the project types list:

![Visual Studio template selection window](./img/template_selection.png)

After selecting the template, you will be prompted to select a project location and then optionally enter some details about your app that will be applied to the template:

![Visual Studio template parameters window](./img/template_parameters.png)

The template will create a new ASP.NET Core MVC application that is pre-configured to use the Industrial App Store for authentication. The `README.md` file for the new project provides additional instructions for completing the setup.


# Creating an App using `dotnet new`

If you are not using Visual Studio, you can create a new C# project using the `dotnet new` command from the command line as follows:

```
mkdir MyNewApp.Web
cd MyNewApp.Web
dotnet new iasmvc
```


## Specifying Project Parameters

When creating the project, you can provide several command line parameters to pre-populate items such as the display name used in the app, and the client ID used for authentication. Run `dotnet new iasmvc --help` to see all of the available options. 

Examples:

```
# Sets the app display name and client ID

dotnet new iasmvc --app-name "My First App" --client-id "abcdef0123456789"
```

```
# Creates a project that targets .NET 6 (not compatible with Visual Studio 2019!)

dotnet new iasmvc --Framework net6.0
```

Note that it is not possible to specify the client secret when creating the project; the `README.md` file created by the template in the project folder contains instructions for setting the client secret.


# Writing an On-Premises App

The `README.md` file created by the template in the project folder contains instructions for writing an app that can use an on-premises Data Core API instance instead of (or in addition to) the Industrial App Store, as well as highlighting differences in behaviour between Industrial App Store and on-premises apps. 

You can view the templated version of the templates `README.md` file [here](./templates/iasmvc/README.md). Note that the templated version of the file contains placeholders that are replaced by Visual Studio or `dotnet new` when you create a project from the template!

> If you want to develop an on-premises app (or an app that can be deployed both on-premises and through the Industrial App Store), please [contact Intelligent Plant](https://www.intelligentplant.com/contact-us) to request an on-premises Data Core API installation.
