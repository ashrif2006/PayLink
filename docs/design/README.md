# Design diagrams

Six diagrams, all as Mermaid **source** (`.mmd`). The source is the artifact we
review and change; the rendered picture is an export.

| File | Diagram |
| --- | --- |
| `use-case.mmd` | Use Case — actors and their goals |
| `domain-class.mmd` | Domain Class — entities, relationships, enums |
| `pay-bill-sequence.mmd` | Sequence — pay a bill, including the `Unknown` path |
| `refund-sequence.mmd` | Sequence — refund after failure, manual and automatic |
| `payment-state.mmd` | Payment State — the allowed lifecycle transitions |
| `er-diagram.mmd` | ER — tables, keys, and column types |

## How to render

**Quickest:** open <https://mermaid.live>, paste one file's contents, export PNG/SVG.

**From the command line:**

```bash
npx -y @mermaid-js/mermaid-cli -i docs/design/payment-state.mmd -o payment-state.svg
```

**In VS Code:** install the *Markdown Preview Mermaid Support* or *Mermaid Editor*
extension and open the `.mmd` file directly.

## Consistency rules

The diagrams are graded on agreeing with each other, so check these before submitting:

- Every state in `payment-state.mmd` appears as a payment status in `er-diagram.mmd`.
- `HeldBalance` exists in the domain class, the ER (`held_balance`), and the pay sequence (the `Hold` call).
- Every arrow in the pay sequence that touches money has a matching ledger entry type.
- `Unknown` has no automatic edge to `Refunded` in any diagram.
