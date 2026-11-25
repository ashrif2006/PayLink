# PayLink Software Architecture

## 1. Architecture Overview

PayLink will adopt a Modular Monolith Architecture.

The system will be developed as a single ASP.NET Core Web API application, internally divided into independent business modules. Each module will have a clear responsibility, a well-defined public interface, and its own business logic and tests.

## 2. Technology Stack

- Programming Language: C#
- Backend Framework: ASP.NET Core Web API (.NET 8)
- ORM: Entity Framework Core
- Database: Microsoft SQL Server
- Unit Testing: xUnit
- Containerization: Docker and Docker Compose
- CI/CD: GitHub Actions
- Version Control: Git and GitHub

## 3. System Modules

| Module | Responsibility |
|---|---|
| Accounts and Wallet | Manages customer accounts, wallet balances, and fund reservations. |
| Biller Catalog | Manages available billers and supported services. |
| Payment Processing | Coordinates payment operations and manages the payment lifecycle. |
| Biller Integration | Handles communication with external biller systems. |
| Fees and Commissions | Calculates transaction fees and commissions. |
| Fraud Checks | Validates transactions and detects suspicious activities. |
| Notifications | Sends notifications based on payment events. |
| Reconciliation | Verifies transaction consistency and resolves payment discrepancies. |

## 4. Internal Architecture

The application will be organized into the following logical layers:

- Presentation Layer: Handles HTTP requests and API responses.
- Application Layer: Coordinates use cases and communication between modules.
- Domain Layer: Contains business entities, rules, and domain logic.
- Infrastructure Layer: Handles database access and external service integrations.

These layers will support separation of concerns within the modular monolith.

## 5. Module Communication

Modules will communicate through well-defined interfaces using dependency injection.

Synchronous communication will be used for operations that require immediate results, such as wallet validation, fraud checks, and fee calculation.

Event-driven communication will be used for operations that can be processed independently, such as notifications and reconciliation.

During Phase 2, Notifications and Reconciliation will be evolved using a message broker or event bus to reduce coupling and support asynchronous processing.

## 6. Data Management

Microsoft SQL Server will be used as the main relational database, with Entity Framework Core managing data persistence.

Each module will be responsible for its own business data. Other modules should access or modify that data through the owning module's public interface.

Database transactions and appropriate concurrency controls will be used to maintain wallet consistency and prevent duplicate or conflicting payment operations.

## 7. Architecture Selection Rationale

The Modular Monolith Architecture was selected for the following reasons:

- Simpler development and deployment for a small team.
- Clear separation of business responsibilities.
- Easier testing and debugging.
- Reduced infrastructure complexity.
- Support for transactional consistency.
- Improved maintainability and extensibility.
- Compatibility with the existing Docker and CI/CD setup.
- Ability to evolve selected modules into independent services when necessary.

## 8. Alternative Architecture

Microservices Architecture was considered as an alternative.

Although microservices provide independent deployment and scaling, they introduce additional complexity in distributed communication, data consistency, monitoring, and infrastructure management.

Therefore, a modular monolith is proposed as the initial architecture for PayLink.

## 9. Quality Attributes

- Performance: Efficient internal communication and database operations.
- Availability: Failure handling and isolation of asynchronous operations.
- Maintainability: Clear module boundaries and separation of concerns.
- Modifiability: Support for adding new billers and business rules with minimal changes.
- Testability: Independent testing of business modules.
- Security: Controlled data access and secure configuration management.

## 10. Future Evolution

The architecture will support future improvements, including asynchronous event processing, independent deployment of selected modules, and additional biller integrations.

## 11. Decision Status

Proposed for team review.

This document describes the intended architecture and does not claim that all modules or architectural mechanisms have already been implemented.