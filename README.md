# dance-school-api-dotnet

REST API for a dance school, built with ASP.NET Core. It is a port of my earlier
Rails application ([fullstack-ruby-on-rails](https://github.com/kamilszwaradzki/fullstack-ruby-on-rails)):
same domain, same business rules, rebuilt in .NET 10 + EF Core + SQL Server.

My day-to-day stack is PHP (Laravel, Symfony). I am building this project to learn
.NET on a real domain instead of another to-do app. Since I already know the domain,
I can focus on how things are done in C#.

## Status

Early stage. What is in the repo right now:

- a solution with two projects: `src/DanceSchool.Api` (controller-based Web API)
  and `tests/DanceSchool.Tests` (xUnit),
- domain entities and an EF Core `DbContext` for SQL Server, with an initial
  migration (unique indexes, check constraints, no cascade deletes),
- SQL Server 2022 in Docker (`docker-compose.yml`),
- a development data seeder (users, courses, groups, enrolments, passes),
- `EnrollmentService` (enrol, waiting list, cancel with promotion), covered by
  integration tests against a real SQL Server (Testcontainers). There is no
  HTTP endpoint for it yet,
- `dotnet-ef` as a local tool in `dotnet-tools.json`.

This README only describes what actually works and is updated as the code grows.

## Roadmap

- [ ] Courses and groups (weekday, start time, capacity, instructor)
- [ ] Group enrolment with a waiting list: when someone cancels, the first person
      in the queue takes the spot
- [ ] Passes with a fixed number of entries and an expiry date
- [ ] Attendance checked by instructors: presence uses one entry from the pass,
      absence gives it back
- [ ] Monthly lesson generation in the `Europe/Warsaw` time zone
      (DST changes can otherwise shift lessons by an hour)
- [ ] Three roles: admin, instructor, participant (JWT)
- [x] SQL Server in Docker (`docker compose up -d`)
- [ ] The API itself in `docker compose` as well

## Running locally

Requires the .NET 10 SDK and Docker.

1. Start SQL Server. Put a strong SA password in `.env` first (at least 8 characters,
   three of: upper case, lower case, digits, symbols; otherwise the container will
   not start):

   ```bash
   cp .env.example .env
   docker compose up -d --wait
   ```

   `--wait` returns once the healthcheck reports the database as ready.

2. Store the connection string and the password for seeded accounts in
   [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
   (they are not kept in the repo). Use the same SA password as in `.env`:

   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
     "Server=localhost,1433;Database=DanceSchool;User Id=sa;Password=<SA password>;TrustServerCertificate=True" \
     --project src/DanceSchool.Api
   dotnet user-secrets set "Seed:Password" "<password for seeded accounts>" \
     --project src/DanceSchool.Api
   ```

3. Create the database and run the API:

   ```bash
   dotnet tool restore
   dotnet ef database update --project src/DanceSchool.Api
   dotnet run --project src/DanceSchool.Api
   ```

   In Development the API also applies pending migrations on startup and then
   seeds sample data if the `Users` table is empty, so step 3 works on a fresh
   database even without `dotnet ef database update`.

The API listens on `http://localhost:5075` (`http` profile) or
`https://localhost:7014` (`https` profile). In Development, the OpenAPI document
is available at `/openapi/v1.json`.

Seeded accounts all use the `Seed:Password` value, e.g. `admin@danceschool.pl`
(admin), `jan.kowalski@danceschool.pl` (instructor), `piotr.wisniewski@example.com`
(participant).

## Tests

```bash
dotnet test
```

The tests start their own SQL Server container through Testcontainers, so Docker
must be running. They do not use the database from `docker compose`.
