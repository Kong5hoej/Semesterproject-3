# Semesterproject 3 – Trading Forum ("Den grønne avis")

A three-tier trading forum where users post items for sale or trade, follow users and categories, chat with sellers, and get notifications.

## Architecture

```
Blazor client  --HTTP/JSON + JWT-->  ASP.NET Core Web API  --gRPC-->  Java database server  --JPA-->  PostgreSQL
(C#, http://localhost:5132)          (C#, http://localhost:5012)      (Spring Boot, gRPC :9090)      (sep3_tradingforum)
```

| Tier | Folder | Tech | Entry point |
|---|---|---|---|
| Presentation | `csharp_project/Client/BlazorApp` | Blazor Server, Blazor.Bootstrap, .NET 9 | `Program.cs` (http://localhost:5132) |
| Business logic | `csharp_project/WebAPI` + `csharp_project/Server` | ASP.NET Core Web API, JWT auth, gRPC client, .NET 9 | `WebAPI/Program.cs` (http://localhost:5012) |
| Data | `java_project` | Spring Boot 3.5, Spring Data JPA, gRPC 1.75, Java 21 | `src/main/java/post/Server.java` (gRPC on port 9090) |

The gRPC contract is shared by both sides: [java_project/src/main/proto/TradingForum.proto](java_project/src/main/proto/TradingForum.proto).
DTOs between the Blazor client and the Web API live in `csharp_project/Shared/ApiContracts`.

## Prerequisites

- .NET 9 SDK
- JDK 21 and Maven
- PostgreSQL

## Setup

1. **Create the database** in PostgreSQL:
   ```sql
   CREATE DATABASE sep3_tradingforum;
   ```
2. **Create your `.env`** – copy [.env.example](.env.example) to `.env` in the repo root and fill in your PostgreSQL username/password and a random JWT key (at least 32 characters). `.env` is git-ignored; never commit it.
   The database URL (`localhost:5432`) is in [java_project/src/main/resources/application.properties](java_project/src/main/resources/application.properties). Tables are created automatically (`ddl-auto=update`).
3. **Generate the gRPC code**
   - Java: `mvn compile -f java_project/pom.xml`
   - C#: generated automatically on `dotnet build csharp_project/csharp_project.sln`

## Running

Start the tiers in this order, each in its own terminal:

1. **Database server** – run `post.Server` from your IDE (IntelliJ), after `mvn compile`.
   On first start with an empty database it seeds dummy data (categories, posts, a chat, notifications).
2. **Web API**
   ```
   dotnet run --project csharp_project/WebAPI
   ```
3. **Blazor client**
   ```
   dotnet run --project csharp_project/Client/BlazorApp
   ```
   Open http://localhost:5132.

### URLs and ports

| Setting | Where | Default |
|---|---|---|
| gRPC server URL used by the Web API | `csharp_project/WebAPI/appsettings.json` → `gRPC_URL` | `http://localhost:9090` |
| Web API URL used by the client | `csharp_project/Client/BlazorApp/appsettings.json` → `WebAPI_URL` | `http://localhost:5012` |
| JWT issuer/audience | `Jwt` section in both `appsettings.json` files (must match) | `Gruppe5` |
| JWT key, DB username/password | `.env` in the repo root (`Jwt__Key`, `DB_USERNAME`, `DB_PASSWORD`) | – |

### Test users (seeded)

| Username | Password |
|---|---|
| `admin` | `password` |
| `bob` | `password` |

## Tests

`csharp_project/Client/HttpServiceTests` contains NUnit integration tests for the posts HTTP service. They need the database server and Web API running, and the seeded `admin` user. They write to the database.

```
dotnet test csharp_project/Client/HttpServiceTests
```

## Project structure

```
csharp_project/
  Client/BlazorApp/         Blazor UI (pages, auth state provider, HTTP services)
  Client/HttpServiceTests/  NUnit integration tests
  WebAPI/                   Controllers, JWT setup, global exception middleware
  Server/                   Business logic services + post state pattern (Active/Inactive/Closed), gRPC client
  Shared/ApiContracts/      DTOs shared by client and Web API
java_project/
  src/main/proto/           TradingForum.proto (gRPC contract)
  src/main/java/post/
    server/                 gRPC service implementations
    database/               DAOs, Spring Data repositories, seed data (DummyData)
    domain/                 JPA entities
```
