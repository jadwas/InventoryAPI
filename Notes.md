# Notes
Inventory is a simple system, scaffolding, for inventory with possibility to create orders
## Assumptions

- The system follows a clean, layered architecture with clear separation between API, Application (CQRS), Domain, and Infrastructure.
- MediatR is used to enforce a strict command/query pipeline.
- FluentValidation provides request‑level validation, including async rules.
- The API exposes RESTful endpoints, with PATCH used for partial state changes.
- Correlation Id assigned to request helps to identify flow over layers
- Use .Net Core capabilities

## Simplifications
- The same database for both command and query operations, without a separate read model.
- Very optimistic concurrency handling, without complex conflict resolution strategies.
- No authentication or authorization, assuming an internal API or a trusted environment.
- Use SQLite for simplicity, without considering more complex database options.

## Trade-offs
- SQLite InMemory for Tests - simplifies setup and teardown, but may not perfectly mimic behavior of a production database.
- CQRS Granularity - Using a single database for both commands and queries simplifies development but may limit scalability and performance in high-load scenarios.
- Use of MediatR - non-free commercial use, adds an extra layer of abstraction, which can increase complexity and reduce performance, but provides a clean separation of concerns and promotes a more maintainable codebase
