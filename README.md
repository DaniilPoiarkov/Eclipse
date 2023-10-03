# Eclipse

- [Architecture diagram](#architecture-diagram)
- [Modules](#modules)
- [Tests](#tests)
- [Integrations](#integrations)
- [Tech stack](#tech-stack)
- [Deployment and CICD](#deployment-and-cicd)

## Architecture diagram
![Eclipse drawio light](https://github.com/DaniilPoiarkov/Eclipse/assets/101814817/e15e8e70-4940-4656-9ffb-8ad608c64e02)

## Modules

### WebAPI
Basicly controllers and health-checks

### Pipelines
Contain logic with telegram interaction. Other words it is available for end user functionality

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

## Deployment and CICD
* GitHub actions
* Azure AppServices


