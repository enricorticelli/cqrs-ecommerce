# ADR 0007: Shipping Operational Management + Manual Order Override

## Status
Accepted

## Context
The system needed explicit operational capabilities in backoffice for shipment lifecycle management, while preserving architectural constraints (pure CQRS, event-driven backend integration, clean layering).

In parallel, operations required an explicit manual fallback to cancel orders already marked as completed.

## Decision
1. Extend `Shipping` with CQRS-based operational endpoints for:
- shipment listing
- lookup by order id
- shipment status update

2. Keep endpoints thin in `Shipping.Api` and route all capabilities through command/query handlers and dispatchers.

3. Introduce dedicated backoffice section/page for shipment operations.

4. Approve controlled manual override in order management:
- allow `Completed -> Failed` transition only from backoffice manual action
- keep automatic event-driven workflow unchanged

5. Keep checkout integration asynchronous; no direct HTTP service-to-service in core workflow.

## Consequences
- Better operational control for logistics without violating module boundaries.
- Consistent CQRS implementation in Shipping aligned with the rest of the backend modules.
- Explicitly documented exception to terminal-state regression rules.
- Documentation and architecture guidance remain aligned with implemented behavior.
