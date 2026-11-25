# ADR-003: Data Ownership

## Status

Proposed for team review.

## Context

PayLink consists of multiple business modules that manage different types of business data.

The system requires strong consistency for financial transactions, wallet balances, payment states, and ledger entries.

The data ownership approach must prevent unauthorized modifications, reduce coupling between modules, and support future architectural evolution.

Microsoft SQL Server and Entity Framework Core are used as the database and persistence technologies.

## Considered Options

### Option 1: Shared Database with Unrestricted Access

All modules use the same database and can directly access and modify any table.

Advantages:

- Simple initial implementation.
- Easy access to related data.
- Simplified database queries and transactions.

Disadvantages:

- High coupling between modules.
- Unclear data ownership.
- Increased risk of inconsistent data.
- Difficult maintenance and future service extraction.

### Option 2: Shared Database with Logical Module Ownership

All modules use a single physical database, but each module owns specific tables and controls access to its business data.

Advantages:

- Clear data ownership.
- Reduced coupling between modules.
- Simpler transaction management.
- Compatibility with the modular monolith architecture.
- Lower infrastructure complexity.
- Supports future service extraction.

Disadvantages:

- Requires strict enforcement of module boundaries.
- Modules share the same physical database.
- Database-level failures may affect multiple modules.

### Option 3: Separate Database per Module

Each module owns and manages an independent database.

Advantages:

- Strong data isolation.
- Independent database management.
- Supports independent service deployment and scaling.

Disadvantages:

- Increased infrastructure complexity.
- Difficult cross-module transactions.
- Additional data synchronization requirements.
- Higher development and maintenance effort.

## Decision

PayLink will initially use a shared Microsoft SQL Server database with logical data ownership for each business module.

Each module will be responsible for managing its own business data.

Other modules must use the owning module's public interfaces or published events rather than directly modifying its tables.

## Data Ownership

The following ownership allocation is proposed based on the existing ER diagram.

| Module | Owned Data |
|---|---|
| Accounts and Wallet | USERS, WALLETS, LEDGER_ENTRIES |
| Biller Catalog | BILLERS, BILLER_SERVICES |
| Payment Processing | PAYMENTS, PAYMENT_EVENTS |
| Biller Integration | External biller communication and response mapping |
| Fees and Commissions | FEE_RULES, PROMOTIONS, COMMISSION_ENTRIES |
| Fraud Checks | Fraud validation rules and decisions |
| Notifications | NOTIFICATIONS |
| Reconciliation | RECONCILIATION_REPORTS |

Biller Integration and Fraud Checks do not currently have dedicated tables in the ER diagram. Additional persistent data may be introduced if required during implementation.

AUDIT_LOGS will be managed through a shared auditing infrastructure component with controlled write access.

## Data Access Rules

- Each module controls modifications to its owned data.
- Cross-module operations must use public interfaces.
- Cross-module references may use entity identifiers and foreign keys.
- Other modules must not directly modify wallet balances or payment states.
- Database access must use Entity Framework Core.
- Sensitive configuration must not be committed to version control.
- Monetary values must use the C# decimal type and appropriate SQL Server decimal precision.

## Transaction Management

Critical financial operations must maintain transactional consistency.

The Accounts and Wallet module is responsible for:

- Checking available wallet balances.
- Reserving funds.
- Releasing reserved funds.
- Finalizing wallet deductions.
- Recording ledger entries.

The Payment Processing module is responsible for:

- Creating payment records.
- Managing payment state transitions.
- Enforcing payment idempotency.
- Recording payment lifecycle events.

Related wallet and payment updates must be coordinated within appropriate local database transactions.

Concurrency controls must prevent simultaneous payment requests from spending the same available wallet funds.

Database transactions must not remain open while waiting for external biller responses.

Unknown payments must retain their reserved funds until the payment outcome is confirmed.

Refund operations must prevent duplicate fund releases.

## Data Consistency

- Use database constraints to protect data integrity.
- Enforce unique idempotency keys according to the payment requirements.
- Use appropriate transaction boundaries.
- Apply SQL Server-compatible concurrency controls.
- Maintain traceable financial ledger entries.
- Ensure asynchronous event consumers are idempotent.
- Publish events only for committed business state changes.

## Rationale

Shared database infrastructure with logical module ownership provides a balance between consistency, maintainability, and implementation simplicity.

It supports reliable financial transactions without introducing the complexity of distributed databases.

Clear data ownership also reduces coupling and prepares the system for future architectural changes.

## Consequences

### Positive Consequences

- Clear responsibility for business data.
- Improved financial consistency.
- Simpler database management.
- Reduced infrastructure overhead.
- Improved maintainability.
- Compatibility with the existing technology stack.

### Negative Consequences

- Shared physical database dependency.
- Limited independent database scaling.
- Module boundaries require active enforcement.
- Future service extraction may require data migration.

## Mitigation

- Define clear module interfaces.
- Restrict direct cross-module data modifications.
- Use appropriate database transactions and constraints.
- Test concurrent wallet operations.
- Test duplicate payment and refund requests.
- Review database access during code reviews.

## Future Evolution

Notifications and Reconciliation may evolve into independently operating components during Phase 2.

Their data ownership boundaries will be preserved to support asynchronous processing and potential future database separation.

## Related Documents

- 001-architecture-style.md
- 002-module-communication.md
- ../design/architecture.md
- ../design/er-diagram.mmd
- ../design/domain-class.mmd
- ../requirements.md