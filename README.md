# Eclipse

- [Getting started](#getting-started)
- [Architecture diagram](#architecture-diagram)
- [Authorization flow](#authorization-flow)
- [Modules](#modules)
- [Tests](#tests)
- [Integrations](#integrations)
- [Tech stack](#tech-stack)
- [Deployment and CICD](#deployment-and-cicd)

## Getting started

### Requirements
* Docker
* ngrok
* .NET 8

<p>Go to <i>docker</i> directory, create here a copy of <i>docker-compose.override.yaml</i> file with name <i>docker-compose.local.yaml</i>.</p>
<p>Follow instructions in this file to build the solution locally.</p>
<p>After entering all required values for your <i>docker-compose.local.yaml</i> file run <i>.\build.ps1</i> script.</p>
<p>It will take a few minutes to start web api as cosmosdb emulator takes a while to prepare itself for work.</p>
<p>Check <i>eclipse-webapi</i> container logs.</p>
<p>Run <i>.\stop.ps1</i> script to stop container. It will also remove image.</p>

## Architecture diagram
![Eclipse-Project references drawio](https://github.com/DaniilPoiarkov/Eclipse/assets/101814817/8c32847f-ecaf-4927-9e24-de2210a353b0)

## Authorization flow
![Eclipse-Authorization flow drawio](https://github.com/user-attachments/assets/9d6e3adc-6c26-4e68-a277-821c9f838ea7)


## Modules

### WebAPI
Basicly controllers and health-checks.

### Pipelines
Contain logic with telegram interaction. In other words it is available for end user functionality.

This diagram shows how pipeline concept works and how user message proccedes:
![Eclipse-pipeline-flow drawio](https://github.com/DaniilPoiarkov/Eclipse/assets/101814817/1b0ce07a-1aa7-4225-b25b-bc96a89e26f2)

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
Built on top of __Microsoft.Extensions.Localization__ engine to work with json files as resource source with bunch of useful stuff.
Additionally provides an api which allows you to use another culture in disposable scope.

### Infrastructure
Basicly contains wrappers with only necessary API and cross-cutting concerns.

### DataAccess
Data persistence with EF Core Cosmos Db provider.

## Tests
<p>Each Test project reference base Eclipse.Tests class library, that provides helpers which used through all tests.</p>
<p>Each module has own test project.</p>
<p>BDD tests written with SpecFlow are all in the single tests assembly.</p>
<p>Integration tests also isolated in separate assembly called <i>Eclipse.IntegrationTests</i>.</p>

## Integrations
### API
* Google API
* Telegram API

### Tech stack
* ASP.NET 8
* Quartz
* Azure CosmosDb
* Polly
* Scrutor
* Serilog
* EFCore
* Redis
* Azure Application Insights
* MiniExcel
* Docker

### Testing
* NSubstitute
* XUnit
* FluentAssertions
* Bogus
* Meziantou
* SpecFlow
* Testcontainers

## Deployment and CICD
* GitHub actions
* Azure AppServices


