# ADR 0004: Standardize Clean Architecture + CQRS Template Across Modules

## Status
Accepted

## Context
Before the refactor, modules had mixed structure and inconsistent application contracts. This made code harder to evolve and increased coupling between API, orchestration, and infrastructure concerns.

## Decision
Adopt a consistent module template for all core services:
- `*.Api`
- `*.Application`
- `*.Domain`
- `*.Infrastructure`

Use shared CQRS abstractions in `Shared.BuildingBlocks` (`ICommand`, `IQuery`, handlers, dispatchers, pipeline behaviors) and keep endpoint handlers thin (HTTP mapping only).

## Consequences
- Uniform module structure and lower cognitive load.
- Cleaner dependency direction and stronger SOLID boundaries.
- Faster onboarding and easier cross-module refactoring.
