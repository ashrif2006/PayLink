# ADR-002: Module Communication

## Status

Proposed for team review.

## Context

PayLink consists of multiple business modules that need to communicate to complete payment operations.

Some operations require immediate responses, such as wallet balance validation, fraud checks, and fee calculation.

Other operations, such as notification delivery and reconciliation, can be processed asynchronously.

The communication approach must maintain payment consistency, reduce coupling between modules, and support future architectural evolution.

## Considered Options

### Option 1: Synchronous Communication Only

Modules communicate directly through public interfaces and method calls.

Advantages:

- Simple implementation.
- Immediate responses.
- Easier debugging and testing.
- Low communication overhead within a modular monolith.

Disadvantages:

- Increased runtime dependencies between modules.
- Slow operations may delay payment processing.
- Failures may propagate between modules.
- Limited flexibility for asynchronous operations.

### Option 2: Asynchronous Event-Driven Communication Only

Modules communicate by publishing and consuming events through an event bus or message broker.

Advantages:

- Reduced coupling between modules.
- Improved failure isolation.
- Support for asynchronous processing.
- Easier addition of new event subscribers.

Disadvantages:

- Increased implementation complexity.
- Eventual consistency.
- More difficult debugging and testing.
- Additional requirements for event delivery, retries, and duplicate handling.
- Less suitable for operations requiring immediate validation.

### Option 3: Hybrid Communication

Combine synchronous communication through public interfaces with asynchronous event-driven communication.

Advantages:

- Immediate responses for critical payment operations.
- Reduced coupling for background operations.
- Better isolation of notification failures.
- Supports future architectural evolution.
- Balances consistency and flexibility.

Disadvantages:

- Requires maintaining two communication mechanisms.
- Requires clear communication rules.
- Asynchronous operations introduce additional testing requirements.

## Decision

PayLink will adopt a Hybrid Communication approach.

Synchronous communication will be used for operations that require immediate results.

Asynchronous event-driven communication will be used for operations that do not need to block the payment workflow.

## Synchronous Communication

The Payment Processing module will communicate through public interfaces with:

- Accounts and Wallet
- Biller Catalog
- Fees and Commissions
- Fraud Checks
- Biller Integration

These interactions will use C# interfaces and dependency injection.

## Asynchronous Communication

Payment Processing will publish relevant payment events.

Proposed events include:

- PaymentCompletedEvent
- PaymentFailedEvent

Notifications will subscribe to relevant payment events and process notification delivery independently.

Reconciliation will consume relevant events and perform scheduled checks to resolve payment discrepancies.

During Phase 2, Notifications and Reconciliation will be evolved using a message broker or event bus.

## Reliability and Consistency

- Critical wallet operations must use appropriate database transactions and concurrency controls.
- Payment requests must support idempotency.
- Notification failures must not change payment outcomes.
- Event consumers must handle duplicate events safely.
- Failed asynchronous operations must support appropriate retry mechanisms.
- Payment events must represent committed payment state changes.
- Unknown payments must not be automatically refunded.
- A reliable event publication mechanism, such as a transactional outbox, should be evaluated during implementation.

## Rationale

The hybrid approach provides a balance between transactional consistency and loose coupling.

Synchronous communication supports operations that require immediate decisions, while asynchronous communication prevents background operations from unnecessarily delaying payment processing.

This approach is compatible with the proposed modular monolith architecture and supports the required Phase 2 evolution.

## Consequences

### Positive Consequences

- Clear communication responsibilities.
- Reduced coupling between modules.
- Improved payment workflow responsiveness.
- Better isolation of background operation failures.
- Support for future service extraction.

### Negative Consequences

- Additional event management complexity.
- More complex integration testing.
- Possible delays in asynchronous processing.
- Additional requirements for retry and duplicate handling.

## Mitigation

- Define clear public module interfaces.
- Use consistent event contracts.
- Implement idempotent event handlers.
- Apply appropriate retry policies.
- Monitor and log asynchronous failures.
- Test synchronous and asynchronous interactions using xUnit.

## Related Documents

- 001-architecture-style.md
- ../design/architecture.md
- ../design/architecture.mmd
- ../design/design-patterns.md
- ../requirements.md