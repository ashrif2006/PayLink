# PayLink Design Patterns

## 1. Introduction

PayLink will use five object-oriented design patterns to improve maintainability, extensibility, testability, and separation of concerns.

The selected patterns are:

1. Adapter Pattern
2. State Pattern
3. Strategy Pattern
4. Chain of Responsibility Pattern
5. Observer Pattern

These patterns are aligned with the project requirements, domain class diagram, payment state diagram, and proposed modular monolith architecture.

## 2. Adapter Pattern

### Category
Structural Design Pattern.

### Module
Biller Integration.

### Problem
PayLink integrates with multiple external billers, including ElectroCo, AquaCo, and TopUpNet. These billers may use different protocols, request formats, and response formats.

Direct integration would increase coupling between payment processing and external biller systems.

### Proposed Solution
Define a common biller interface and implement a separate adapter for each external biller.

### Proposed Components

- IBillerAdapter
- ElectroCoAdapter
- AquaCoAdapter
- TopUpNetAdapter

Each adapter will implement common operations such as:

- Inquire
- Pay
- CheckStatus

The Payment Processing module will communicate with billers through the common interface rather than depending directly on their APIs.

### Benefits

- Reduces coupling with external systems.
- Simplifies the addition of new billers.
- Isolates changes to external APIs.
- Improves testability.

### Testing

- Verify request and response conversion for each adapter.
- Test successful responses, rejected requests, and timeouts.
- Verify that changes to ElectroCo API do not require changes to Payment Processing.

## 3. State Pattern

### Category
Behavioral Design Pattern.

### Module
Payment Processing.

### Problem
A payment passes through multiple states during its lifecycle. Each state has specific rules and permitted transitions.

Managing all state-dependent behavior through conditional statements can increase complexity and introduce invalid transitions.

### Proposed Solution
Encapsulate state-dependent payment behavior in separate state classes using a common state interface.

### Proposed Components

- IPaymentState
- InitiatedState
- FundsHeldState
- SentToBillerState
- CompletedState
- FailedState
- UnknownState
- RejectedState
- RefundedState

The Payment entity will delegate state-dependent behavior to its current state.

The implementation must follow the payment lifecycle defined in the project requirements.

An Unknown payment must not be automatically refunded. A refund is permitted only after the payment is confirmed as failed.

Completed, Rejected, and Refunded are terminal states.

### Benefits

- Prevents invalid payment transitions.
- Encapsulates state-specific behavior.
- Reduces complex conditional logic.
- Improves payment lifecycle maintainability.

### Testing

- Verify all valid state transitions.
- Reject invalid state transitions.
- Verify that a timeout moves the payment to Unknown.
- Verify that Unknown payments are not automatically refunded.
- Verify that terminal states cannot transition to other states.

## 4. Strategy Pattern

### Category
Behavioral Design Pattern.

### Module
Fees and Commissions.

### Problem
Different biller services require different fee calculation rules. These rules may include fixed fees, percentage-based fees, tiered fees, and promotional fee exemptions.

Hardcoding all calculations into a single class would make future changes more difficult.

### Proposed Solution
Define a common fee calculation interface and implement separate strategies for different fee calculation rules.

### Proposed Components

- IFeeStrategy
- FixedFeeStrategy
- PercentageFeeStrategy
- TieredFeeStrategy
- PromotionalFeeStrategy
- FeeCalculator

The FeeCalculator will select the appropriate strategy according to the service type and applicable business rules.

All monetary calculations will use the C# decimal type.

### Benefits

- Encapsulates fee calculation algorithms.
- Supports configurable business rules.
- Simplifies the introduction of new fee policies.
- Improves maintainability and testability.

### Testing

- Verify fixed fee calculations.
- Verify percentage-based fees and minimum fee limits.
- Verify tiered fee calculations.
- Verify that applicable active promotions result in zero service fees.

## 5. Chain of Responsibility Pattern

### Category
Behavioral Design Pattern.

### Module
Fraud Checks.

### Problem
Payment requests must pass multiple fraud prevention checks before funds can be reserved.

Combining all validation rules into a single method would make the fraud prevention logic difficult to maintain and extend.

### Proposed Solution
Implement fraud prevention rules as a chain of independent handlers.

Each handler will validate a specific rule. If the validation succeeds, the request proceeds to the next handler. If the validation fails, the request is rejected and the chain stops.

### Proposed Components

- IFraudCheckHandler
- DailyLimitHandler
- VelocityCheckHandler
- FraudService

Additional handlers can be introduced when new fraud prevention rules are required.

### Benefits

- Separates fraud prevention rules.
- Supports flexible validation chains.
- Simplifies the addition of new fraud checks.
- Improves independent testing.

### Testing

- Verify that valid payments pass all fraud checks.
- Verify that payments exceeding daily limits are rejected.
- Verify that velocity rule violations are rejected.
- Verify that processing stops when a fraud check fails.
- Verify that rejected payments do not reserve wallet funds.

## 6. Observer Pattern

### Category
Behavioral Design Pattern.

### Module
Notifications.

### Problem
Payment status changes may require additional actions, such as sending SMS and push notifications.

Directly coupling payment processing with notification delivery can cause unnecessary dependencies and make payment processing vulnerable to notification provider failures.

### Proposed Solution
Publish payment events when relevant payment state changes occur.

Notification handlers will subscribe to these events and process notifications independently of the payment workflow.

### Proposed Components

- PaymentCompletedEvent
- PaymentFailedEvent
- IPaymentEventHandler
- SmsNotificationHandler
- PushNotificationHandler

Notification delivery failures must not change the payment outcome.

Failed notification deliveries will follow the retry policy defined in the project requirements.

During Phase 2, the event-driven design will be extended using a message broker or event bus to support asynchronous processing and the evolution of Notifications and Reconciliation.

### Benefits

- Reduces coupling between modules.
- Supports multiple event subscribers.
- Isolates notification failures from payment processing.
- Supports future event-driven architecture.

### Testing

- Verify that payment events trigger the appropriate notifications.
- Verify that SMS and push notifications are processed independently.
- Verify that notification provider failures do not affect payment results.
- Verify notification retry behavior.

## 7. Design Patterns Summary

| Pattern | Category | Module | Main Purpose |
|---|---|---|---|
| Adapter | Structural | Biller Integration | Integrate different external biller APIs. |
| State | Behavioral | Payment Processing | Manage payment lifecycle behavior and transitions. |
| Strategy | Behavioral | Fees and Commissions | Support different fee calculation algorithms. |
| Chain of Responsibility | Behavioral | Fraud Checks | Execute independent fraud prevention rules. |
| Observer | Behavioral | Notifications | Decouple payment events from notification delivery. |

## 8. Implementation Guidelines

- Use C# interfaces and dependency injection.
- Keep each pattern within its responsible business module.
- Maintain clear module boundaries.
- Follow SOLID principles.
- Apply Test-Driven Development using xUnit.
- Avoid unnecessary dependencies between modules.
- Update the relevant design diagrams when implementation decisions are finalized.

## 9. Decision Status

Proposed for team review.

The component names and implementation structures in this document are proposed designs. They do not represent completed implementations.

The existing domain class diagram already provides a foundation for several selected patterns. Further implementation and testing will be completed during Phase 2.