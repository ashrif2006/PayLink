# PayLink — Requirements (Phase 1)

> Owner: Person 1 (Requirements & Product Lead) · Reviewers: whole team
> Status: **draft — 25 stories listed, acceptance criteria being finalised**

## 1. Problem summary

A customer holds money in a digital wallet and pays household bills (electricity,
water, mobile, internet) through one app. The payment path itself is familiar; what
makes this system hard is the money that is "in flight":

- the wallet must reserve funds **before** the biller is contacted,
- a biller that received the money but replied late must **not** be treated as a failure,
- a customer who taps **Pay** twice must be charged **once**,
- a biller callback that arrives twice must change nothing the second time.

Example used throughout the documents: a wallet holding **1000 EGP** pays a
**350 EGP** electricity bill with a **2.50 EGP** service fee.

| Step | Amount |
| --- | --- |
| Electricity bill | 350.00 EGP |
| Service fee | 2.50 EGP |
| **Total** | **352.50 EGP** |
| Remaining available | **647.50 EGP** |

## 2. Stakeholders

| Stakeholder | Interest |
| --- | --- |
| **Customer** | Pay bills safely, see a correct balance, never be charged twice |
| **Agent** (kiosk / shop) | Collect cash, deposit into a customer wallet, pay on their behalf |
| **Biller** (electricity, water, mobile) | Receive exactly one settlement per bill reference |
| **Admin** | Manage billers and fees, see all payments |
| **Support employee** | Answer "where is my money?" and issue refunds |
| **Finance / reconciliation** | Prove every movement matches the biller's settlement file |
| **Compliance / fraud** | Limits, blocking, audit trail |

## 3. Scope

**In scope (Phase 1 documents, Phase 2 builds):** wallet and balances, biller
catalog, bill payment with holds, refunds, fees, fraud limits, notifications,
admin and support views, reconciliation.

**Out of scope:** card issuing, credit, lending, multi-currency conversion,
merchant acquiring, native mobile clients.

## 4. Payment flow (happy path)

1. Customer is authenticated and allowed to pay.
2. System fetches the bill amount from the biller.
3. Service fee is calculated.
4. Fraud checks run; the balance must cover amount + fee.
5. Funds are **held** in the wallet.
6. Payment request is sent to the biller.
7. Biller confirms → the hold is **captured** (money leaves for good).
8. Biller fails → the hold is **released** back to the customer.
9. Customer is notified.

## 5. User stories

Priority: **M**ust / **S**hould / **C**ould. Every story needs its own acceptance
criteria in Given/When/Then form before it is considered done (three examples follow).

### Accounts & Wallet — Person 1

| ID | Story | Priority |
| --- | --- | --- |
| US-01 | As a customer, I want to view my available and held balance so that I know what I can spend right now. | M |
| US-02 | As a customer, I want to top up my wallet so that I can pay bills. | M |
| US-03 | As a customer, I want to see my transaction history with dates and amounts so that I can check my spending. | M |
| US-04 | As a customer, I want to sign in securely so that nobody else can use my wallet. | M |
| US-05 | As a customer, I want to see which payments are holding my money and why. | S |
| US-06 | As a customer, I want to complete identity verification so that my wallet has higher limits. | S |
| US-07 | As a customer, I want a monthly statement I can export so that I can submit it to my employer. | C |

### Billers & Payments — Person 2

| ID | Story | Priority |
| --- | --- | --- |
| US-08 | As a customer, I want to browse billers by category so that I can find the right one. | M |
| US-09 | As a customer, I want to fetch a bill by its reference so that I pay the correct amount. | M |
| US-10 | As a customer, I want to pay a bill from my wallet in one action so that it is settled. | M |
| US-11 | As a customer, I want a receipt with a payment reference so that I have proof of payment. | M |
| US-12 | As a customer, I want to check the status of a pending payment so that I know whether my money moved. | M |
| US-13 | As a customer, I want a second tap on **Pay** to be ignored so that I am never charged twice. | M |

### Fees, Fraud & Notifications — Person 3

| ID | Story | Priority |
| --- | --- | --- |
| US-14 | As a customer, I want to see the service fee before I confirm so that the total is not a surprise. | M |
| US-15 | As an admin, I want fee rules per biller (fixed or percentage) so that fees are consistent. | M |
| US-16 | As an admin, I want daily payment limits per customer so that fraud losses are bounded. | M |
| US-17 | As a fraud analyst, I want suspicious patterns blocked automatically so that abuse stops early. | S |
| US-18 | As a customer, I want a confirmation message when a payment succeeds. | M |
| US-19 | As a customer, I want a message explaining a failure and that my money came back. | M |

### Admin, Support & Reconciliation — Person 4

| ID | Story | Priority |
| --- | --- | --- |
| US-20 | As an admin, I want to filter all payments by status, biller and date so that I can investigate. | M |
| US-21 | As a support employee, I want to search a payment by reference or phone so that I can answer quickly. | M |
| US-22 | As a support employee, I want to issue a controlled refund with a reason so that an unhappy customer is made whole. | S |
| US-23 | As a finance user, I want to import the biller's settlement file so that I can compare totals. | S |
| US-24 | As a finance user, I want mismatches flagged so that I know which payments to chase. | S |
| US-25 | As an admin, I want to add or disable a biller so that the catalog stays accurate. | C |

## 6. Acceptance criteria

### US-01 — View wallet balance

- **Given** I am an authenticated customer,
- **When** I request my wallet,
- **Then** the system returns `availableBalance` and `heldBalance` separately,
- **And** their sum equals my total balance,
- **And** no other customer's wallet data is included.

### US-10 — Pay a bill

- **Given** my available balance is 1000 EGP and the bill is 350 EGP with a 2.50 EGP fee,
- **When** I confirm the payment,
- **Then** 352.50 EGP is held, then captured on the biller's confirmation,
- **And** my available balance ends at 647.50 EGP,
- **And** exactly one payment and the matching ledger entries exist.

### US-13 — Double tap on Pay

- **Given** I submitted a payment with idempotency key `K`,
- **When** the same request with key `K` arrives again while the first is in flight,
- **Then** the system returns the **same** payment, unchanged,
- **And** no second hold is taken,
- **And** the biller receives at most one submission.

> Remaining 22 stories: acceptance criteria to be written by their owners before the
> Phase 1 submission. Use the same Given/When/Then shape.

## 7. Non-functional requirements

Each one is measurable; "fast and secure" is not a requirement.

| ID | Area | Requirement | How we measure |
| --- | --- | --- | --- |
| NFR-01 | Performance | 100 payments/second sustained for one minute | Load test against `/payments` with a mocked biller; report p95 latency |
| NFR-02 | Reliability | Zero double charges | Replay every payment request twice in tests; assert a single ledger debit |
| NFR-03 | Consistency | No negative available balance, ever | Concurrency test: 2 payments of 400 on a 500 balance — exactly one succeeds |
| NFR-04 | Testability | ≥ 70% coverage of domain logic | `dotnet test --collect:"XPlat Code Coverage"` on `PayLink.Api.Domain` |
| NFR-05 | Auditability | 100% of money movements recorded | Assert one ledger entry per hold/capture/release; reconciliation totals must match |
| NFR-06 | Availability | One biller's outage does not stop payments to other billers | Fail one biller's mock; pay another biller successfully |

## 8. Assumptions and AI ambiguities

Decisions taken where the brief was silent. Each is a candidate prompt-log entry.

| # | Ambiguity | Decision | Why |
| --- | --- | --- | --- |
| A-1 | Fee: fixed or percentage? | Per-biller policy, fixed + optional percentage, with min/max | Real billers differ; hardcoding one rule would need a rewrite |
| A-2 | What happens to `Unknown`? | Funds stay held; resolved by reconciliation; never auto-refunded | The biller may already have the money |
| A-3 | Who pays the fee when payment fails? | Nobody — hold and fee are released together | Charging for a failed payment is not defensible |
| A-4 | Rounding of fees | Round half away from zero to 2 decimals, at fee calculation time | Avoids 0.005 EGP mismatches in the ER ledger sum |
| A-5 | Same bill paid twice deliberately? | Allowed once the first payment reaches a final state; new idempotency key required | A duplicate bill reference may be legitimate (two months) |
| A-6 | Refund timing | Immediate for `Failed`; manual approval for `Completed` | Reversing a settled payment needs a human decision |

## 9. Open questions for the team

- [ ] PostgreSQL confirmed as the database (affects ER diagram types)?
- [ ] Which biller integration style in Phase 2 — real HTTP sandbox or local mock?
- [ ] Do we need an Agent role in Phase 1 stories, or only Customer and Admin?
- [ ] Money as `decimal` everywhere (yes) — who reviews for accidental `double`?
