# Convenzioni Endpoint HTTP

## Obiettivo

Definire uno standard unico per naming, path, versioning e separazione dei contesti `store`/`admin`.

## Pattern base

1. Tutti gli endpoint pubblici passano dal gateway.
2. Formato path pubblico obbligatorio:
   - `/api/{context}/{service}/v{version}/...`
3. `context` consentiti:
   - `store`
   - `admin`
4. `service` coincide con il bounded context esposto (`catalog`, `cart`, `order`, `payment`, `shipping`, `warehouse`).

## Versioning

1. La versione e sempre nel path (`v1`, `v2`, ...).
2. Per default si estende la versione corrente solo con modifiche backward-compatible.
3. Le breaking change seguono ADR-0004.
4. Eccezione attiva registrata: separazione `store`/`admin` con cut-over diretto su `v1` (vedi ADR-0004).

## Regole di esposizione

1. Whitelist strict nel gateway: si espongono solo gli endpoint esplicitamente autorizzati.
2. Nessuna route legacy pubblica senza contesto (`/api/{service}/...`).
3. Ogni endpoint deve appartenere chiaramente a `store` o `admin` in base al consumer.
4. Se uno stesso handler e disponibile in entrambi i contesti, i nomi endpoint devono essere univoci.

## Convenzioni naming endpoint

1. Path resource-oriented: nomi al plurale (`/products`, `/orders`, `/shipments`).
2. Azioni non CRUD solo quando necessarie e con verbo esplicito nel segmento finale:
   - `/manual-cancel`
   - `/manual-complete`
3. Nessuna logica di dominio nei mapper endpoint.

## Query params e payload

1. Query params ammessi solo per filtraggio, ricerca e paginazione.
2. Nomi query/payload in `camelCase`.
3. Validazione input al boundary API prima della delega ai service applicativi.

## Codici di risposta

1. `200` per read/update con body.
2. `201` per create con location coerente al path contestualizzato.
3. `204` per delete senza body.
4. `400`/`404`/`409` tramite mapper errori condiviso.

## Checklist PR per nuovi endpoint

1. Path conforme a `/api/{context}/{service}/v{version}/...`.
2. Endpoint aggiunto alla whitelist gateway.
3. Contratto request/response esplicito e testato.
4. Consumer aggiornati (`frontend/web`, `frontend/admin`, script) se impattati.
5. Documentazione aggiornata (`README`, `docs/`, ADR se necessario).
