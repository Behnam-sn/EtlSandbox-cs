# EtlSandbox Project Context

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

The project follows a clean, layered architecture, separating concerns into distinct project types.

-   **`Sources/Cores`**:
    -   `EtlSandbox.Domain`: Defines core business entities (`Rental`, `CustomerOrderFlat`) and repository interfaces.
    -   `EtlSandbox.Application`: Implements application-level logic, including MediatR commands and handlers.
-   **`Sources/Infrastructures`**:
    -   `EtlSandbox.Infrastructure.Common`: Shared infrastructure components like connection factories, transformers, and REST API clients.
    -   `EtlSandbox.Infrastructure.Jupiter`: Data access logic for the source MySQL database.
    -   `EtlSandbox.Infrastructure.Mars`: Data access logic for the central SQL Server database.
    -   `EtlSandbox.Infrastructure.Neptune`: Data access logic for the destination PostgreSQL database.
    -   `EtlSandbox.Infrastructure.Venus`: Data access logic for the second destination SQL Server database.
-   **`Sources/Hosts`**: Contains the runnable applications (worker services and web APIs), each with its own `Dockerfile`.
-   **`Sources/Presentations`**: Contains presentation-layer logic, such as API controllers and the hosted service workers.

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