# Industrial App Store Client Tools

This repository contains client tools and templates for writing [Industrial App Store](https://appstore.intelligentplant.com) apps using C# and ASP.NET Core.


# Installing Templates

You can install the [Industrial App Store templates](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.Templates) for `dotnet new` as follows:

```
dotnet new --install IntelligentPlant.IndustrialAppStore.Templates
```


# Creating an App using a Template

Befor creating a new app, you should [create an app registration](https://appstore.intelligentplant.com/Developer/AddApplication) on the Industrial App Store. After your app registration has been completed, you can create a new C# project using the `dotnet new` command from the command line as follows:

```
mkdir MyNewApp
cd MyNewApp
dotnet new iasmvc
```

This will create a new ASP.NET Core MVC application that is pre-configured to use the Industrial App Store for authentication. The `README.md` file for the new project provides additional instructions for completing the setup.

When creating the project, you can provide several command line parameters to pre-populate items such as the display name used in the app, and the client ID secret key used for authentication. Run `dotnet new iasmvc --help` to see all of the available options. 

Examples:

```
# Sets the app display name, client ID, and client secret used for 
# authentication.
dotnet new iasmvc --app-name "My First App" --client-id "my_client_id" --client-secret "my_client_secret"

# Uses the Proof Key for Code Exchange (PKCE) extension to the OAuth 
# authorization code flow instead of a secret key.
dotnet new iasmvc --app-name "My First App" --client-id "my_client_id" --pkce

# Specifies the local HTTPS port to use instead of randomly choosing a port.
dotnet new iasmvc --port 43789
```
