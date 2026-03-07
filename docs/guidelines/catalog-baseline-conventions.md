# Convenzioni Baseline Modulo (Derivate da Catalog)

## Obiettivo

Definire uno standard riusabile per i bounded context backend, prendendo `Catalog` come baseline implementativa.

## Struttura per layer

1. `Api`
2. `Application`
3. `Domain`
4. `Infrastructure`
5. `Shared.BuildingBlocks` (solo componenti cross-context)

## Convenzioni Api

1. `Program.cs` resta minimale: solo chiamate a extension method di modulo (`Add<Context>Module`, `Use<Context>ModuleAsync`).
2. Endpoint senza business logic: validano input base, leggono correlation id, delegano ai service applicativi.
3. Mapping `View -> Response` in mapper statici dedicati, uno per tipo di risposta.
4. Nessuna dipendenza dell'API da dettagli tecnici (EF, broker, Wolverine, SQL).

## Convenzioni Application

1. Separazione netta command/query.
2. `*CommandService` per use-case di scrittura.
3. `*QueryService` per listing, dettaglio e ricerca (`searchTerm`).
4. Repository come astrazioni di persistence (`I*Repository`), senza logica HTTP o mapping response.
5. Regole cross-entity in policy/specification (`I*Rules`), non nei service endpoint.
6. Mapper `Entity -> View` in `Application` con interfaccia shared `IViewMapper<TEntity, TView>`.
7. Niente service "fat": un service per responsabilita, composizione tramite DIP.

## Convenzioni Infrastructure

1. Implementazioni repository EF Core separate dai service applicativi.
2. Query di ricerca incapsulate in query object/componenti dedicati.
3. Publish eventi tramite adapter infra (`OutboxDomainEventPublisher`) dietro astrazione application (`IDomainEventPublisher`).
4. Outbox/inbox e durability gestiti da Wolverine, senza leakage nel layer `Application`.
5. Registrazioni DI centralizzate in extension method infrastrutturali del modulo.

## Convenzioni Shared

1. Eventi di integrazione in `Shared.BuildingBlocks.Contracts.IntegrationEvents.<Context>`.
2. Naming eventi versionato: `<EventName>V1`.
3. Un tipo per file.
4. Metadata obbligatori: `eventId`, `occurredAtUtc`, `correlationId`, `sourceContext`.
5. Astrazioni riusabili in shared (es. `IDomainEventPublisher`, `IViewMapper<,>`, eccezioni applicative standard).

## Convenzioni di codice

1. Un tipo per file.
2. Niente classi contenitore senza responsabilita.
3. Nomi espliciti orientati al dominio.
4. Nessun accesso al database di altri bounded context.

## Testing minimo per ogni modulo

1. Unit test su regole/policy.
2. Unit test su command/query service con dipendenze mock/fake.
3. Integration test repository EF su Postgres.
4. Contract test eventi (nome/versione/metadata).
5. Endpoint test per confermare contratti HTTP invariati.
