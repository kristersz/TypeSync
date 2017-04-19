# TypeSync
Roslyn-based utility for keeping TypeScript code in sync with C# source in client-server web applications. Can be used for syncing DTOs, API consuming services and more.
Primarily meant to be used for Angular front-end solutions.

## Use cases
1. Generate TypeScript model classes from C# DTO objects.
2. Generate Angular specific Typescript services that can consume ASP.NET Web APIs.
3. Generate TypeScript validators based on server side implemented .NET DTO validation.
4. Scaffold entire Angular project templates based on server side ASP.NET Web APIs.

## Supported features
* Works with .NET solutions, projects or individual C# files.
* Support for various symbol/file name cases (PascalCase, camelCase, kebab-case or snake_case).

1. Model generation:
	* Classes.
	* Properties.
	* CLR types.
	* Arrays and collections.
	* Nullable types.
	* Enums.
	* Inheritance.
2. Web client generation:
	* Web API Controllers as Angular services.
	* Public controller methods as wrapped Angular Http service functions.
	* Supports attribute routing.
	* Strongly typed return types and parameters.

## System requirements
TypeSync uses MSBuildWorkspace to open and build .NET solutions and projects.
MSBuildWorkspace directly or indirectly depends on being able to find the location of MSBuild binaries, settings, and targets in the registry. This was fine for MSBuild 14.0, but no such information is available for 15.0 (by design).

In case interactions with MSBuildWorkspace are failing, you need to do the following:
* Add the Microsoft.Build and Microsoft.Build.Tasks.Core packages from NuGet to TypeSync console app project.
* Add the MSBuild assembly binding redirects to TypeSync console app.config.

In case of VS 2015:
```xml
<?xml version ="1.0"?>
<configuration>
    <runtime>
        <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
            <dependentAssembly>
                <assemblyIdentity name="Microsoft.Build.Framework" publicKeyToken="b03f5f7f11d50a3a" culture="neutral"/>
                <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="14.0.0.0"/>
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name="Microsoft.Build.Engine" publicKeyToken="b03f5f7f11d50a3a" culture="neutral"/>
                <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="14.0.0.0"/>
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name="Microsoft.Build" publicKeyToken="b03f5f7f11d50a3a" culture="neutral"/>
                <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="14.0.0.0"/>
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name="Microsoft.Build.Conversion.Core" publicKeyToken="b03f5f7f11d50a3a" culture="neutral"/>
                <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="14.0.0.0"/>
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name="Microsoft.Build.Tasks.Core" publicKeyToken="b03f5f7f11d50a3a" culture="neutral"/>
                <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="14.0.0.0"/>
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name="Microsoft.Build.Utilities.Core" publicKeyToken="b03f5f7f11d50a3a" culture="neutral"/>
                <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="14.0.0.0"/>
            </dependentAssembly>
        </assemblyBinding>
    </runtime>
</configuration>
```

In case of VS 2017:
```xml
<?xml version="1.0" encoding="utf-8"?>
  <configuration>
    <runtime>
      <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
        <dependentAssembly>
          <assemblyIdentity name="Microsoft.Build.Framework" culture="neutral" publicKeyToken="b03f5f7f11d50a3a" />
          <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="15.1.0.0" />
        </dependentAssembly>
        <dependentAssembly>
          <assemblyIdentity name="Microsoft.Build" culture="neutral" publicKeyToken="b03f5f7f11d50a3a" />
          <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="15.1.0.0" />
        </dependentAssembly>
        <dependentAssembly>
          <assemblyIdentity name="Microsoft.Build.Conversion.Core" culture="neutral" publicKeyToken="b03f5f7f11d50a3a" />
          <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="15.1.0.0" />
        </dependentAssembly>
        <dependentAssembly>
          <assemblyIdentity name="Microsoft.Build.Tasks.Core" culture="neutral" publicKeyToken="b03f5f7f11d50a3a" />
          <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="15.1.0.0" />
        </dependentAssembly>
        <dependentAssembly>
          <assemblyIdentity name="Microsoft.Build.Utilities.Core" culture="neutral" publicKeyToken="b03f5f7f11d50a3a" />
          <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="15.1.0.0" />
        </dependentAssembly>
        <dependentAssembly>
          <assemblyIdentity name="Microsoft.Build.Engine" culture="neutral" publicKeyToken="b03f5f7f11d50a3a" />
          <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="15.1.0.0" />
        </dependentAssembly>
        <dependentAssembly>
          <assemblyIdentity name="Microsoft.Build.Conversion.Core" culture="neutral" publicKeyToken="b03f5f7f11d50a3a" />
          <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="15.1.0.0" />
        </dependentAssembly>
        <dependentAssembly>
          <assemblyIdentity name="Microsoft.Activities.Build" culture="neutral" publicKeyToken="31bf3856ad364e35" />
          <bindingRedirect oldVersion="4.0.0.0" newVersion="15.0.0.0" />
        </dependentAssembly>
        <dependentAssembly>
          <assemblyIdentity name="XamlBuildTask" culture="neutral" publicKeyToken="31bf3856ad364e35" />
          <bindingRedirect oldVersion="4.0.0.0" newVersion="15.0.0.0" />
        </dependentAssembly>
    </runtime>
  </configuration>

```

Related GitHub Roslyn issues:
* <https://github.com/dotnet/roslyn/issues/17401>
* <https://github.com/dotnet/roslyn/issues/15056>
