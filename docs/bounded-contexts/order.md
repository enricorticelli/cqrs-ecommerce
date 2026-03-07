# Bounded Context: Order

## Scopo

Gestire il lifecycle dell'ordine come core process, dal create alla chiusura.

## Responsabilita

1. Creazione ordine con invarianti di dominio.
2. Gestione transizioni di stato ordine.
3. Coordinamento logico del processo cross-context tramite eventi.

## Ownership dati

- Ordine.
- Stato ordine.
- Informazioni cliente e indirizzi associati all'ordine.

## Integrazioni

- Riceve eventi da Warehouse, Payment, Shipping.
- Pubblica eventi verso Cart e altri contesti interessati.
- Espone API query/comandi per stato ordine.

## Confini

Order non accede ai database esterni e non incorpora regole interne di payment/shipping.
