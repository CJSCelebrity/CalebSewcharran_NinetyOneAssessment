# Design Notes

This document is intended to record the design process of how I interpreted the project instructions and what was spoken off in the brief. It includes the decisions I made and, where applicable, the tradeoffs that were made. If a requirement was ambiguous in the instructions, I have stated so in the document

---

## 1. Interpretation of the brief

**Alphabetical ordering.** The brief says tied top scorers are output in
alphabetical order, but does not say alphabetical by what. Sorting by second name
would put "Sipho Lolo" before "George Of The Jungle"; the expected output in the
brief shows the reverse. I therefore sort by the rendered full name
(`FirstName SecondName`), which reproduces the stated output.

**"A way to input data via a plain-text file."** I read this as a statement
about the input *mechanism*, a file on disk rather than a GUI or a database, 
not as a request to support multiple file formats. A `.csv` file is a plain-text
file, so a single reader satisfies it. I initially built a factory to select
between a CSV reader and a text reader and removed it: both implementations were
identical, so the abstraction selected between things that did not differ. If
formats did multiply, then we would expand the parser to accomodate more file types by extending the functionality that it already offers.

**"Output directed to STDOUT."** This implies the program is scriptable, so input is taken as a command-line argument rather than an
interactive dialog. Results are output to the console; diagnostics go to serilog error logging in the console, so
`app > results.txt` captures exactly the answer and nothing else.

**"Write the data you read into a database table."** I persist every valid row
from the file. The top scorers are a computed view over that data, not the data itself.

---

## 2. Assumptions

- The CSV has a header row, and columns are bound by header *name*
- First name + second name is the only available identity. It is not unique in
  the real world; see point 4.
- Scores are whole numbers and non-negative.
- Input is UTF-8; a byte-order mark is detected and stripped so it does not
  corrupt the first header field.

  ---

## 3. Solution shape

```
Core (console)  ──┐
                  ├──> Application ──> (interfaces, domain model, use cases)
Api             ──┘         ▲
                            │
                     Infrastructure  (EF Core, SQLite, file I/O)
```

The dependency arrow runs Infrastructure → Application, as adhered to by using Clean Architecture principles. Application defines the interfaces; Infrastructure implements them; the composition roots (console and API) wire them together.

The pipeline is four single-purpose steps:

| Step | Responsibility | Knows about |
|---|---|---|
| `FileReaderService` | path → text | the filesystem |
| `CsvParserService` | text → rows of fields | commas, quotes, newlines |
| `PersonMapper` | rows → `Person` | headers, types, validity |
| `TopScorerService` | `Person[]` → winners | scores |

The split is driven by testability, not tidiness. The parser and the mapper are
pure functions i.e same input, same output, on any machine and so they are tested
directly with no fixtures. Only the reader touches the outside world, and it is
the one component I did not unit test.

**A note on layering.** Four projects is more structure than a problem this size
needs. I kept it because the brief asks to see an application I could maintain
and evolve, and the boundaries are where they would be in a larger system. In a
real codebase of this scope I would probably start with two.

---

## 4. Key decisions

**Hand-written CSV parser (required by the brief).** A single character loop
with an in-quotes state flag, rather than utilizing the `Split(',')` or a regex. Both of those break on the case the constraint exists to catch: a comma or a newline inside a quoted field. The parser handles quoted delimiters, quoted newlines, escaped
double-quotes, empty fields, CRLF and LF, and files with or without a trailing
newline. An unterminated quote at end-of-input is a structural failure and
throws.

**No AutoMapper.** Mapping three fields does not justify a dependency, and
runtime-configured mapping fails at runtime rather than at compile time. The
recent MediatR licence change (https://www.jimmybogard.com/automapper-and-mediatr-going-commercial/) is a concrete reminder that a transitive
dependency can become a commercial problem; for something this small, a
hand-written mapper has no downside.

**Two models, not one.** `Person` is an immutable domain record; `PersonEntity`
is a mutable EF class in Infrastructure. Sharing one type would mean either EF
attributes on the domain model or JSON attributes on the persistence model. The
API has its own request/response contracts for the same reason: the wire format
should be free to change without touching the domain.

**Ties handled by filtering, not counting.** Find the maximum score, then return
everyone who has it.

**SQLite, schema from migrations.** Chosen so the project runs on a clean clone
with no container and no connection string to configure. Migrations are
committed; the `.db` file is gitignored, because a binary cannot be reviewed and
drifts from the schema it was built from. `Database.Migrate()` runs at startup
so the database is created and current on first run. Trade-off accepted: SQLite
tells us nothing about behaviour under concurrent writes. See point 7.

**Surrogate key.** The CSV provides no natural key, and names are not unique, so
the table has an `Id` the source data does not. This is my addition, and it is
the honest consequence of the schema the brief specifies.

**Design-time DbContext factory.** `dotnet ef` needs to construct the context
without booting an application host. The factory hardcodes a local connection
string for that purpose only; runtime configuration is unaffected.

---

## 5. Error handling policy

Two categories of bad input, handled differently:

**Structural failures** A file with an unterminated quote means the file cannot be
trusted, so the run fails with a non-zero exit code.

**Row-level failures** Where there is a non-numeric score, a missing field, an empty name, these are collected with their record numbers, reported on the console, and the remaining valid rows are processed. I aimed for a partial success instead of an outright fail.

I went back and forth on this. The argument against is that a scoring system
where a row is silently dropped can produce a *wrong winner*, not just an
incomplete list. The argument for is that a reviewer running the tool on their
own data gets a useful answer plus a precise complaint, rather than nothing. I
chose partial success and made the failures loud.

Exit codes: `0` success, `1` processing failure, `2` usage error.

---

## 6. Testing approach

Tests concentrate on the two components with real logic: the CSV parser and the
mapper. The parser tests read as a specification of the format — one test per
edge case, named for the behaviour. The end-to-end test asserts the exact
expected output against the supplied `TestData.csv`, so the brief's stated
result is verified on every push rather than demonstrated once in a screenshot.

Deliberately not unit tested: the file reader (a thin wrapper over
`File.ReadAllTextAsync`) and the console writer (would require redirecting
`Console.Out` to assert on formatting). Both are covered indirectly by the
end-to-end test, and testing them in isolation would assert that the framework
works.

---

## 7. Securing the API

The endpoints are currently unauthenticated. What I would do, in layers:

**Transport.** HTTPS enforced. This is assumed rather than discussed.

**Authentication.** For a service-to-service API, OAuth2 client credentials with
JWT bearer tokens, validated on signature, issuer, audience and expiry
(`AddAuthentication().AddJwtBearer()` plus `[Authorize]`). In an asset manager
the realistic answer is to integrate with the organisation's existing identity
provider rather than issue tokens here. The auth is not something an individual
service should reinvent.

An API key is the lighter alternative for a trusted internal caller. It is
weaker: no expiry, no identity, and rotation means coordinating with every
consumer. I would use it only for an internal, low-risk integration.

**Authorisation.** Read and write are not the same risk. `POST /scores` mutates
the record and should require a different scope or role than the GET endpoints.
Separating them matters more than which mechanism issues the token.

**Around the edges.** Model validation on inbound requests (implemented). Rate
limiting to protect the database. Connection strings from a secret store, never
from committed configuration. Structured logging of who wrote what, since score
data is the kind of thing people dispute.

---

## 8. Hosting in the cloud

Assuming AWS, and assuming a UI is added:

| Concern | Component | Why |
|---|---|---|
| Edge | CloudFront + S3 | Static SPA, cached at the edge |
| API entry | API Gateway | TLS termination, throttling, auth integration, one place for the public contract |
| Compute | ECS Fargate | Predictable latency, no cold starts, no execution ceiling |
| Data | RDS PostgreSQL (Multi-AZ) | Concurrent writes, managed backups, point-in-time recovery |
| Identity | Cognito, or the corporate IdP | Token issuance and validation |
| Secrets | Secrets Manager | Connection string, rotated, never in config |
| Images | ECR | Versioned, scanned |
| Observability | CloudWatch logs, metrics, alarms | Structured logs already emitted via Serilog |
| Provisioning | Terraform | Reviewable, repeatable, environment parity |

**Networking:** API Gateway public, Fargate tasks in private subnets, RDS in
isolated subnets reachable only from the task security group. IAM task roles
rather than credentials anywhere.

**Fargate over Lambda, with a caveat.** Lambda is cheaper at low traffic and
scales to zero, which suits an internal API used a few times a day. I chose
Fargate for predictable latency and because bulk CSV processing is the kind of
workload that runs into the 15-minute execution limit. If the traffic profile
proved to be genuinely high throughout usage, then a Lambda would be the better
choice and I would switch.

**Bulk upload would not go through the API.** Posting a large CSV through API
Gateway means a request timeout budget and a payload limit. The better request is
a presigned S3 upload, an S3 event, and a worker that processes the object
asynchronously so that the API then reports job status rather than doing the work
inline.

---

## 9. What I would do differently with more time

- **Property-based tests for the parser.** Round-tripping generated records
  through a writer and back would find edge cases my example-based tests miss.
- **A `Score` value object** instead of a raw `int`, so an invalid score cannot
  be constructed at all rather than being validated at the boundary.
- **Idempotent bulk load.** Loading currently replaces the table so repeated
  runs stay consistent. Upserting on a real identifier would be correct; names
  are not one.
- **Integration tests for the API** using an in-memory or file-backed SQLite
  database and `WebApplicationFactory`.
- **Collapse the layering.** Due to Clean Architecture having multiple layers, there are some layers that have not been expanded upon efficiently. So these layers could be removed upon inspection