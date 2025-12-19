# JerneProject

Kort pitch
En moderne webapplikation til ugentlige lodtrækninger og automatisk match af vindende spilleplader til spillet Døde Duer for Jerne IF — med robust validering, services og en React/Vite klient.

## Om produktet
JerneProject er et fuldt stack system designet til at håndtere købs-flow for spilleplader, administration af ugentlige træk (WinningBoards) og automatisk matching af vindende plader. Serverdelen (C#/.NET) indeholder et lag med domænevalideringer, services og exceptions, samt services til at oprette vindere og matche brugernes plader. Klienten er skrevet med React + Vite og giver en enkel brugeroplevelse til at købe plader, se historik og følge træk.

Projektet er bygget med fokus på:
- Tydelige regel-validatorer (f.eks. for at forhindre flere træk for samme uge).
- Servicelag (services) som udfører forretningslogik og mapping mellem domain-entities og DTOs.
- Tests (unit/integration) der dækker kritiske flows som køb og matching.

## Nøglefunktioner
- Opret og administrer ugentlige vindernumre (`WinningBoard`).
- Automatisk matching af aktive plader mod vindernumre via `BoardMatchService`.
- Regler/valideringer der stopper ugyldige operationer, f.eks. forhindring af mere end ét vindende board pr. uge.
- Database-lag via `dataaccess` (EF Core-migrationer og entities).
- Integrationstests og unit tests i `test/`.
- Frontend i `client/` (Vite + React + TypeScript).

## Arkitektur og mapper (kort)
- `server/` — .NET backend (flere projekter: `dataaccess`, `service`, `api`, `test`).
  - `service/Services` — forretningslogik (fx `WinningBoardService`, `BoardMatchService`).
  - `service/Rules` — valideringsregler og regel-undtagelser (fx `WinningBoardRules`, `BoardMatcherRules`).
  - `dataaccess/Entities` — EF Core entities (`WinningBoard`, `Board`, `WinningNumber`, osv.).
  - `api/` — ASP.NET Web API-controllerne og DI-registrering (`ServiceCollectionExtensions`).
  - `test/` — xUnit-tests.
- `client/` — React/Vite frontend (TypeScript).

## TechStack
- Backend: .NET 10 (C#), Entity Framework Core
- Frontend: React + Vite + TypeScript
- Database: PostgreSQL (forbindelse konfigureres via `ConnectionStringHelper` / appsettings)
- Test: xUnit (server/test)
- Byg / Dev: dotnet CLI fra API'en, npm i Client

## Hurtigstart (lokalt development)
Krav:
- Opret .env ud fra template filen. 
- .NET 10 SDK installeret
- Node.js + npm installeret
- En PostgreSQL instans (eller anden kompatibel database), opret forbindelse  miljøvariabler(env)

Server (CLI)
1) Byg service-projektet:

```powershell
dotnet build 
```

2) Kør hele API (fra `server/api` projektet) efter at have sat connection string i `appsettings.json` eller miljøvariabler:

```powershell
cd 
dotnet run
```

Frontend (PowerShell)
1) Installer afhængigheder og start dev-server:

```powershell
cd 
npm install
npm run dev
```

Åbn den adresserede URL som Vite logger (typisk `http://localhost:5173`).

## Kør tests
- Kør testprojektet:

```powershell
dotnet test 
```

Bemærk: Hvis du oplever filer låst under build (MSB3021/MSB3027), luk eventuelle kørende `dotnet`/API processer eller IDE-instanser der bruger projektets DLL'er, og kør build/tests igen.

## API & brug (overblik)
- Serveren eksponerer endpoints for brugere, boards, winning boards og matching (se `api/Controllers`).
- Eksempel-flow:
  1. Administrator opretter et `WinningBoard` (trukne numre).
  2. `WinningBoardService` validerer via `IWinningBoardRules` (fx antal numre, unikke tal, og nu også: at der ikke allerede findes et `WinningBoard` for samme uge).
  3. Efter oprettelse kaldes `BoardMatchService.GetBoardsContainingNumbersWithDecrementerAsync` for at finde vindende plader og opdatere deres status/antal uger købt.
- Fejlhåndtering: domain/regler kaster specialiserede undtagelser (`DuplicateResourceException`, `ResourceNotFoundException`, `InvalidRequestException`), som global exception middleware bør mappe til passende HTTP-statuskoder (fx 409, 404, 400).

## Sikkerhed & drift
- JWT-baseret auth er sat op i `api` (se `TokenService` og `JWT_*` indstillinger).
- Production-forbindelse til databasen bør styres via sikre miljøvariabler.


## Forslag til næste forbedringer.
- Tilføj DB-unik nøgle (unique constraint) på `(Week, WeekYear)` hos `WinningBoard` for at sikre DB-level konsistens (beskytter mod race-conditions).
- Tilføj flere unit-tests for at dække concurrency / duplikat-scenarier.
- Dokumentér API endpoints i `api/` OpenAPI (der er en `openapi-with-docs.json` i projektet).

## Login (sample credentials)
- Admin:
  - Email: harper.young@example.com
  - Password: Silk!713
  - Rolle: Administrator

- Bruger (normal bruger):
  - Email: isabella.lopez@example.com
  - Password: Charm?478
  - Rolle: Bruger

## Contributors
- https://github.com/benj4515
- https://github.com/GreenzQe
- https://github.com/MathiasFIv
