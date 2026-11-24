# PayLink
## Software Requirements Specification (SRS)
### Phase 1 – Requirements Specification

**Project:** PayLink – Digital Wallet and Bill Payment System  
**Phase:** Phase 1 – Requirements Specification  
**Platform:** Backend System  
**Target Market:** Egypt

---

# 1. Problem Summary

PayLink is a digital wallet and bill-payment backend system operating in Egypt. It enables customers, through a mobile application, and agents, through POS terminals, to inquire about and pay utility bills reliably.

The main engineering challenges include:

- **Financial Accuracy:** Guaranteeing zero double-charges and using exact decimal arithmetic.
- **Ledger-Based Accounting:** Maintaining accurate wallet balances through immutable financial ledger entries.
- **Third-Party Integration:** Handling unreliable third-party biller APIs, including timeouts, asynchronous callbacks, and different communication protocols.
- **Extensibility:** Supporting the onboarding of new billers without modifying the core business logic.

The system aims to provide secure, reliable, and scalable financial transactions while maintaining strict consistency and auditability.

# 2. Stakeholders

The system involves the following stakeholders:

| Stakeholder | Description |
|---|---|
| Customers | End-users who top up their wallets and pay utility bills. |
| Agents | Shop owners who process payments on behalf of customers and earn commissions. |
| Billers | Utility providers such as ElectroCo, AquaCo, TopUpNet, and NetCo that receive payments and provide settlement files. |
| Support Staff | Employees responsible for searching payment histories, processing refunds, and blocking or unblocking accounts. |
| System Administrators | Users responsible for managing billers, fees, limits, promotions, and daily operational reports. |

# 3. Functional Requirements

The following section defines the 25 user stories that describe the expected functionality of the PayLink system.

## 3.1. Accounts and Wallet Management

### US-01 (FR-1) – Customer Registration
**Priority:** Must

As a customer, I want to register an account with basic verification so that I can use PayLink services.

**Acceptance Criteria:**

- **Given:** A new user submits valid registration details.
- **When:** The account is successfully created.
- **Then:**
  - The user's verification level is set to `BASIC`.
  - The daily transaction limit is set to 5,000 EGP.
  - An empty wallet is initialized for the customer.

### US-02 (FR-2) – Upgrade Customer Verification
**Priority:** Should

As an administrator, I want to upgrade a customer's verification level to `VERIFIED` so that the customer can access higher transaction limits.

**Acceptance Criteria:**

- **Given:** A customer currently has a `BASIC` verification level.
- **When:** An administrator approves the verification request.
- **Then:** The customer's daily transaction limit increases to 30,000 EGP.

### US-03 (FR-3) – View Wallet Information
**Priority:** Must

As a customer, I want to view my available balance, held balance, and transaction history.

**Acceptance Criteria:**

- **Given:** An authenticated customer.
- **When:** The customer requests their wallet details.
- **Then:** The system returns:
  - Available balance.
  - Held balance.
  - Chronologically ordered ledger entries.

### US-04 (FR-4) – Wallet Top-Up
**Priority:** Must

As a customer, I want to top up my wallet using a simulated card payment or agent cash deposit.

**Acceptance Criteria:**

- **Given:** A valid top-up amount.
- **When:** The top-up transaction is confirmed.
- **Then:**
  - A credit ledger entry is recorded.
  - The available wallet balance increases by the exact decimal amount.

### US-05 (FR-5) – Ledger-Based Wallet Accounting
**Priority:** Must

As the system, I must calculate all wallet balances strictly from ledger entries.

**Acceptance Criteria:**

- **Given:** Any financial movement occurs.
- **When:** The transaction is processed.
- **Then:**
  - An immutable ledger entry is appended.
  - Wallet balances are derived from the sum of ledger entries.
  - Wallet balances must never be overwritten directly.

## 3.2. Biller Catalog and Inquiry

### US-06 (FR-6) – Biller and Service Management
**Priority:** Must

As an administrator, I want to add, edit, and disable billers and their services.

**Acceptance Criteria:**

- **Given:** An authenticated administrator.
- **When:** The administrator disables a biller.
- **Then:** Customers can no longer initiate new payments through that biller.

### US-07 (FR-7) – View Active Billers
**Priority:** Must

As a customer, I want to view active billers and their required input fields.

**Acceptance Criteria:**

- **Given:** Active billers exist in the system catalog.
- **When:** A customer requests the biller catalog.
- **Then:** Each biller displays:
  - Service type.
  - Minimum transaction amount.
  - Maximum transaction amount.
  - Required input fields, such as a meter number or account number.

### US-08 (FR-8) – Bill Inquiry
**Priority:** Must

As a customer, I want to perform a bill inquiry before making a payment.

**Acceptance Criteria:**

- **Given:** A valid meter number or account number.
- **When:** The customer submits an inquiry request.
- **Then:** PayLink queries the corresponding biller and returns:
  - Bill amount.
  - Calculated service fee.
  - Total payable amount.

## 3.3. Payment Processing and Lifecycle Management

### US-09 (FR-9) – Utility Bill Payment
**Priority:** Must

As a customer, I want to pay a utility bill directly from my wallet.

**Acceptance Criteria:**

- **Given:** The customer has sufficient available balance and passes all fraud checks.
- **When:** The customer confirms the payment.
- **Then:**
  - The required funds are held.
  - The corresponding biller is contacted.
  - Upon successful confirmation, the held funds are converted into a final charge.

### US-10 (FR-10) – Idempotent Payment Requests
**Priority:** Must

As a mobile client, I want repeated requests using the same idempotency key to return the original result.

**Acceptance Criteria:**

- **Given:** A payment request contains an already-used `Idempotency-Key`.
- **When:** The same request is resent due to a network retry.
- **Then:**
  - No duplicate payment is created.
  - No additional charge occurs.
  - The original response is returned.

### US-11 (FR-11) – Strict Payment State Machine
**Priority:** Must

As the system, I must enforce a strict payment state machine.

**Acceptance Criteria:**

- **Given:** A payment exists in a particular state.
- **When:** An invalid state transition is attempted, such as changing `Completed` to `Failed`.
- **Then:** The transition is rejected.

### US-12 (FR-12) – Funds Holding and Release
**Priority:** Must

As the system, I must hold customer funds before calling the biller and release them if the payment fails.

**Acceptance Criteria:**

- **Given:** A payment is in the `Initiated` state.
- **When:** Fraud checks are successfully passed.
- **Then:**
  - Funds are moved from `available_balance` to `held_balance`.
  - The payment transitions to `FundsHeld`.
  - If the biller refuses the payment, the payment transitions to `Failed`.
  - The held funds are released back to `available_balance` through a compensating ledger entry, and the release is recorded as `Refunded`.

### US-13 (FR-13) – Safe Handling of Biller Timeouts
**Priority:** Must

As the system, I must handle biller timeouts safely without automatically refunding potentially successful payments.

**Acceptance Criteria:**

- **Given:** A live biller does not respond within 10 seconds.
- **When:** The timeout is triggered.
- **Then:**
  - The payment status changes to `Unknown`.
  - The funds remain held.
  - The system schedules up to three status re-queries:
    - First retry: After 1 minute.
    - Second retry: After 5 minutes.
    - Third retry: After 15 minutes.

### US-14 (FR-14) – Asynchronous Biller Callbacks
**Priority:** Must

As the system, I must accept asynchronous callbacks from TopUpNet and ignore duplicate callbacks.

**Acceptance Criteria:**

- **Given:** A TopUpNet payment is in the `SentToBiller` or `Unknown` state.
- **When:** A success callback is received twice.
- **Then:**
  - The first callback successfully completes the payment.
  - The second callback is ignored.
  - The duplicate callback is logged.

### US-15 (FR-15) – Multiple Biller Integrations
**Priority:** Must

As the system, I must integrate with at least three billers using different communication protocols.

**Supported Integrations:**

| Biller | Integration Protocol |
|---|---|
| ElectroCo | REST / JSON |
| AquaCo | SOAP / XML |
| TopUpNet | Asynchronous REST |

**Acceptance Criteria:**

- **Given:** A payment request is submitted for any supported biller.
- **When:** The payment is dispatched.
- **Then:**
  - The corresponding adapter translates the request and response.
  - Protocol-specific details remain isolated within the Biller Integration module.
  - Core business modules do not depend on individual biller protocols.

## 3.4. Fees, Commissions, and Fraud Prevention

### US-16 (FR-16) – Configurable Service Fees
**Priority:** Must

As the system, I must calculate service fees according to configurable business rules and active promotions.

**Acceptance Criteria:**

- **Given:** A customer initiates a bill payment.
- **When:** The service fee is calculated.
- **Then:** The system applies the following fee rules:

| Service Type | Fee Calculation |
|---|---|
| Electricity | 2.50 EGP |
| Water | 2.50 EGP |
| Mobile Top-Up | 1.5% of the bill amount, with a minimum fee of 1 EGP |
| Internet | Tiered fees of 2 EGP or 4 EGP |
| Active Promotion | 0 EGP |

If an applicable active promotion exists, the service fee is set to 0 EGP.

### US-17 (FR-17) – Agent Commission
**Priority:** Should

As an agent, I want to pay bills on behalf of customers and earn a commission.

**Acceptance Criteria:**

- **Given:** An agent successfully processes a customer payment.
- **When:** The payment is confirmed as completed.
- **Then:** 40% of the service fee is credited to the agent's wallet.

### US-18 (FR-18) – Transaction Limits Enforcement
**Priority:** Must

As the system, I must reject payments that violate individual transaction limits or daily transaction limits.

**Acceptance Criteria:**

- **Given:** A customer submits a payment request.
- **When:** The payment violates any of the following limits:
  - Minimum payment amount: 5 EGP.
  - Maximum payment amount: 10,000 EGP.
  - Daily limit for `BASIC` customers: 5,000 EGP.
  - Daily limit for `VERIFIED` customers: 30,000 EGP.
- **Then:**
  - The payment transitions to `Rejected`.
  - A clear error message explaining the rejection reason is returned.

### US-19 (FR-19) – Velocity-Based Fraud Detection
**Priority:** Should

As the system, I must enforce a velocity-based fraud prevention rule.

**Acceptance Criteria:**

- **Given:** An account attempts more than five payments within 60 seconds.
- **When:** The sixth payment attempt arrives.
- **Then:**
  - The account is blocked for 10 minutes.
  - The sixth payment is rejected.

## 3.5. Support, Notifications, Audit, and Reconciliation

### US-20 (FR-20) – Payment History Search
**Priority:** Must

As a support employee, I want to search for a payment using its reference number and view its complete history.

**Acceptance Criteria:**

- **Given:** A valid payment reference number.
- **When:** A support employee searches for the payment.
- **Then:** The system displays:
  - All payment state transitions.
  - Corresponding timestamps.
  - Biller responses.
  - Complete payment history.

### US-21 (FR-21) – Customer Account Management
**Priority:** Should

As a support employee, I want to block and unblock customer accounts.

**Acceptance Criteria:**

- **Given:** A suspicious customer account.
- **When:** A support employee blocks the account.
- **Then:** All subsequent payment attempts from that account are rejected.

### US-22 (FR-22) – Payment Notifications
**Priority:** Must

As a customer, I want to receive SMS and push notifications when a payment is completed or failed.

**Acceptance Criteria:**

- **Given:** A payment reaches the `Completed` or `Failed` state.
- **When:** The payment state transition occurs.
- **Then:**
  - SMS notifications are dispatched asynchronously.
  - Push notifications are dispatched asynchronously.
  - Notification processing does not block the payment workflow.

### US-23 (FR-23) – Notification Retry Mechanism
**Priority:** Should

As the system, I must retry failed notification deliveries up to three times.

**Acceptance Criteria:**

- **Given:** An SMS provider is unavailable.
- **When:** A notification delivery attempt fails.
- **Then:**
  - The system retries delivery up to three times.
  - If all attempts fail, the notification status is marked as `UNDELIVERED`.

### US-24 (FR-25) – Audit Logging
**Priority:** Must

As an auditor, I want every financial movement and administrative action to be recorded in an immutable audit log.

**Acceptance Criteria:**

- **Given:** A ledger entry or administrative update is executed.
- **When:** The operation is performed.
- **Then:** An immutable audit log entry is created containing:
  - `who`: The actor responsible for the operation.
  - `when`: The operation timestamp.
  - `action`: The action performed.
  - `reason`: The reason for the operation.

### US-25 (FR-26) – Daily Reconciliation Reports
**Priority:** Should

As an operations user, I want to generate daily reconciliation reports comparing PayLink records with biller settlement files.

**Acceptance Criteria:**

- **Given:** A biller settlement file is available.
- **When:** The reconciliation process is executed.
- **Then:**
  - PayLink transaction records are compared against the biller's settlement records.
  - Any mismatch is flagged in the reconciliation report.
  - Mismatches, such as a biller reporting `Paid` while PayLink records the payment as `Failed`, are flagged for support review.

---

# 4. Non-Functional Requirements

The following non-functional requirements define the expected quality attributes and measurable performance criteria of the PayLink system.

## 4.1. Correctness and Financial Integrity

**Requirement:** The system must guarantee financial consistency under concurrent payment requests and duplicate submissions.

**Acceptance Criteria:**
- Zero double-charges.
- Zero negative wallet balances.
- Correct handling of concurrent payment requests.
- Accurate handling of duplicate requests through idempotency mechanisms.

## 4.2. Performance and Scalability

**Requirement:** The system must support high transaction throughput.

**Acceptance Criteria:**
- Sustain at least 100 payments per second.
- Maintain this throughput for at least one minute.
- Performance testing must be conducted against mock biller services.

## 4.3. Resilience and Fault Isolation

**Requirement:** Failure of one biller must not affect transactions involving other billers.

**Acceptance Criteria:**
- An outage of one biller, such as AquaCo, must result in 0% impact on payments to other operational billers.
- Biller failures must remain isolated within their respective integration boundaries.

## 4.4. Modifiability and Extensibility

**Requirement:** The system must allow new billers and protocol versions to be integrated without modifying core business logic.

**Acceptance Criteria:**
- Adding a new biller must require changes only to the Biller Integration module and configuration.
- Integrating ElectroCo v2 must not require changes to core business modules.
- Existing integrations must continue working independently.

## 4.5. Auditability

**Requirement:** Every financial movement must have a corresponding audit record.

**Acceptance Criteria:**
- 100% of financial movements must be audited.
- Each audit record must contain the responsible actor, timestamp, and reason.
- Audit records must be immutable.

## 4.6. Testability and Code Coverage

**Requirement:** Domain logic must be sufficiently covered by automated tests.

**Acceptance Criteria:**
- At least 70% line coverage for domain logic.
- Coverage requirements must be enforced in the Continuous Integration (CI) pipeline.

---

# 5. Ambiguities and Design Decisions

This section documents the identified ambiguities in the requirements and the decisions made to ensure consistent implementation.

## 5.1. Ambiguity 1 – Daily Transaction Limit Calculation

**Question:** Does the daily transaction limit (5,000 EGP for BASIC customers and 30,000 EGP for VERIFIED customers) include the service fee or only the bill amount?

**Decision:** The daily transaction limit applies only to the bill amount (principal), excluding the service fee.

**Rationale:** This prevents changes in service fees from unexpectedly blocking customers who are close to their daily transaction limits.

## 5.2. Ambiguity 2 – Concurrent Payment Requests

**Question:** What happens when two concurrent payment requests arrive from the same wallet, while the wallet contains enough balance for only one transaction?

**Decision:** PostgreSQL row-level locking using `SELECT ... FOR UPDATE` will be implemented during the `FundsHeld` operation.

**Rationale:** Locking the wallet row ensures that only one transaction can modify the wallet's financial state at a time, preventing race conditions, double spending, and negative balances.

## 5.3. Ambiguity 3 – Monetary Data Types

**Question:** Should monetary values be stored using floating-point numbers or decimal types?

**Decision:** All monetary values must use exact decimal arithmetic.

**Implementation:**

- **Database:** PostgreSQL `NUMERIC(12, 2)`.
- **Application:** Python `Decimal`.
- Floating-point types such as `float` must never be used for financial calculations.

**Rationale:** Decimal arithmetic avoids floating-point precision errors and ensures accurate financial calculations.

---

# 6. Summary

PayLink is designed as a reliable and extensible digital wallet and bill-payment backend system.

Phase 1 establishes the functional and non-functional requirements that guide the system's architecture and implementation.

The main engineering priorities are:

1. Financial accuracy and ledger-based accounting.
2. Secure and idempotent payment processing.
3. Strict payment lifecycle and state management.
4. Reliable integration with multiple billers using different protocols.
5. Fraud prevention and transaction limit enforcement.
6. Asynchronous notifications and callback handling.
7. Immutable audit logging and daily reconciliation.
8. High performance, resilience, and extensibility.

These requirements provide the foundation for the subsequent software design and implementation phases.
