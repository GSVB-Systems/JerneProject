# JerneProject

Modern web app for managing users, boards, transactions, and winning boards. The stack is a Vite/React/TypeScript client and an ASP.NET Core Web API backed by PostgreSQL. Docker Compose and Fly.io configs are included for local and hosted deployments.

## Architecture
- Client (`client/`): Vite + React 19 + TypeScript, Tailwind/DaisyUI for styling, React Router for navigation, Jotai for token storage, fetch-based generated API client (`src/models/ServerAPI.ts`).
- API (`server/api/`): ASP.NET Core 10, JWT auth, role-based authorization, Swagger/OpenAPI with automatic TS client generation on startup.
- Shared contracts (`server/Contracts/`), services (`server/service/`), data access (`server/dataaccess/`), tests (`server/test/`).
- Infrastructure: PostgreSQL (local via Compose), containerized client/server (`client/Dockerfile`, `server/Dockerfile`), Fly.io configs in `client/fly.toml` and `server/fly.toml`.

## Quick start (local)
Prereqs: Node 22+, npm, .NET 10 SDK (see `server/global.json`), Docker (for Compose).

```bash
# 1) Backend (API)
cd server
cp api/.env.template api/.env   # fill in secrets/DB creds (see Env vars below)
dotnet restore
cd api
dotnet run                       # serves on http://localhost:8080

# 2) Frontend (client)
cd ../client
npm install
npm run dev                      # Vite dev server, default http://localhost:5173

# 3) Full stack via Docker Compose (runs client, server, Postgres)
cd ..
docker compose up --build
```

## Environment & configuration
- API (`server/api`)
  - `.env` (from `.env.template`): `JWT_SECRET` (base64 HMAC key), `JWT_ISSUER`, `JWT_AUDIENCE`, `POSTGRES_*` (host/db/user/password/port). `compose.yml` expects `.env` at repo root for the server container.
  - `appsettings.json` / `appsettings.Development.json`: logging levels, hosts.
  - CORS is wide open in `Program.cs` (AllowAnyOrigin/Method/Header) — tighten for production.
- Client (`client`)
  - At runtime nginx template reads `BACKEND_URL` env var (see `client/Dockerfile`); in Compose it defaults to `http://server:8080`.
  - Vite dev proxies are not configured; set `VITE_BACKEND_URL` or update fetch baseUrl in `src/api-clients.ts` if needed.
- Docker/Fly
  - `compose.yml` exposes client on 80, API on 8080, Postgres on 5432 with simple creds (change for real use).
  - `fly.toml` files define apps `jerneproject-client1` and `jerneproject-server1`; adjust names/regions and secrets before deploy.

## Security & authorization
- Authentication: Email/password login at `POST /api/Auth/login` issues a JWT signed with `JWT_SECRET` (HMAC SHA-512). Token includes `sub` (user id) and `role` claim.
- Roles: `Administrator` and `Bruger` (user). Controllers enforce `[Authorize(Roles = ...)]`:
  - Users: admin can list/create/update/delete; both roles can read their own; subscription checks require auth.
  - Boards: create as `Bruger`; admin can list, update, delete; both roles can fetch by user id.
  - Transactions: create/purchase as `Bruger`; admin can list/update/delete; both roles can list by user id.
  - Winning boards: admin-only endpoints.
  - Passwords: users can change their own (`User-change-password` requires token), admins can reset others.
- Frontend route protection: `ProtectedRoute` blocks unauthenticated users; `AdminProtectedRoute` and `UserProtectedRoute` decode JWT `role` claim and redirect to `/login` if mismatched.
- Token storage: sessionStorage via `TOKEN_KEY`; `api-clients.ts` attaches `Authorization: Bearer <token>` when present.
- Secrets: `JWT_SECRET` and database credentials must be rotated and supplied via environment; the checked-in example `.env` is for local/dev only.

## Linting, formatting, testing
- Client: ESLint configured in `client/eslint.config.js`. Commands: `npm run lint` and `npm run lint:fix`. TypeScript build check via `npm run build` (tsc + Vite).
- API/Server: No explicit lint/format scripts; standard `dotnet format` can be added. Unit test project in `server/test/` (xUnit); run with `dotnet test` from `server/`.

## Project status
- Working:
  - JWT login flow and protected API endpoints.
  - Role-gated UI routes for admin/user dashboards, transactions, board history, winner history, user management, first-login flow.
  - TS client generation from OpenAPI on API startup keeps `client/src/models/ServerAPI.ts` in sync.
  - Docker Compose local stack with Postgres, API, and client behind nginx.
- Known issues / risks:
  - CORS is fully open; restrict origins before production.
  - `.env` sample in repo contains secrets; rotate and avoid committing real keys.
  - `AuthController` uses `.Result` when creating the token (sync over async); could deadlock in other contexts — should be awaited.
  - No automated CI, lint, or test hooks yet; consider adding GitHub Actions.
  - No rate limiting or account lockout on login; consider adding to mitigate brute force.

## Deployment (Fly.io)
- Client: `client/fly.toml` serves built assets on port 80 via nginx; set `BACKEND_URL` secret to the API URL.
- API: `server/fly.toml` serves on 8080. Set Fly secrets: `JWT_SECRET`, `JWT_ISSUER`, `JWT_AUDIENCE`, `POSTGRES_*` or point to managed DB. Build with `dotnet publish` (already in `server/Dockerfile`).

## API docs
- Swagger/OpenAPI enabled; visit `/swagger` on the API host. Generated spec also written to `server/api/openapi-with-docs.json` at runtime.

## Contributing / next steps
- Add CI pipeline for lint + tests.
- Harden CORS, secrets management, and login throttling before production.
- Extend README with API examples once endpoints stabilize.
