# Architecture Decision Records (ADR)

## Stato

- `Accepted`: decisione attiva e applicabile.
- `Proposed`: in valutazione.
- `Superseded`: sostituita da una ADR piu recente.
- `Deprecated`: mantenuta solo per storico.

## Indice ADR

| ADR | Titolo | Stato | Data |
| --- | --- | --- | --- |
| [0001](./0001-microservices-pragmatici.md) | Modello architetturale: microservizi pragmatici | Accepted | 2026-03-07 |
| [0002](./0002-comunicazione-inter-context.md) | Comunicazione tra bounded context | Accepted | 2026-03-07 |
| [0003](./0003-data-ownership-database-separati.md) | Ownership dati e database separati | Accepted | 2026-03-07 |
| 0004 | Contract-first e versioning contratti | Proposed | 2026-03-07 |
| 0005 | Eventual consistency e compensazioni | Proposed | 2026-03-07 |
| 0006 | Idempotenza e deduplica messaggi | Proposed | 2026-03-07 |
| 0007 | Osservabilita distribuita minima | Proposed | 2026-03-07 |
| 0008 | Strategia test backend | Proposed | 2026-03-07 |

## Regole di manutenzione

1. Nuova decisione non banale: creare nuova ADR da template.
2. Cambio di decisione: non modificare il passato, creare ADR nuova e marcare la vecchia come `Superseded`.
3. Ogni ADR deve riportare contesto, decisione, alternative, conseguenze e trade-off.
4. Ogni ADR accettata deve essere referenziata dalla documentazione interessata.
