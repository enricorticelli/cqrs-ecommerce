# ADR 0006: Separate Backoffice Frontend from Public Storefront

## Status
Accepted

## Context
Operational functions (catalog management, warehouse actions, order operations) were mixed into the public storefront experience, creating UX and boundary confusion.

## Decision
Introduce a dedicated admin frontend (`frontend/admin`) with separate runtime and routes, and remove management pages from the public storefront (`frontend/web`).

## Consequences
- Clear separation between customer UX and operational UX.
- Better security posture and future authorization boundary for admin features.
- Two frontend deployables to maintain in Docker and CI/CD.
