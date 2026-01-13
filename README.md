# LegacyOrderService – Developer Assessment

## Overview
This repository contains a refactor version of **LegacyOrderService**, a small .NET 8 console application originally that served as a simple order processor.
The refactor focuses on stabilizing a legacy codebase, preserving existing behavior, and preparing the system for future business growth without over-engineering.
Key improvements include better error handling, clearer separation of concerns, extensible pricing logic, and structured logging.

---

## High-Level Architecture

The solution is split into two projects:

```
src/
 ├─ OrderService.ConsoleApp   // Console app & composition root
 └─ OrderService.Core  // Domain, use cases, abstractions
```

**Dependency direction**

```
ConsoleApp → Core
```

### Core responsibilities

* Business logic and domain rules
* Application use cases implemented with MediatR
* Pricing rules via Strategy pattern
* Cross-cutting logging via MediatR pipeline behaviors

### App responsibilities

* Console input/output
* Dependency injection
* Serilog configuration
* SQLite persistence
* Retry and fail-fast behavior

---

## Design Rules & Principles

* **Commands vs Queries**

  * Commands represent intent to change state (e.g. place or save an order)
  * Queries are read-only and side-effect free

* **Pricing Strategy**

  * Pricing logic is implemented using the Strategy pattern
  * New pricing rules can be added without modifying existing handlers

* **Logging**

  * Centralized via MediatR pipeline
  * Core logs through `ILogger<T>` only
  * Serilog is configured at the application boundary

* **Architecture**

  * Business logic is isolated from infrastructure
  * Dependencies point inward
  * Core is testable without database or console dependencies

---

## How to Run

```bash
dotnet build
dotnet run --project src/OrderService.ConsoleApp
```

The application will prompt for:

* Customer name
* Product name
* Quantity

It will then:

1. Process and price the order
2. Display order details in the console
3. Persist the order to the database

## Testing
Unit tests are included in the `tests/OrderService.Core.Tests` project.
To run the tests:
```bash
dotnet test tests/OrderService.Core.Tests
```

---

## Extending the Solution

The Core project is intentionally designed to be **host-agnostic** and can be reused by different application types without modification.

Typical extensions may include:

* A **Web API** for exposing order operations over HTTP
* A **Worker Service** for background or batch processing
* Other console tools or scheduled jobs

When adding new hosts:

* Reference `OrderService.Core` as the application layer
* Configure dependency injection, logging, and infrastructure at the host boundary
* Use MediatR to invoke existing commands and queries
* Keep business logic and pricing rules inside Core

As the system grows, infrastructure concerns (databases, external services) can be extracted into a dedicated Infrastructure project and shared across multiple hosts.

This approach allows the system to evolve from a simple console application into a service-oriented architecture without rewriting core business logic.
