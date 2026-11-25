# ADR-001: Architecture Style

## Status

Proposed for team review.

## Context

PayLink is a bill payment and digital wallet backend that integrates with multiple external billers and supports several business modules.

The system requires reliable payment processing, wallet consistency, maintainability, testability, and support for future changes.

The current technology stack consists of ASP.NET Core Web API (.NET 8), Entity Framework Core, Microsoft SQL Server, Docker, and GitHub Actions.

The architecture must support the project requirements while remaining manageable for a small development team.

## Considered Options

### Option 1: Modular Monolith Architecture

Develop PayLink as a single deployable application divided into independent business modules with clear responsibilities and public interfaces.

Advantages:

- Simpler development and deployment.
- Reduced infrastructure complexity.
- Easier debugging and integration testing.
- Simpler transaction management.
- Compatibility with the existing Docker and CI/CD setup.
- Clear separation of business responsibilities.

Disadvantages:

- Modules cannot initially be deployed independently.
- Scaling individual modules is more difficult.
- Poorly enforced module boundaries may increase coupling.
- A process-level failure may affect the entire application.

### Option 2: Microservices Architecture

Develop PayLink as a collection of independently deployable services that communicate through network-based interfaces or asynchronous messages.

Advantages:

- Independent deployment and scaling.
- Better isolation of service-level failures.
- Greater flexibility for individual services.
- Clear service ownership.

Disadvantages:

- Increased infrastructure and deployment complexity.
- More difficult distributed transaction management.
- Additional network latency and communication failures.
- More complex monitoring, debugging, and testing.
- Higher development and maintenance effort.

## Decision

PayLink will initially adopt a Modular Monolith Architecture.

The application will be divided into the following business modules:

- Accounts and Wallet
- Biller Catalog
- Payment Processing
- Biller Integration
- Fees and Commissions
- Fraud Checks
- Notifications
- Reconciliation

Each module will have a clear responsibility, a public interface, and independent business logic and tests.

## Rationale

The Modular Monolith Architecture provides a suitable balance between simplicity, maintainability, and extensibility.

It reduces infrastructure overhead and supports transactional consistency, which is particularly important for wallet balances, fund reservations, and payment processing.

It also allows the team to reuse the existing ASP.NET Core, SQL Server, Docker, and GitHub Actions configuration.

## Consequences

### Positive Consequences

- Faster initial development.
- Simpler deployment and testing.
- Easier management of database transactions.
- Improved separation of business responsibilities.
- Reduced operational overhead.

### Negative Consequences

- Limited independent module scaling.
- Shared application deployment.
- Potential coupling if module boundaries are not enforced.
- Limited process-level failure isolation.

## Mitigation

- Enforce clear module boundaries.
- Communicate through public interfaces.
- Use dependency injection.
- Maintain independent module tests.
- Avoid direct access to another module's internal data.
- Isolate external service failures using appropriate error handling.

## Future Evolution

During Phase 2, Notifications and Reconciliation will be evolved using a message broker or event bus to support asynchronous processing.

The architecture will preserve clear module boundaries to support future extraction into independent services if necessary.

## Related Documents

- ../design/architecture.md
- ../design/architecture.mmd
- ../design/design-patterns.md
- ../requirements.md