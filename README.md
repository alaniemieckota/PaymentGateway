# Payment Gateway
This is a demo approach to implement PaymentGateway

## Build & Run

### Prerequisites
* .net core 6 [Main Page](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or [Installation Scripts](https://dotnet.microsoft.com/en-us/download/dotnet/scripts)

When .net core 6 is installed  you can execute:
1. open terminal and navigate where `PaymentGateway.sln` file is located
2. to build:
```
dotnet build
```
3. to run test
```
dotnet test
```
4. to start service
```
dotnet run --project PaymentGateway/PaymentGateway.csproj
```
open in browser: `http://localhost:5089/swagger/index.html` so you can manually interact with the API

## Assumptions
* Storage is in memory (for real purposes I would go for Postgresql)
* Only part of the code is covered with tests, as this is only for demo purposes
* Request/Response logs are collected by .net core middleware acitaved by `.AddHttpLogging`
* Validations happens thanks to [FluentValidation](https://github.com/FluentValidation/FluentValidation)
* Logic is delegated from controllers to handlers which are invoked by [MediatR](https://github.com/jbogard/MediatR)
* Authentication is done by dummy approch to recognize a token from header (which can be extended in any direction)