# 🍔 Good Hamburger

REST API for order management at a hamburger restaurant, built as a technical challenge using **C# with .NET 10** and **ASP.NET Core**.

> 🇧🇷 [Resumo em Português](#resumo-em-português) disponível no final deste documento.

---

## Table of Contents

- [About](#about)
- [Menu](#menu)
- [Discount Rules](#discount-rules)
- [Architecture Decisions](#architecture-decisions)
- [Project Structure](#project-structure)
- [How to Run](#how-to-run)
- [Running Tests](#running-tests)
- [Endpoints](#endpoints)
- [What Was Left Out](#what-was-left-out)
- [Resumo em Português](#resumo-em-português)

---

## About

A system to register and manage orders for the **Good Hamburger** restaurant, with automatic discount calculation, business rule validations, and menu exposure via REST API. The project was built following Clean Architecture principles with clear layer separation, dependency inversion, and a focus on testability.

---

## Menu

| Product | Category | Price |
|---|---|---|
| X Burger | Sandwich | $ 5.00 |
| X Egg | Sandwich | $ 4.50 |
| X Bacon | Sandwich | $ 7.00 |
| French Fries | Side | $ 2.00 |
| Soft Drink | Drink | $ 2.50 |

---

## Discount Rules

| Combination | Discount |
|---|---|
| Sandwich + Fries + Drink | 20% |
| Sandwich + Drink | 15% |
| Sandwich + Fries | 10% |
| Sandwich only / other combinations | No discount |

Each order may contain **at most one item per category**. Duplicate items return a `400 Bad Request` with a clear error message.

---

## Architecture Decisions

### 1. Layer separation (distinct projects)

The solution is divided into independent projects within the same solution, each with a clear and single responsibility:

- **GoodHamburger.Domain** — entities, enums, and pure domain rules with no external dependencies
- **GoodHamburger.Application** — business rules, services, DTOs, and repository interfaces
- **GoodHamburger.Infrastructure** — database implementation with EF Core and repositories
- **GoodHamburger.IoC** — composition root responsible for registering all dependencies
- **GoodHamburger.API** — controllers, middlewares, and application configuration

### 2. Dependency inversion via dedicated IoC project

The `GoodHamburger.IoC` project is the only one that knows all other layers. The API references only IoC, maintaining the Dependency Inversion Principle: `Application` defines the interfaces (`IOrderRepository`, `IProductRepository`), `Infrastructure` implements them, and `IoC` binds them together — without `Application` ever knowing about EF Core or any persistence detail.

### 3. Domain modeling — Enum over inheritance

Products (sandwiches, sides, and drinks) are represented by a single `Product` class with a `ProductCategory` enum, rather than an inheritance hierarchy. This decision was made because all products share exactly the same attributes (name and price) and have no distinct behaviors between them. Inheritance would add complexity without real benefit for the current scope.

### 4. Menu as a database table with seed data

Initially considered as static data, the menu was moved to a `Products` database table with EF Core seed data. This decision was made because prices and products can change over time, and reading from the database is more realistic and honest to the domain. The seed data populates the table automatically on the first migration, so no manual setup is required.

### 5. Order identification — Guid + OrderNumber

Orders are identified internally by a `Guid`, used in all API routes, to avoid exposing sequential IDs that could be enumerated by malicious clients. A human-friendly `OrderNumber` field (auto-incremented integer) is included in responses for customer-facing display purposes but is never accepted as a route parameter. This gives security without sacrificing usability.

### 6. Isolated discount calculation

The discount logic was isolated in a static `DiscountCalculator` class inside the Application layer, separate from `OrderService`. Being static makes it explicit that this class has no state and no dependencies — pure input/output logic. This makes unit testing straightforward and keeps the business rule explicit and traceable.

### 7. Input/Output DTOs

Two types of DTOs were defined: `OrderRequestDto` (input — what the client sends) and `OrderDto` (output — what the API returns). A single `OrderRequestDto` is used for both Create and Update since both operations require the same input. This avoids over-engineering while maintaining a clean separation between the API contract and the domain model.

### 8. Database — SQLite for development

SQLite was chosen because it requires no server installation, making it ideal for local execution and code evaluation. The database file is created automatically on startup via `db.Database.Migrate()`. Switching to PostgreSQL or SQL Server would only require changing the connection string and the `UseX()` method in the IoC layer, with no impact on any other layer.

### 9. Infrastructure excluded from code coverage

Repository and mapping classes are excluded from code coverage using `[ExcludeFromCodeCoverage]`. These classes are better validated through integration tests against a real database. Unit tests focus on the layers that contain actual business logic: Domain, Application, and API controllers.

### 10. Enum serialization as strings

The JSON serialization was configured to return enum values as strings (`"category": "Sandwich"`) instead of integers (`"category": 0`). This makes the API self-documenting and removes the need for clients to maintain an enum lookup table.

---

## Project Structure

```
GoodHamburger/
├── src/
│   ├── GoodHamburger.API/
│   │   ├── Controllers/
│   │   │   ├── OrdersController.cs
│   │   │   └── MenuController.cs
│   │   ├── Properties/
│   │   │   └── launchSettings.json
│   │   ├── appsettings.json
│   │   └── Program.cs
│   │
│   ├── GoodHamburger.Application/
│   │   ├── DTOs/
│   │   │   ├── OrderDto.cs
│   │   │   ├── OrderItemDto.cs
│   │   │   ├── OrderRequestDto.cs
│   │   │   └── ProductDto.cs
│   │   ├── Interfaces/
│   │   │   ├── IOrderRepository.cs
│   │   │   ├── IOrderService.cs
│   │   │   └── IProductRepository.cs
│   │   └── Services/
│   │       ├── DiscountCalculator.cs
│   │       └── OrderService.cs
│   │
│   ├── GoodHamburger.Domain/
│   │   ├── Entities/
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   └── Product.cs
│   │   └── Enums/
│   │       └── ProductCategory.cs
│   │
│   ├── GoodHamburger.Infrastructure/
│   │   ├── Context/
│   │   │   └── AppDbContext.cs
│   │   ├── Mappings/
│   │   │   ├── OrderItemMapping.cs
│   │   │   ├── OrderMapping.cs
│   │   │   └── ProductMapping.cs
│   │   └── Repositories/
│   │       ├── OrderRepository.cs
│   │       └── ProductRepository.cs
│   │
│   └── GoodHamburger.IoC/
│       └── DependencyInjection.cs
│
└── tests/
    └── GoodHamburger.Tests/
        ├── API/
        │   ├── MenuControllerTests.cs
        │   └── OrdersControllerTests.cs
        ├── Application/
        │   ├── DiscountCalculatorTests.cs
        │   └── OrderServiceTests.cs
        └── Domain/
            └── OrderTests.cs
```

---

## How to Run

### Prerequisites

- [.NET 10 SDK](https://dot.net)
- [Git](https://git-scm.com)

### Steps

**1. Clone the repository**
```bash
git clone https://github.com/your-username/GoodHamburger.git
cd GoodHamburger
```

**2. Run the API**
```bash
dotnet run --project src/GoodHamburger.API
```

**3. Access Swagger**

Open in your browser: [http://localhost:5062/swagger](http://localhost:5062/swagger)

> The SQLite database file (`goodhamburger.db`) is created automatically on the first run via EF Core migrations. No manual database setup is required.

### Troubleshooting

**Reset the database**
```bash
find . -name "*.db" | xargs rm -f
dotnet run --project src/GoodHamburger.API
```

---

## Running Tests

**Run all tests**
```bash
dotnet test
```

**Run with coverage report**
```bash
dotnet test --collect:"XPlat Code Coverage"
```

**Generate HTML coverage report**
```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
open coverage-report/index.html
```

### Test coverage

| Layer | Approach |
|---|---|
| Domain | Unit tests — `Order` computed properties (`Subtotal`, `Total`) |
| Application | Unit tests with Moq — `DiscountCalculator` and `OrderService` |
| API | Unit tests with Moq — `OrdersController` and `MenuController` |
| Infrastructure | Excluded via `[ExcludeFromCodeCoverage]` — better suited for integration tests |

---

## Endpoints

### Menu

| Method | Route | Description |
|---|---|---|
| `GET` | `/menu` | Returns all available products |

### Orders

| Method | Route | Description |
|---|---|---|
| `POST` | `/orders` | Creates a new order |
| `GET` | `/orders` | Lists all orders |
| `GET` | `/orders/{id}` | Gets an order by ID |
| `PUT` | `/orders/{id}` | Updates an existing order |
| `DELETE` | `/orders/{id}` | Removes an order |

### Example — Create order

**Request**
```json
POST /orders
{
  "productIds": [1, 4, 5]
}
```

**Response `201 Created`**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "orderNumber": 1,
  "createdAt": "2026-04-28T10:00:00Z",
  "items": [
    { "productId": 1, "productName": "X Burger", "productPrice": 5.00 },
    { "productId": 4, "productName": "French Fries", "productPrice": 2.00 },
    { "productId": 5, "productName": "Soft Drink", "productPrice": 2.50 }
  ],
  "subtotal": 9.50,
  "discountPercentage": 20,
  "total": 7.60
}
```

**Response `400 Bad Request` — duplicate item**
```json
{
  "error": "Duplicate item: only one Sandwich is allowed per order."
}
```

**Response `404 Not Found` — order not found**
```json
{
  "error": "Order 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found."
}
```

---

## What Was Left Out

| Item | Reason |
|---|---|
| Blazor Frontend | Focus was on backend solidity; Blazor WebAssembly is planned as the next step |
| Authentication / authorization | Out of scope for this challenge |
| Production database | SQLite covers local scope; PostgreSQL would be the choice for production |
| Docker / containerization | Not required, but the architecture supports it without changes to any layer |
| Pagination on listing | Not necessary given the expected order volume in the context of this challenge |
| Integration tests | Infrastructure layer is excluded from coverage; integration tests would be the next step to validate EF Core queries against a real database |

---

## Resumo em Português

### Sobre o projeto

API REST para gerenciamento de pedidos da lanchonete **Good Hamburger**, desenvolvida como desafio técnico em **C# com .NET 10** e **ASP.NET Core**.

### Decisões de arquitetura

A solução foi dividida em cinco projetos independentes dentro da mesma solution: **Domain** (entidades e regras puras), **Application** (regras de negócio e interfaces), **Infrastructure** (EF Core e repositórios), **IoC** (injeção de dependências) e **API** (controllers e configuração). O projeto IoC é o único que conhece todas as camadas, mantendo o Princípio da Inversão de Dependência.

Os produtos foram modelados com uma única classe `Product` e um enum `ProductCategory` (Sandwich, Side, Drink), em vez de herança, pois todos compartilham os mesmos atributos. O cardápio é persistido no banco de dados com dados de seed, populados automaticamente na primeira execução.

Os pedidos utilizam `Guid` como identificador nas rotas da API (por segurança, evitando enumeração sequencial) e um campo `OrderNumber` inteiro e incremental para exibição ao cliente. A lógica de desconto foi isolada em uma classe estática `DiscountCalculator`, separada do `OrderService`, facilitando os testes unitários.

### Testes

Os testes cobrem as camadas de Domain, Application e API com testes unitários usando **xUnit**, **Moq** e **FluentAssertions**. A camada de Infrastructure foi excluída da cobertura via `[ExcludeFromCodeCoverage]`, sendo mais adequada para testes de integração.

### Como executar

```bash
git clone https://github.com/seu-usuario/GoodHamburger.git
cd GoodHamburger
dotnet run --project src/GoodHamburger.API
```

Acesse [http://localhost:5062/swagger](http://localhost:5062/swagger) para explorar a API. O banco de dados SQLite é criado automaticamente na primeira execução.
