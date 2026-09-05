# Hospital Management System

## Repository layout

```text
.
├── database/
│   ├── schema/              # Immutable SQL Server source-of-truth schema
│   └── scripts/             # Approved additive database operations only
├── src/
│   ├── HMS.Api/             # HTTP host, middleware, contracts, and configuration
│   ├── HMS.Application/     # Use-case abstractions and application services
│   ├── HMS.Domain/          # Database- and framework-independent domain code
│   └── HMS.Infrastructure/  # EF Core and SQL Server implementations
└── tests/
    ├── HMS.UnitTests/       # Isolated Domain/Application tests
    └── HMS.IntegrationTests/# API and infrastructure integration tests
```

`database/schema/HMS.sql` is the database source of truth. No schema object is changed by the application foundation.
