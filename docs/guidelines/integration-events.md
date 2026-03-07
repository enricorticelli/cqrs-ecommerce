# Guideline Integration Events

## Obiettivo

Definire eventi di integrazione chiari, versionati e resilienti per comunicazione tra bounded context.

## Regole di design

1. Evento = fatto di business gia avvenuto.
2. Nome evento al passato (`OrderPlacedV1`, `PaymentAuthorizedV1`).
3. Payload minimo necessario per il consumer.
4. Nessuna dipendenza da entity interne di altri contesti.
5. Definizione contratti in `Shared.BuildingBlocks.Contracts.IntegrationEvents.<Context>`.
6. Un tipo evento per file.

## Versioning

1. Ogni breaking change richiede nuova versione evento.
2. Preferire estensione backward-compatible quando possibile.
3. Deprecare versioni vecchie con piano esplicito.

## Metadata minimi

1. `eventId` univoco.
2. `occurredAtUtc`.
3. `correlationId` per tracing end-to-end.
4. `sourceContext`.

## Consumer policy

1. Handler idempotenti per default.
2. Retry con backoff su errori transient.
3. Dead-letter queue e runbook di replay.
4. Logging strutturato con correlation id.

## Producer policy

1. `Application` dipende solo da `IDomainEventPublisher`.
2. L'implementazione tecnica (es. Wolverine outbox) resta in `Infrastructure`.
3. Publish evento e persistenza stato devono avvenire nello stesso boundary transazionale.

## Contract testing

1. Ogni producer pubblica schema/event contract.
2. Ogni consumer valida compatibilita prima del deploy.

## ADR correlate

- `../adr/0002-comunicazione-inter-context.md`
- `../adr/0003-data-ownership-database-separati.md`
- `../adr/0004-contract-first-versioning.md`
- `../adr/0005-eventual-consistency-compensazioni.md`
- `../adr/0006-idempotenza-deduplica.md`
