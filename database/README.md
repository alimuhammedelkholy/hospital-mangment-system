# Database assets

## Structure

- `schema/HMS.sql` — the SQL Server schema source of truth. Foundation work must not modify it.
- `scripts/` — reserved for additive, reviewed operational or deployment scripts.

Do not place application source code or generated EF migrations in this directory without an approved database-ownership decision.
