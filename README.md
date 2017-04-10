# TypeSync
Roslyn-based utility for keeping TypeScript code in sync with C# source in client-server web applications. Can be used for syncing DTOs, API consuming services and more.
Primarily meant to be used for Angular front-end solutions.

## Use cases
1. Generate TypeScript model classes from C# DTO objects.
2. Generate Angular specific Typescript services that can consume ASP.NET Web APIs.
3. Generate TypeScript validators based on server side implemented .NET DTO validation.
4. Scaffold entire Angular project templates based on server side ASP.NET Web APIs.

## Supported features
1. Model generation:
	* Classes.
	* Properties.
	* CLR types.
	* Arrays and collections.
	* Nullable types.
	* Enums.
	* Inheritance.
	* Works with .NET solutions, projects or individual C# files.
	* Support for various symbol/file name cases (PascalCase, camelCase, kebab-case or snake_case).
