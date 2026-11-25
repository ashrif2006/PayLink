
Verdicts: **Accepted** (used as-is) · **Fixed** (partly right, we corrected it) · **Rejected** (wrong, not used).

| # | Area | Prompt (short) | AI answer summary | Verdict | Why |
|---|---|---|---|---|---|



## Entry 1 — Unit test for HealthController
>Act as senior .NET Developer. I have ASP.NET Core controller 'HealthController'
> with an action `GetHealth()` that returns `Ok(new { status = 200, service = "PayLink APP", timestamp = DateTime.UtcNow })`.
>write an XUnit test
>
**Verdict:** Fixed.

**Why**

1. `WebApplicationFactory` starts the real pipeline and needs the
   `Microsoft.AspNetCore.Mvc.Testing` package. That is an integration test, not a
   unit test, and it makes CI slower for no benefit here — the controller has no
   dependencies, so it can be constructed directly.
2. The assertion was simply wrong: the payload returns the **integer** `200`, not the
   string `"ok"`. The test failed for the right reason — it caught our own mismatch.

Correction kept in `tests/PayLink.Api.Tests/UnitTest1.cs`: call `controller.GetHealth()`,
assert `OkObjectResult` with `StatusCode == 200`, parse the anonymous object with
`JsonDocument`, and assert `status == 200`, `service == "PayLink APP"`, and that the
timestamp is inside the test window.

## Entry 2 — Dockerfile (Fixed)

**Prompt**

> Give me a production-ready Dockerfile for an ASP.NET Core 8 Web API. The project
> file is at `src/PayLink.Api/PayLink.Api.csproj`.
>
> **AI answer summary**

A single-stage Dockerfile based on `mcr.microsoft.com/dotnet/sdk:8.0`, running as
root, `COPY . .` followed by `dotnet restore`, `EXPOSE 80`, and `ENTRYPOINT dotnet run`.

**Verdict:** Fixed.

**Why**
1. **Single stage.** The SDK image is ~800MB and contains compilers we do not want in
   production. Fixed with two stages: `sdk:8.0` builds and publishes, `aspnet:8.0` runs.
2. **Port 80.** Our compose maps `8081:8080` and the app listens on 8080 through
   `ASPNETCORE_HTTP_PORTS`. Port 80 has no listener, so the container looked "up" but
   answered nothing. Fixed to `EXPOSE 8080`.
3. **Copy order.** `COPY . .` before `dotnet restore` invalidates the restore layer on
   every source change, so every build restored NuGet again. Fixed by copying only the
   `.csproj`, restoring, then copying the sources.
4. `dotnet run` in the entrypoint rebuilds at container start. Replaced with
   `dotnet publish /p:UseAppHost=false` and `ENTRYPOINT ["dotnet", "PayLink.Api.dll"]`.

   




