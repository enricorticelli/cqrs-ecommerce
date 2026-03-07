# ADR-0001: Modello architetturale a microservizi pragmatici

- Data: 2026-03-07
- Stato: Accepted
- Decisori: Product/Tech Owner
- Consultati: Stakeholder progetto
- Informati: Team backend/frontend

## Contesto

Il backend attuale espone API mock ma la soluzione e gia strutturata in bounded context separati (`Catalog`, `Cart`, `Order`, `Payment`, `Shipping`, `Warehouse`) con gateway dedicato. Il team e ridotto e ha priorita di time-to-market, con previsione di carico medio/alto.

## Decisione

Adottare un modello di microservizi pragmatici:

1. bounded context separati come unita di deploy logica;
2. ownership dati per contesto;
3. integrazione contract-first;
4. complessita infrastrutturale introdotta solo quando necessaria.

## Alternative considerate

1. Modular monolith: piu semplice operativamente, ma riduce isolamento e indipendenza di evoluzione dei context gia separati.
2. Microservizi full enterprise: massima autonomia, ma overhead eccessivo per team singolo.
3. Architettura ibrida non governata: flessibile nel breve, ma alto rischio di incoerenza architetturale.

## Conseguenze

### Positive

- Confini di dominio chiari e allineati al modello DDD strategico.
- Scalabilita e evoluzione per contesto.
- Riduzione del rischio di accoppiamento semantico.

### Negative / Trade-off

- Maggior complessita operativa rispetto al monolite.
- Necessita di disciplina alta su contratti, osservabilita e test.

## Impatto su implementazione

- Ogni contesto implementa vertical slice complete (API + application + domain + infrastructure).
- No logica di business nel gateway.
- Ogni variazione architetturale cross-context richiede ADR.

## Piano di adozione

1. Formalizzare decisioni operative in ADR-0002 e ADR-0003.
2. Implementare primi flussi reali su `Order`, `Payment`, `Shipping`.
3. Estendere agli altri contesti con stessa governance.

## Riferimenti

- `../architecture.md`
- `./0002-comunicazione-inter-context.md`
- `./0003-data-ownership-database-separati.md`
