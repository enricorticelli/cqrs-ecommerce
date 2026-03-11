# Bounded Context: Account

## Scopo

Gestire identity e profilo per storefront clienti e backoffice admin con confini chiari tra i due realm.

## Responsabilita

1. Registrazione/login/logout/refresh utenti customer.
2. Login/logout/refresh utenti admin con RBAC base.
3. Profilo cliente (`me`) e rubrica indirizzi.
4. Aggregazione cronologia ordini cliente tramite query al contesto Order.

## Ownership dati

- Utenti e credenziali hashate (customer/admin).
- Token di refresh e codici temporanei (verifica email, reset password).
- Profilo cliente e indirizzi.

## Integrazioni

- Espone endpoint `store/account` e `admin/account` via gateway.
- Interroga `Order` per la vista ordini associati all'utente autenticato.

## Confini

Account non modifica lo stato ordine, pagamento o spedizione; aggrega solo dati di lettura cross-context.
