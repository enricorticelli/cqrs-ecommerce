# ADR 0005: Use MongoDB for CQRS Read Models

## Status
Accepted

## Context
Read endpoints were initially tied too closely to event/document state in write persistence. CQRS requires dedicated query-side models optimized for reads and isolated from write/event concerns.

## Decision
Use MongoDB as the query-side store for read models across all backend modules, while keeping PostgreSQL + Marten for write model and event streams.

## Consequences
- Clear write/read separation aligned with CQRS.
- Query models can evolve independently from event stream schema.
- Additional projection consistency and operational monitoring become necessary.
