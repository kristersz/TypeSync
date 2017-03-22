# TypeSync
Roslyn-based utility for keeping TypeScript code in sync with C# source in client-server web applications. Can be used for syncing DTOs, API consuming services and more.
Primarily meant to be used for Angular front-end solutions.

## Use cases
1. Generating TypeScript model classes from C# DTO objects.
2. Generating Angular specific Typescript services that can consume ASP.NET Web APIs.
3. Generating TypeScript validators based on server side implemented .NET DTO validation.
4. Scaffolding entire Angular project templates based on server side ASP.NET Web APIs.

## Supported features
1. DTO generation:
	* Classes.
	* Properties (identifiers and predefined types).
