# Introduction

The following project reads a a CSV of people and outputs the scores related to those people. It writes the top scorer or scorers and their relevant marks to the console. In addition, this project makes use of SQLite for the database and exposes it via a small REST API

The CSV Parsing is implemented by hand from string data, as required by the assessment. No CSV library was used

The design decisions and trade-offs are noted off in the DESIGN.md file

N.B The DESIGN.md file contains answers to the following questions:

*Demonstrate how you would secure these endpoints in the API*

*If you had to host this API in the Cloud and create a user interface for it, what Cloud components would you use?*

## Running the console application

From the repository root:

```bash
dotnet run --project NinetyOneAssessment.Core -- ./TestData.csv
```

With no argument it falls back to the bundled `TestData.csv`:

```bash
dotnet run --project NinetyOneAssessment.Core
```

Note the `--`: without it, `dotnet run` treats the path as one of its own
arguments.

### Expected output

For the supplied `TestData.csv`:

```
George Of The Jungle
Sipho Lolo
Score: 78
```

### Exit Codes

The following exit codes are implemented to guide the user of the application to make sense of what the output means.

| Code | Meaning |
|---|---|
| 0 | Success |
| 1 | Processing failure (file unreadable, malformed CSV) |
| 2 | Usage error (too many arguments, empty path) |

`--help` prints usage.

### Running the API

```bash
dotnet run --project NinetyOneAssessment.Api
```

Interactive API documentation is available at `/swagger`
which is the easiest way to exercise the endpoints.

### Endpoints

| Method | Route | Purpose |
|---|---|---|
| `POST` | `/api/scores` | Add a new score |
| `GET` | `/api/scores/topscorers` | Top scorer(s) and the mark |
| `GET` | `/api/scores/{firstName}/{secondName}` | Score(s) for one person |

## Database

SQLite, created automatically on first run by applying EF Core migrations. The
`.db` file is not committed; the migrations are, so a clean clone produces the
same schema.

The table columns mirror the CSV: `First Name`, `Second Name`, `Score`, plus a
surrogate `Id`.

Loading a file replaces the table contents, so repeated runs are idempotent.

## Project structure

| Project | Contains |
|---|---|
| `NinetyOneAssessment.Core` | Console entry point, composition root |
| `NinetyOneAssessment.Api` | REST API, controllers, request/response contracts |
| `NinetyOneAssessment.Application` | Domain model, interfaces, parsing and scoring logic |
| `NinetyOneAssessment.Infrastructure` | EF Core, SQLite, file I/O |
| `NinetyOneAssessment.Tests` | Unit and end-to-end tests |