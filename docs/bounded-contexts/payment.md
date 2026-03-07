# Bounded Context: Payment

## Scopo

Gestire autorizzazione e stato dei pagamenti.

## Responsabilita

1. Avvio sessione o richiesta di autorizzazione.
2. Gestione esito (`Authorized`, `Rejected`, `Failed`).
3. Produzione di eventi di risultato pagamento.

## Ownership dati

- Sessioni di pagamento.
- Stato autorizzazione.
- Riferimenti transazione.

## Integrazioni

- Consuma richieste di autorizzazione da Order.
- Pubblica esiti di pagamento verso Order.
- Espone endpoint tecnici verso frontend/pagine hosted quando necessari.

## Confini

Payment non decide stato ordine finale: pubblica esiti e resta owner del solo dominio pagamento.
