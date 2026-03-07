# Bounded Context: Shipping

## Scopo

Gestire creazione spedizioni e avanzamento dello stato logistico.

## Responsabilita

1. Creazione shipment a seguito di trigger di processo.
2. Generazione tracking code.
3. Aggiornamento stato spedizione nel tempo.

## Ownership dati

- Spedizione.
- Tracking code.
- Stato consegna.

## Integrazioni

- Consuma richieste di creazione da Order.
- Pubblica esiti/aggiornamenti verso Order.
- Espone API per query stato spedizione.

## Confini

Shipping non gestisce logiche di pagamento o stock.
