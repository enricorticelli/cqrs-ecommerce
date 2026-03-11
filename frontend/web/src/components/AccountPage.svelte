<script lang="ts">
  import { onMount } from 'svelte';
  import {
    createMyAddress,
    deleteMyAddress,
    fetchMyAddresses,
    fetchMyOrders,
    fetchMyProfile,
    logoutCustomer,
    updateMyProfile,
    type AccountAddress,
    type AccountOrderSummary,
    type AccountProfile
  } from '../lib/api';
  import { clearAuthTokens, getAccessToken, getRefreshToken } from '../lib/auth';

  let isLoading = true;
  let error = '';
  let profile: AccountProfile | null = null;
  let addresses: AccountAddress[] = [];
  let orders: AccountOrderSummary[] = [];

  let firstName = '';
  let lastName = '';
  let phone = '';

  let addressLabel = 'Casa';
  let addressStreet = '';
  let addressCity = '';
  let addressPostalCode = '';
  let addressCountry = 'Italia';
  let addressDefaultShipping = true;
  let addressDefaultBilling = true;

  async function loadAll() {
    isLoading = true;
    error = '';

    const accessToken = getAccessToken();
    if (!accessToken) {
      window.location.href = '/login';
      return;
    }

    try {
      const [profileResponse, addressResponse, ordersResponse] = await Promise.all([
        fetchMyProfile(accessToken),
        fetchMyAddresses(accessToken),
        fetchMyOrders(accessToken)
      ]);

      profile = profileResponse;
      firstName = profileResponse.firstName;
      lastName = profileResponse.lastName;
      phone = profileResponse.phone;
      addresses = addressResponse;
      orders = ordersResponse;
    } catch (err) {
      error = err instanceof Error ? err.message : 'Errore nel caricamento account.';
    } finally {
      isLoading = false;
    }
  }

  async function saveProfile() {
    const accessToken = getAccessToken();
    if (!accessToken) return;

    try {
      profile = await updateMyProfile(accessToken, {
        firstName,
        lastName,
        phone
      });
    } catch (err) {
      error = err instanceof Error ? err.message : 'Errore nel salvataggio profilo.';
    }
  }

  async function addAddress() {
    const accessToken = getAccessToken();
    if (!accessToken) return;

    try {
      const created = await createMyAddress(accessToken, {
        label: addressLabel,
        street: addressStreet,
        city: addressCity,
        postalCode: addressPostalCode,
        country: addressCountry,
        isDefaultShipping: addressDefaultShipping,
        isDefaultBilling: addressDefaultBilling
      });

      addresses = [created, ...addresses];
      addressStreet = '';
      addressCity = '';
      addressPostalCode = '';
      addressCountry = 'Italia';
    } catch (err) {
      error = err instanceof Error ? err.message : 'Errore creazione indirizzo.';
    }
  }

  async function removeAddress(addressId: string) {
    const accessToken = getAccessToken();
    if (!accessToken) return;

    try {
      await deleteMyAddress(accessToken, addressId);
      addresses = addresses.filter((x) => x.id !== addressId);
    } catch (err) {
      error = err instanceof Error ? err.message : 'Errore eliminazione indirizzo.';
    }
  }

  async function logout() {
    const refreshToken = getRefreshToken();
    clearAuthTokens();

    if (refreshToken) {
      await logoutCustomer(refreshToken).catch(() => undefined);
    }

    window.location.href = '/';
  }

  onMount(loadAll);
</script>

<div class="space-y-6 reveal">
  <div class="flex items-start justify-between gap-3">
    <div>
      <h1 class="font-title text-3xl font-extrabold text-[#202223]">Il mio account</h1>
      <p class="mt-1 text-sm text-[#616161]">Profilo, indirizzi e cronologia ordini.</p>
    </div>
    <button type="button" class="btn-secondary" on:click={logout}>Logout</button>
  </div>

  {#if isLoading}
    <div class="surface-card p-6 text-sm text-[#616161]">Caricamento account...</div>
  {:else}
    {#if error}
      <div class="rounded-xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700">{error}</div>
    {/if}

    {#if profile}
      <section class="surface-card space-y-4 p-6">
        <h2 class="font-title text-2xl font-bold text-[#202223]">Profilo</h2>

        <div class="grid gap-4 sm:grid-cols-2">
          <label>
            <span class="form-label">Nome</span>
            <input class="form-input" bind:value={firstName} />
          </label>
          <label>
            <span class="form-label">Cognome</span>
            <input class="form-input" bind:value={lastName} />
          </label>
          <label class="sm:col-span-2">
            <span class="form-label">Email</span>
            <input class="form-input" value={profile.email} readonly />
          </label>
          <label class="sm:col-span-2">
            <span class="form-label">Telefono</span>
            <input class="form-input" bind:value={phone} />
          </label>
        </div>

        <button type="button" class="btn-primary" on:click={saveProfile}>Salva profilo</button>
      </section>
    {/if}

    <section class="surface-card space-y-4 p-6">
      <h2 class="font-title text-2xl font-bold text-[#202223]">Indirizzi</h2>

      <div class="grid gap-4 sm:grid-cols-2">
        <label>
          <span class="form-label">Etichetta</span>
          <input class="form-input" bind:value={addressLabel} />
        </label>
        <label>
          <span class="form-label">Via</span>
          <input class="form-input" bind:value={addressStreet} />
        </label>
        <label>
          <span class="form-label">Citta</span>
          <input class="form-input" bind:value={addressCity} />
        </label>
        <label>
          <span class="form-label">CAP</span>
          <input class="form-input" bind:value={addressPostalCode} />
        </label>
        <label>
          <span class="form-label">Paese</span>
          <input class="form-input" bind:value={addressCountry} />
        </label>
      </div>

      <div class="flex flex-wrap items-center gap-4 text-sm text-[#4a4f55]">
        <label class="flex items-center gap-2">
          <input type="checkbox" bind:checked={addressDefaultShipping} />
          Predefinito spedizione
        </label>
        <label class="flex items-center gap-2">
          <input type="checkbox" bind:checked={addressDefaultBilling} />
          Predefinito fatturazione
        </label>
      </div>

      <button type="button" class="btn-primary" on:click={addAddress}>Aggiungi indirizzo</button>

      <div class="space-y-3">
        {#if addresses.length === 0}
          <p class="text-sm text-[#616161]">Nessun indirizzo salvato.</p>
        {:else}
          {#each addresses as address}
            <div class="rounded-xl border border-[#e1e3e5] bg-white p-4">
              <div class="flex items-start justify-between gap-3">
                <div>
                  <p class="font-semibold text-[#202223]">{address.label}</p>
                  <p class="text-sm text-[#4a4f55]">{address.street}</p>
                  <p class="text-sm text-[#4a4f55]">{address.postalCode} {address.city}, {address.country}</p>
                </div>
                <button type="button" class="text-xs font-semibold text-rose-700" on:click={() => removeAddress(address.id)}>Elimina</button>
              </div>
            </div>
          {/each}
        {/if}
      </div>
    </section>

    <section class="surface-card space-y-4 p-6">
      <h2 class="font-title text-2xl font-bold text-[#202223]">I miei ordini</h2>

      {#if orders.length === 0}
        <p class="text-sm text-[#616161]">Ancora nessun ordine associato all'account.</p>
      {:else}
        <div class="space-y-3">
          {#each orders as order}
            <a href={`/orders/${order.id}`} class="block rounded-xl border border-[#e1e3e5] p-4 transition hover:bg-[#f6f6f7]">
              <div class="flex items-center justify-between gap-3">
                <div>
                  <p class="text-xs uppercase tracking-[0.15em] text-[#8c9196]">Ordine</p>
                  <p class="font-semibold text-[#202223]">#{order.id.slice(0, 8)}</p>
                </div>
                <div class="text-right">
                  <p class="font-semibold text-[#202223]">{order.totalAmount.toFixed(2)} €</p>
                  <p class="text-xs text-[#616161]">{new Date(order.createdAtUtc).toLocaleString('it-IT')}</p>
                </div>
              </div>
              <p class="mt-2 text-sm text-[#4a4f55]">Stato: {order.status}</p>
            </a>
          {/each}
        </div>
      {/if}
    </section>
  {/if}
</div>
