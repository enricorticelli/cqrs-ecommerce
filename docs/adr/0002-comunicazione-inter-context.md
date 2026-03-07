# ADR-0002: Comunicazione tra bounded context

- Data: 2026-03-07
- Stato: Accepted
- Decisori: Product/Tech Owner
- Consultati: Stakeholder progetto
- Informati: Team backend/frontend

## Contesto

I processi di business coinvolgono piu contesti (es. ordine, pagamento, spedizione, magazzino). Serve una strategia uniforme per scegliere tra comunicazione sincrona e asincrona, evitando accoppiamenti forti e semantica ambigua.

## Decisione

Adottare un modello di comunicazione misto con regole esplicite:

1. HTTP sincrono per richieste utente, query immediate e comandi a confine API.
2. Event-driven asincrono per processi cross-context e transizioni di stato multi-step.
3. API gateway solo per routing e policy trasversali, mai per orchestrazione di dominio.
4. Contratti di integrazione versionati e backward-compatible.

## Alternative considerate

1. Solo HTTP sincrono: semplice all'inizio, ma fragile su resilienza e coupling temporale.
2. Solo event-driven: disaccoppiato ma complesso per use case semplici e debugging.
3. Orchestrazione centralizzata nel gateway: viola i confini di dominio.

## Conseguenze

### Positive

- Riduzione del coupling temporale tra contesti.
- Migliore resilienza dei workflow distribuiti.
- Maggiore chiarezza su responsabilita e ownership.

### Negative / Trade-off

- Aumento complessita su osservabilita e tracing.
- Necessita di politiche chiare su retry, idempotenza e ordering.

## Impatto su implementazione

- Definire linee guida per eventi in `docs/guidelines/integration-events.md`.
- Introdurre test di contratto per API/eventi cross-context.
- Standardizzare correlation id e logging strutturato.

## Piano di adozione

1. Definire eventi del dominio applicativo per ogni context.
2. Applicare policy di retry/idempotenza su handler.
3. Monitorare i workflow distribuiti con metriche e tracing.

## Riferimenti

- `../architecture.md`
- `./0003-data-ownership-database-separati.md`
- `../guidelines/integration-events.md`
