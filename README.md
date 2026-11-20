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
- EF Core packages for SQL Server (no `DbContext` or migrations yet),
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
- [ ] SQL Server in Docker + `docker compose up`

## Running locally

Requires the .NET 10 SDK.

```bash
dotnet tool restore
dotnet build
dotnet run --project src/DanceSchool.Api
```

The API listens on `http://localhost:5075` (`http` profile) or
`https://localhost:7014` (`https` profile). In Development, the OpenAPI document
is available at `/openapi/v1.json`.

Tests:

```bash
dotnet test
```