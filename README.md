# Eclipse

- [Architecture diagram](#architecture-diagram)
- [Modules](#modules)
- [Tests](#tests)
- [Integrations](#integrations)
- [Tech stack](#tech-stack)
- [Deployment and CICD](#deployment-and-cicd)

## Architecture diagram
![Eclipse-Project references drawio](https://github.com/DaniilPoiarkov/Eclipse/assets/101814817/2b033579-b7d2-490f-b93f-6f4b3477f5f8)

## Modules

### WebAPI
Basicly controllers and health-checks

### Pipelines
Contain logic with telegram interaction. In other words it is available for end user functionality

This diagram shows how pipeline concept works and how user message proccedes
![Eclipse-pipeline-flow drawio](https://github.com/DaniilPoiarkov/Eclipse/assets/101814817/38c678c7-864c-4232-98e2-f809da031109)

### Application.Contracts
Provides public API for application use cases

### Application
Contains implementations of public API

### Domain.Shared
Contains domain constants that can be shared thoughout whole solution

### Domain
Contains domain logic of application

### Core
Provides easy API to build and retrieve pipelines.

### Localization
Provides API to add multiple language support for application

### Infrastructure
Basicly contains wrappers with only necessary API

### DataAccess
Data consistency

## Tests
Each Test project reference base Eclipse.Tests class library, that provides helpers which used through all tests.
Each module has own test project.

## Integrations
### API
* Google API
* Telegram API

### Tech stack
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

## Deployment and CICD
* GitHub actions
* Azure AppServices


