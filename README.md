# PayLink API

Backend service built with **.NET 8 (ASP.NET Core Web API)**, **Entity Framework Core**, and **Microsoft SQL Server**, containerized with **Docker** and tested automatically via **GitHub Actions** and **xUnit**.

---

## Prerequisites for Running the Project

You do **not** need Visual Studio 2022 or the .NET SDK installed just to run and test the API locally. You only need:

* **Git**
* **Docker Desktop** (with Docker Compose enabled)

*(Optional: Install **.NET 8 SDK** and **Visual Studio 2022** or **VS Code** if you plan to write code or run/add xUnit tests locally.)*

---

## Quick Start (Run with Docker)

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/ashrif2006/PayLink.git](https://github.com/ashrif2006/PayLink.git)
   cd PayLink
   ```

2. **Create your local `.env` file from `.env.example`:**
   * **Linux / macOS / Git Bash:**
     ```bash
     cp .env.example .env
     ```
   * **PowerShell:**
     ```powershell
     Copy-Item .env.example .env
     ```
   * Open `.env` and set a strong `DB_PASSWORD` (at least 8 characters, matching SQL Server password rules). Never commit `.env` to Git.

3. **Start SQL Server and the API:**
   ```bash
   docker compose up -d --build
   ```

4. **Verify that everything is running:**
   ```bash
   docker compose ps
   curl http://localhost:8081/api/health
   ```
   * `paylink-sql` should show `healthy`.
   * `paylink-api` should show `Up`.
   * If port `8081` is already in use on your machine, update `8081:8080` in `docker-compose.yml` to another free port.

---

## How to Contribute (Add Files or Edit Code)

Follow this workflow for every change—whether you are adding documentation/files or editing API code:

1. **Start a new branch from the latest `main`:**
   ```bash
   git checkout main
   git pull origin main
   git checkout -b feat/your-task-name
   ```

2. **Add your files or make your code changes:**
   * You can use **VS Code**, **Visual Studio 2022**, or any text editor.
   * If you change C# code, rebuild and restart the containers to test locally:
     ```bash
     docker compose up -d --build
     ```

3. **Commit and push your branch:**
   ```bash
   git add .
   git commit -m "feat: describe your change"
   git push -u origin feat/your-task-name
   ```

4. **Open a Pull Request on GitHub:**
   * Create a PR from your branch into `main`.
   * Wait for **GitHub Actions** (`build-and-test`) to pass.
   * Once approved and green, merge the PR and delete the branch.

5. **Sync your local `main` after merge:**
   ```bash
   git checkout main
   git pull origin main
   git branch -D feat/your-task-name
   ```

---

## Team Rules

* **Never commit directly to `main`.** Always work in a feature/chore branch and merge via Pull Request.
* **Keep `.env` local.** Only commit `.env.example` and non-secret configuration changes.
* **Let CI verify builds and tests.** Every PR runs the .NET build and xUnit test suite automatically in GitHub Actions.
