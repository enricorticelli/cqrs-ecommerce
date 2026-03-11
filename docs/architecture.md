# Architettura Target Backend

## Obiettivo

Realizzare un backend non piu mockato con una architettura a microservizi pragmatici, coerente con i bounded context del dominio e sostenibile da un team ridotto.

## Bounded context

- `Catalog`: gestione catalogo prodotti e metadati commerciali.
- `Cart`: gestione carrello e stato pre-ordine.
- `Order`: lifecycle dell'ordine e orchestrazione del processo.
- `Payment`: autorizzazione e stato pagamento.
- `Shipping`: creazione e avanzamento spedizioni.
- `Warehouse`: disponibilita e riserva stock.
- `Communication`: comunicazioni esterne (email) guidate da eventi.
- `Account`: identity e profilo cliente/backoffice.
- `Gateway`: routing HTTP, nessuna logica di dominio.

## Modello di integrazione

1. Sincrono HTTP tramite gateway per query e comandi utente.
2. Asincrono event-driven tra bounded context per processi cross-dominio.
3. Nessun accesso diretto al database di un altro contesto.
4. Contratti di integrazione espliciti e versionati.

## Coerenza e transazioni

- Coerenza forte solo entro il singolo bounded context.
- Coerenza tra contesti gestita come eventual consistency.
- Ogni flusso cross-context deve prevedere retry, idempotenza e compensazione.

## Vincoli operativi

- Team singolo: privilegiare semplicita e standard ripetibili.
- Time-to-market alto: roadmap per vertical slice.
- Carico medio/alto: separazione dei dati e osservabilita minima obbligatoria.

## Criteri di accettazione architetturale

1. Nessuna dipendenza diretta tra modelli di dominio di contesti diversi.
2. Ogni integrazione cross-context usa API o eventi contrattualizzati.
3. Ogni decisione non banale e tracciata in ADR.
4. Ogni nuovo flusso ha test di dominio, integrazione e contratto.

## Convenzioni implementative di riferimento

1. Ogni modulo adotta separazione `Api/Application/Domain/Infrastructure`.
2. `Api` contiene solo endpoint, contracts e mapper statici `View -> Response`.
3. `Application` separa `CommandService` e `QueryService`, con repository/rules/mappers dedicati.
4. `Infrastructure` implementa repository e adapter tecnici (DB, broker, outbox).
5. Eventi di integrazione condivisi in `Shared.BuildingBlocks.Contracts.IntegrationEvents`.
