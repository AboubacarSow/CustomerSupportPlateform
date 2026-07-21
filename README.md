# Customer Support Platform

Lightweight .NET customer support platform implementing a layered architecture (API, Application, Domain, Infrastructure).

## Overview
This repository contains a reference implementation of a customer support backend with components for ingestion, embeddings, vector search, and chat completions.

## Repository Structure
- `src/CustomerSupportPlateform.API` — API / presentation layer
- `src/CustomerSupportPlateform.Application` — business logic, CQRS, DTOs
- `src/CustomerSupportPlateform.Domain` — domain entities and core models
- `src/CustomerSupportPlateform.Infrastructure` — persistence, embeddings, chat completions, storage
- `infrastructure/docker-compose.yml` — optional services used for local development (database, vector store, etc.)

## Prerequisites
- .NET 9 SDK
- Docker & Docker Compose (recommended for local services)
- PostgreSQL (if running without Docker)

## Local Development
1. Restore and build:

```sh
	dotnet restore
	dotnet build
```

2. (Optional) Start supporting services with Docker Compose:
```sh
	docker-compose -f infrastructure/docker-compose.yml up -d
```

3. Configure connection strings / settings in `src/CustomerSupportPlateform.API/appsettings.Development.json` as needed.

4. Run the API project:
```sh
	cd src/CustomerSupportPlateform.API
	dotnet run
```


## Database / Migrations
Migrations are kept in the infrastructure/persistence areas — apply them using the EF Core tools if required for your environment.

## Contributing
Contributions welcome — please open issues or pull requests for enhancements or bug fixes.

## License
See LICENSE or add one as appropriate for your project.