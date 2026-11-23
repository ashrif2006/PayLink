# Prompt Log

Record of how AI was used while producing Phase 1, including — and this is the part
that is graded — **where the AI was wrong and we corrected it**.

Rules we follow:

- Minimum **5 entries**, of which **at least 2** must show a rejected or corrected AI answer.
- Verdicts: **Accepted** (used as-is) · **Fixed** (partly right, we corrected it) · **Rejected** (wrong, not used).
- An entry without a *why* is not an entry. The reasoning is the deliverable.

| # | Area | Prompt (short) | AI answer summary | Verdict | Why |
| --- | --- | --- | --- | --- | --- |
| 1 | Design — Payment state machine | "Generate a UML state diagram for the payment lifecycle with Initiated, FundsHeld, SentToBiller, Completed, Failed, Unknown, Rejected, Refunded" | Diagram included `Unknown → Refunded` as an automatic transition and treated `Failed` as final without releasing funds | **Rejected** | See full entry below — both are money-losing mistakes |
| 2 | Requirements — fee rounding | "How should we store and round a 2.5% service fee in C#?" | Suggested `double` for the fee and rounding at display time | **Fixed** | `double` loses precision on money; fee must be `decimal` and rounded at calculation, before it is held |
| 3 | DevOps — Dockerfile | "Give a multi-stage Dockerfile for ASP.NET Core 8" | Correct multi-stage build, but ran as root and copied the whole repo before `restore` | **Fixed** | Ran as root (fixed with `USER app`), and the copy order broke layer caching, so every build re-restored |
| 4 | *(fill in)* | | | | |
| 5 | *(fill in)* | | | | |

---

## Entry 1 — Payment state machine (Rejected)

**Prompt**

> Act as a Senior Software Architect. We are building PayLink, a digital wallet and
> bill payment backend. Generate a UML state diagram for the payment lifecycle with
> these states: Initiated, FundsHeld, SentToBiller, Completed, Failed, Unknown,
> Rejected, Refunded.
>
> Rules: `Unknown` must not be refunded automatically. `Completed` and `Rejected` are
> final. Only allowed transitions. Output Mermaid and explain every transition.

**AI answer summary**

Produced a plausible diagram, but with two defects:

1. `Unknown --> Refunded` on a timer ("if no response after N minutes, refund").
2. `SentToBiller --> Failed` marked as a final state, with no release of the held funds.

**Verdict:** Rejected.

**Why**

1. **`Unknown` must not auto-refund.** "No answer" is not "no payment". The biller may
   have received and settled the money; refunding the customer as well pays twice.
   `Unknown` is resolved only by reconciliation, to `Completed` or to `Refunded` after a
   confirmed failure. This is exactly the failure mode the project exists to test.
2. **`Failed` cannot be final.** A failed payment still holds the customer's money. The
   hold must be released, which is a ledger movement and its own state (`Refunded`)
   before the payment stops. Leaving `Failed` final strands the funds and breaks
   NFR-05 (auditability) and NFR-03 (consistency).

The corrected diagram is `docs/design/payment-state.mmd`.

---

## Entry 2 — Fee rounding and money type (Fixed)

**Prompt**

> In C#, we charge a service fee (fixed amount plus percentage, with min and max) on
> a bill payment. How should we store and round the fee?

**AI answer summary**

Suggested `double fee = amount * 0.025;` and rounding with `Math.Round(fee, 2)` only
when formatting the value for display.

**Verdict:** Fixed.

**Why**

- `double` is binary floating point: `0.1 + 0.2 != 0.3`. On a payment system that
  difference eventually shows up as a ledger that does not balance, which fails
  NFR-05 and makes reconciliation impossible to automate.
- Rounding at display time means the *stored* fee and the *shown* fee differ. The
  customer is charged one number and sees another.
- Correction: fee is `decimal`, rounded **once**, at calculation time, half away from
  zero to 2 decimals (`MidpointRounding.AwayFromZero`), and that rounded value is what
  gets held and captured. `docs/requirements.md` records this as assumption A-4.

---

## Entry 3 — Dockerfile (Fixed)

**Prompt**

> Give me a production-ready multi-stage Dockerfile for an ASP.NET Core 8 Web API.

**AI answer summary**

Two-stage build with `sdk:8.0` and `aspnet:8.0`, `EXPOSE 80`, running as root, and
`COPY . .` before `dotnet restore`.

**Verdict:** Fixed.

**Why**

- Running as root inside the container is unnecessary risk; the .NET 8 images ship an
  `app` user, so the final image now ends with `USER app`.
- `COPY . .` before `dotnet restore` invalidates the restore layer on every source edit,
  making builds slow. Copying only the `.csproj` first, restoring, then copying sources
  keeps restore cached.
- The app listens on port **8080** (`ASPNETCORE_URLS=http://+:8080`) instead of 80,
  which needs no privileged port and matches the compose mapping.

See the repository `Dockerfile`.
