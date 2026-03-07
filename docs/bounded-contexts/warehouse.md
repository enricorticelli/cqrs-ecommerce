# Bounded Context: Warehouse

## Scopo

Gestire disponibilita e prenotazione stock per gli ordini.

## Responsabilita

1. Aggiornamento disponibilita inventario.
2. Riserva stock su richiesta workflow ordine.
3. Emissione esito riserva (`Reserved`/`Rejected`).

## Ownership dati

- Stock per prodotto.
- Prenotazioni stock.

## Integrazioni

- Consuma richieste di riserva dal processo ordine.
- Pubblica esito verso Order.
- Espone API tecniche di gestione stock.

## Confini

Warehouse non modifica ordini e non accede a dati di pagamento/spedizione.
