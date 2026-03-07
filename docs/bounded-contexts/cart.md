# Bounded Context: Cart

## Scopo

Gestire il carrello utente fino al checkout.

## Responsabilita

1. Aggiunta/rimozione articoli.
2. Calcolo totale carrello.
3. Produzione dei dati necessari all'avvio ordine.

## Ownership dati

- Carrello.
- Articoli del carrello.
- Stato checkout locale al contesto.

## Integrazioni

- Consuma dati prodotto necessari al carrello tramite contratti.
- Espone API per frontend.
- Pubblica eventi legati al checkout.

## Confini

Il carrello non modifica ordini o pagamenti: invia solo intent/eventi.
