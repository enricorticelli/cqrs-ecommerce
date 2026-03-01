# Architecture Overview

This repository implements a CQRS + Event Sourcing e-commerce baseline with .NET 10 Minimal APIs, Wolverine, Marten, RabbitMQ, PostgreSQL, and a YARP gateway.

## Scope
- Docker-first runtime (`docker compose up --build`).
- Core flow: catalog -> cart -> order -> stock -> payment -> shipping -> final order state.
- No authentication/identity in this phase.

## Core principles
- CQRS (write/read separation).
- Event-driven orchestration for checkout.
- Event sourcing for `Cart` and `Order`.
- Data-per-service (schema-per-service).
- SOLID and dependency inversion across backend layers.

## Services
- `Catalog.Api`: products CRUD.
- `Cart.Api`: cart write/read on event stream.
- `Order.Api`: checkout orchestration and order state.
- `Warehouse.Api`: stock reservation and stock events.
- `Payment.Api`: payment authorization simulation.
- `Shipping.Api`: shipment creation and tracking.
- `User.Api`: demo user profile read.
- `Gateway.Api`: simple YARP reverse proxy.

## Integration event flow
1. `OrderPlacedV1`
2. `StockReservedV1` or `StockRejectedV1`
3. `PaymentAuthorizeRequestedV1`
4. `PaymentAuthorizedV1` or `PaymentFailedV1`
5. `ShippingCreateRequestedV1`
6. `ShippingCreatedV1`
7. `OrderCompletedV1` or `OrderFailedV1`

## Technical governance
- Detailed technical guidelines and implementation rules:
  - `docs/technical-guidelines.md`
- Architectural decisions (ADR):
  - `docs/adr/0001-wolverine-marten.md`
  - `docs/adr/0002-rabbitmq.md`
  - `docs/adr/0003-frontend-astro-svelte.md`
