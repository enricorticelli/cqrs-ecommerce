# Bounded Context: Catalog

## Scopo

Gestire il catalogo prodotti e i metadati commerciali (brand, categorie, collezioni).

## Responsabilita

1. Definire e mantenere anagrafiche prodotto.
2. Esporre query per listing e dettaglio prodotti.
3. Garantire coerenza dei dati catalogo interni al contesto.

## Ownership dati

- Prodotti.
- Brand.
- Categorie.
- Collezioni.

## Integrazioni

- Espone API al frontend/backoffice via gateway.
- Pubblica eventi quando cambiano informazioni rilevanti per altri contesti.

## Confini

Nessun altro contesto puo leggere/scrivere direttamente il database catalogo.
