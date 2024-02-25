# Eclipse

- [Architecture diagram](#architecture-diagram)
- [Modules](#modules)
- [Tests](#tests)
- [Integrations](#integrations)
- [Tech stack](#tech-stack)
- [Deployment and CICD](#deployment-and-cicd)

## Architecture diagram
![Eclipse-Project references drawio](https://github.com/DaniilPoiarkov/Eclipse/assets/101814817/175eb771-eb54-4ad6-9f31-980345aa9000)

## Modules

### WebAPI
Basicly controllers and health-checks.

### Pipelines
Contain logic with telegram interaction. In other words it is available for end user functionality.

This diagram shows how pipeline concept works and how user message proccedes:
![Eclipse-pipeline-flow drawio](https://github.com/DaniilPoiarkov/Eclipse/assets/101814817/38c678c7-864c-4232-98e2-f809da031109)

### Application.Contracts
Provides public API for application use cases.

### Application
Contains implementations of use cases.

### Domain.Shared
Contains domain constants (like enums, const values) that can be shared thoughout whole solution.

### Domain
Contains domain logic of application.

### Core
Provides easy API to build and retrieve pipelines.

### Localization
Provides API for multiple language support in application. As Microsoft.Localization is not very sutable for this purposes and works with XML schemas I desided to create own engine. Here we have ability to dynamicly set culture for localization (Without taking it from request headers) and using json as a schema for resoures. Planning to combine this engine and Microsoft.Localization abstractions.

### Infrastructure
Basicly contains wrappers with only necessary API and cross-cutting concerns.

### DataAccess
Data consistency.

## Tests
Each Test project reference base Eclipse.Tests class library, that provides helpers which used through all tests.
Each module has own test project.
BDD tests written with SpecFlow are all in the single tests assembly.

## Integrations
### API
* Google API
* Telegram API

### Tech stack
* ASP.NET 8
* Quartz
* FluentValidation
* Azure CosmosDb
* Polly
* Scrutor
* Serilog

### Testing
* NSubstitute
* XUnit
* FluentAssertions
* Bogus
* Meziantou
* SpecFlow

## Deployment and CICD
* GitHub actions
* Azure AppServices


