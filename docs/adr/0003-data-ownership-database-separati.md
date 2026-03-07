# ADR-0003: Ownership dati e database separati per bounded context

- Data: 2026-03-07
- Stato: Accepted
- Decisori: Product/Tech Owner
- Consultati: Stakeholder progetto
- Informati: Team backend/frontend

## Contesto

La soluzione richiede isolamento semantico tra contesti e carico medio/alto. L'uso di database condivisi aumenterebbe il coupling, rendendo piu difficile evolvere in modo indipendente i modelli di dominio.

## Decisione

Ogni bounded context possiede i propri dati e il proprio database/schema.

1. Nessun accesso diretto al database di un altro contesto.
2. Integrazione dati solo tramite API o eventi.
3. Modelli di persistenza indipendenti per contesto.
4. Migrazioni e lifecycle dati governati dal contesto proprietario.

## Alternative considerate

1. Database unico condiviso: semplice all'inizio, ma alto accoppiamento e rischio regressioni cross-context.
2. Database unico con schema separati ma accesso promiscuo: riduce parte del rischio ma non elimina violazione ownership.
3. Copia dati non governata: veloce nel breve, debito tecnico alto nel medio periodo.

## Conseguenze

### Positive

- Autonomia evolutiva dei contesti.
- Migliore allineamento con bounded context DDD.
- Riduzione impatti collaterali su modifiche dati.

### Negative / Trade-off

- Maggior lavoro di integrazione e sincronizzazione tra contesti.
- Necessita di osservabilita e riconciliazione dati distribuiti.

## Impatto su implementazione

- Definire ownership per ogni entita business nei documenti dei bounded context.
- Introdurre politiche di versioning contratti e migrazioni backward-compatible.
- Prevedere processi di compensazione in caso di inconsistenze temporanee.

## Piano di adozione

1. Mappare ownership dati per contesto in `docs/bounded-contexts/`.
2. Implementare integrazioni solo via contratti pubblici.
3. Aggiungere check architetturali in review per prevenire accessi cross-db.

## Riferimenti

- `../architecture.md`
- `./0002-comunicazione-inter-context.md`
