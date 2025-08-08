# EtlSandbox

## 1. Project Purpose & Data Flow

The EtlSandbox project is a microservices-based sandbox for demonstrating complex ETL (Extract, Transform, Load) data pipelines. The core purpose is to showcase how data can be ingested from a primary source, processed, and then fanned out to multiple, diverse data stores.

The primary data flow is as follows:

1.  **`AlphaWorkerService`** extracts `Rental` data from a **MySQL** database (`Jupiter`).
2.  It transforms this data into a flattened `CustomerOrderFlat` structure.
3.  This transformed data is loaded into a central **SQL Server** database (`Mars`).
4.  From this central `Mars` database, several other services read the `CustomerOrderFlat` data and load it into their respective destinations:
    *   **`BetaWorkerService`** reads from a REST API (`BetaWebApi`) that exposes the `Mars` data and loads it into a **PostgreSQL** database (`Neptune`).
    *   **`DeltaWorkerService`** reads directly from the `Mars` database and loads the data into a **ClickHouse** database.
    *   **`GammaWorkerService`** reads from the `Mars` database and loads the data into another **SQL Server** database (`Venus`).

## 2. Architecture & Services

The project uses a containerized microservices architecture orchestrated with Docker and `docker-compose`. Each service has a well-defined role in the data pipeline.

-   **`EtlSandbox.AlphaWorkerService`**: The starting point of the ETL pipeline. It extracts from MySQL, transforms the data, and loads it into a SQL Server (`Mars`) instance.
-   **`EtlSandbox.BetaWebApi`**: A web API that exposes the processed data from the `Mars` SQL Server database via REST endpoints.
-   **`EtlSandbox.BetaWorkerService`**: An ETL service that consumes data from the `BetaWebApi`. This demonstrates a service-to-service data transfer pattern. It loads the data into a PostgreSQL (`Neptune`) database.
-   **`EtlSandbox.DeltaWorkerService`**: An ETL service that demonstrates loading data into a columnar analytics database. It reads from the `Mars` SQL Server database and bulk loads into ClickHouse.
-   **`EtlSandbox.DeltaWebApi`**: A utility API that supports the `DeltaWorkerService` by generating `CREATE TABLE` DDL statements for ClickHouse.
-   **`EtlSandbox.GammaWorkerService`**: An ETL service demonstrating a database-to-database transfer between two similar systems. It reads from the `Mars` SQL Server database and loads into another SQL Server (`Venus`) instance.

## 3. Technologies

-   **Backend**: .NET
-   **API**: ASP.NET Core Web API
-   **Containerization**: Docker, docker-compose
-   **Databases**:
    -   MySQL (`Jupiter`)
    -   SQL Server (`Mars`, `Venus`)
    -   PostgreSQL (`Neptune`)
    -   ClickHouse
-   **ORM/Data Access**:
    -   Entity Framework Core
    -   Dapper
-   **API Client**: Flurl (used in `BetaWorkerService`)
-   **Messaging/CQRS**: MediatR (used for in-process command dispatching)

## 4. Code Structure

The project is organized using a Clean Architecture approach, promoting a clear separation of concerns and establishing a well-defined dependency flow. The solution is divided into four main layers: `Cores`, `Infrastructures`, `Presentations`, and `Hosts`.

### Dependency Flow

The dependencies flow inwards, from the outer layers to the core:

`Hosts` -> `Presentations` -> `Infrastructures` / `Application` -> `Domain`

This ensures that the core business logic (`Domain` and `Application`) is independent of any specific technology or delivery mechanism.

### Layer Breakdown

-   **`Sources/Cores`**: This is the heart of the application, containing the business logic. It is completely self-contained and has no dependencies on other layers.
    -   **`EtlSandbox.Domain`**: Defines the "what" of the business. It contains the core business entities (e.g., `Rental`, `CustomerOrderFlat`) and the abstractions (interfaces) for data access, such as `ISourceRepository` and `IExtractor`.
    -   **`EtlSandbox.Application`**: Orchestrates the domain logic. It defines the application's use cases through MediatR commands (e.g., `InsertCommand`) and their corresponding handlers. It depends on the `Domain` layer but knows nothing about how the data is stored or presented.

-   **`Sources/Infrastructures`**: This layer contains the concrete implementations of the abstractions defined in the `Domain` and `Application` layers. It handles all external concerns, such as databases, file systems, and third-party APIs.
    -   **`EtlSandbox.Infrastructure.Common`**: Holds shared infrastructure code, such as generic repository implementations (`EfDestinationRepositoryV1`), connection factories (`MySqlConnectionFactory`, `SqlServerConnectionFactory`), and REST API clients (`FlurlRestApiClient`).
    -   **`EtlSandbox.Infrastructure.Jupiter`**: Implements data access for the MySQL (`Jupiter`) source. Contains the `JupiterDbContext` and concrete repository/extractor implementations for MySQL.
    -   **`EtlSandbox.Infrastructure.Mars`**: Implements data access for the central SQL Server (`Mars`) database.
    -   **`EtlSandbox.Infrastructure.Neptune`**: Implements data access for the PostgreSQL (`Neptune`) destination.
    -   **`EtlSandbox.Infrastructure.Venus`**: Implements data access for the second SQL Server (`Venus`) destination.

-   **`Sources/Presentations`**: This layer is responsible for presenting the data and handling user interaction. It acts as a bridge between the `Hosts` and the application core.
    -   **`EtlSandbox.Presentation`**: Contains the components that drive the application's behavior, such as the ASP.NET Core controllers (`CustomerOrderFlatsController`) for the Web APIs and the generic background service workers (`InsertWorker`, `SoftDeleteWorker`) for the ETL services.

-   **`Sources/Hosts`**: This is the entry point of the application. Each project in this layer is a runnable application that composes the other layers together.
    -   **`EtlSandbox.AlphaWorkerService`, `BetaWorkerService`, etc.**: These are the executable worker services. Their primary responsibility is to configure and register all the necessary services for dependency injection and to run the application host.
    -   **`EtlSandbox.BetaWebApi`, `DeltaWebApi`**: These are the executable ASP.NET Core Web APIs. They configure the HTTP request pipeline, controllers, and other API-specific services.

## 5. Key Logic & Components

### Data Structures

-   **`Rental`**: The source domain entity, representing data from the MySQL `sakila` database.
-   **`CustomerOrderFlat`**: The transformed and flattened entity that is propagated throughout the rest of the pipeline.

### ETL Processes

Each worker service implements a similar pattern using `InsertWorker` and `SoftDeleteWorker` hosted services, but with different underlying components:

-   **`AlphaWorkerService`**:
    -   **Extractor**: `CustomerOrderFlatJupiterDapperExtractor` (MySQL/Dapper)
    -   **Loader**: `CustomerOrderFlatSqlServerBulkCopyLoader` (SQL Server/BulkCopy)
-   **`BetaWorkerService`**:
    -   **Extractor**: `CustomerOrderFlatRestApiExtractor` (REST API/Flurl)
    -   **Loader**: `CustomerOrderFlatPostgreSqlDapperLoader` (PostgreSQL/Dapper)
-   **`DeltaWorkerService`**:
    -   **Extractor**: `CustomerOrderFlatEfExtractor` (SQL Server/EF Core)
    -   **Loader**: `CustomerOrderFlatClickHouseBulkCopyLoader` (ClickHouse/Custom)
-   **`GammaWorkerService`**:
    -   **Extractor**: `CustomerOrderFlatEfExtractor` (SQL Server/EF Core)
    -   **Loader**: `CustomerOrderFlatSqlServerBulkCopyLoader` (SQL Server/BulkCopy)

## 6. How to Run

1.  **Prerequisites**:
    -   Docker Desktop
    -   .NET SDK
2.  **Build & Run**:
    -   Navigate to the `Sources/Hosts/EtlSandbox.AlphaWorkerService` directory.
    -   Run `docker-compose up --build`. This will build all the service images and start the containers.
3.  **Verify**:
    -   You can connect to the various databases using the connection details in `docker-compose.yml` to observe the data flowing through the system.
    -   The `BetaWebApi` is accessible at `http://localhost:5010/swagger`.

## 7. Future Improvements

The `IMPROVEMENTS.md` file in the root directory contains a detailed list of potential enhancements, including:

-   Adopting an event-driven architecture with Kafka or RabbitMQ.
-   Implementing a comprehensive testing strategy (unit, integration, E2E).
-   Centralizing dependency injection and configuration.
-   Adding a modern observability stack (OpenTelemetry, Serilog, Jaeger, Prometheus).
-   Securing APIs and managing secrets properly.
-   Replacing custom ETL logic with industry-standard tools like Airbyte or Debezium. For large-scale transformations in a local environment, the ELT pattern (Airbyte + dbt) targeting an analytical database like ClickHouse is the recommended approach for performance and scalability.
-   And many more advanced topics covering CI/CD, data governance, and cost optimization.
