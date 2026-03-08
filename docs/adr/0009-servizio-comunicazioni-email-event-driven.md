# ADR-0009: Servizio Communication per email esterne event-driven

- Data: 2026-03-08
- Stato: Accepted
- Decisori: Product/Tech Owner
- Consultati: Stakeholder progetto
- Informati: Team backend/frontend

## Contesto

Serve introdurre comunicazioni utente (conferma ordine e notifica spedizione) senza accoppiare i bounded context core a provider email o chiamate sincrone tra servizi.

## Decisione

Adottare un nuovo bounded context `Communication` con consumo asincrono di eventi e invio email SMTP.

1. `Order` pubblica `OrderCompletedForCommunicationV1` con snapshot email destinatario.
2. `Shipping` pubblica `ShipmentInTransitForCommunicationV1` su transizione a `InTransit`.
3. `Communication` consuma eventi dedicati e invia email tramite adapter SMTP.
4. Idempotenza obbligatoria nei consumer con deduplica persistente.
5. Ambiente locale con Mailpit come mock SMTP e UI inbox.

## Alternative considerate

1. Invio email diretto da `Order`/`Shipping`: coupling tecnico e responsabilita mescolate.
2. Chiamata HTTP da `Communication` verso altri context per recuperare email: coupling temporale e maggiore fragilita.
3. Provider email reale da subito: complessita operativa non necessaria in fase iniziale.

## Conseguenze

### Positive

- Separazione chiara tra dominio core e canale comunicazione.
- Resilienza grazie a integrazione asincrona e deduplica.
- Estendibilita verso nuovi template/canali senza toccare i servizi core.

### Negative / Trade-off

- Nuovo servizio da mantenere e monitorare.
- Overhead di nuovi contratti evento e queue dedicate.

## Impatto su implementazione

- Nuovi contratti `OrderCompletedForCommunicationV1` e `ShipmentInTransitForCommunicationV1`.
- Nuove queue RabbitMQ: `order-completed-communication`, `shipment-intransit-communication`.
- Nuove configurazioni: `ConnectionStrings__CommunicationDb`, `Communication__Smtp__*`.
- Nuovo servizio `communication-api` e container `mailpit` nel `docker-compose`.

## Piano di adozione

1. Introdurre modulo `Communication` e wiring infrastrutturale.
2. Aggiornare producer `Order` e `Shipping` con eventi dedicati.
3. Aggiungere test handler/contratti e smoke test locale con Mailpit.

## Riferimenti

- `../architecture.md`
- `../bounded-contexts/communication.md`
- `../guidelines/integration-events.md`
