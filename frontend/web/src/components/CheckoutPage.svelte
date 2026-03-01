<script lang="ts">
  import { onMount } from 'svelte';
  import { createOrder } from '../lib/api';
  import { getProductImage } from '../lib/mock';
  import { formatCurrency } from '../lib/format';
  import { cartId, userId, cartItems, cartTotal, clearCart } from '../stores/cart';
  import { addToast } from '../stores/ui';

  // ─── Form state ─────────────────────────────────────────────────────────────

  let step: 'shipping' | 'payment' | 'review' = 'shipping';

  // Shipping (mocked – not sent to backend)
  let firstName = 'Mario';
  let lastName = 'Rossi';
  let email = 'mario.rossi@example.com';
  let phone = '+39 333 1234567';
  let address = 'Via Roma 1';
  let city = 'Milano';
  let zip = '20100';
  let country = 'Italia';

  // Payment (mocked – the real flow is driven by the saga)
  let cardName = 'Mario Rossi';
  let cardNumber = '4242 4242 4242 4242';
  let cardExpiry = '12/28';
  let cardCvc = '123';

  // Order
  let isSubmitting = false;
  let submitError = '';

  $: items = $cartItems;
  $: subtotal = $cartTotal;
  $: shipping = subtotal > 0 ? (subtotal >= 120 ? 0 : 7.9) : 0;
  $: tax = subtotal * 0.22;
  $: total = subtotal + shipping + tax;
  $: isEmpty = items.length === 0;

  const STEPS = ['Spedizione', 'Pagamento', 'Conferma'];
  const STEP_INDEX: Record<typeof step, number> = { shipping: 0, payment: 1, review: 2 };

  async function placeOrder() {
    isSubmitting = true;
    submitError = '';
    try {
      const result = await createOrder($cartId, $userId);
      clearCart();
      window.location.href = `/orders/${result.orderId}`;
    } catch (err) {
      submitError = err instanceof Error ? err.message : 'Errore durante la creazione dell\'ordine. Riprova.';
      addToast(submitError, 'error');
    } finally {
      isSubmitting = false;
    }
  }

  function formatCard(value: string): string {
    // show only last 4
    const digits = value.replace(/\D/g, '');
    return '•••• •••• •••• ' + digits.slice(-4);
  }

  onMount(() => {
    // If cart is empty, redirect home
    if ($cartItems.length === 0) {
      window.location.href = '/cart';
    }
  });
</script>

<div class="reveal space-y-6">
  <div>
    <a href="/cart" class="text-sm text-slate-500 hover:text-slate-900 transition">← Carrello</a>
    <h1 class="font-title mt-1 text-4xl font-semibold text-slate-900">Checkout</h1>
  </div>

  <!-- Step indicator -->
  <nav class="flex items-center gap-2 text-sm font-medium" aria-label="Passaggi checkout">
    {#each STEPS as label, idx}
      <div class="flex items-center gap-2">
        <span
          class="flex h-7 w-7 items-center justify-center rounded-full text-xs font-bold transition"
          class:bg-slate-900={STEP_INDEX[step] >= idx}
          class:text-white={STEP_INDEX[step] >= idx}
          class:bg-slate-100={STEP_INDEX[step] < idx}
          class:text-slate-400={STEP_INDEX[step] < idx}
        >
          {STEP_INDEX[step] > idx ? '✓' : idx + 1}
        </span>
        <span class:text-slate-900={STEP_INDEX[step] === idx} class:text-slate-400={STEP_INDEX[step] !== idx}>
          {label}
        </span>
      </div>
      {#if idx < STEPS.length - 1}
        <div class="h-px flex-1 bg-slate-200 mx-1"></div>
      {/if}
    {/each}
  </nav>

  {#if isEmpty}
    <div class="rounded-2xl border border-slate-200 bg-white/70 p-8 text-center">
      <p class="text-slate-500">Il carrello è vuoto.</p>
      <a href="/" class="mt-3 inline-block text-sm font-semibold text-amber-700 underline">Torna al catalogo</a>
    </div>
  {:else}
    <div class="grid gap-6 lg:grid-cols-[minmax(0,1fr)_320px]">
      <!-- Form section -->
      <div>
        {#if step === 'shipping'}
          <form
            class="surface-glass rounded-2xl border border-white/70 p-6 shadow-md space-y-4"
            on:submit|preventDefault={() => (step = 'payment')}
          >
            <h2 class="font-title text-2xl font-semibold text-slate-900">Indirizzo di spedizione</h2>
            <p class="text-xs text-slate-400">I dati di spedizione sono richiesti solo a titolo dimostrativo.</p>

            <div class="grid gap-4 sm:grid-cols-2">
              <div>
                <label class="form-label">Nome</label>
                <input class="form-input" type="text" bind:value={firstName} required />
              </div>
              <div>
                <label class="form-label">Cognome</label>
                <input class="form-input" type="text" bind:value={lastName} required />
              </div>
              <div class="sm:col-span-2">
                <label class="form-label">Email</label>
                <input class="form-input" type="email" bind:value={email} required />
              </div>
              <div>
                <label class="form-label">Telefono</label>
                <input class="form-input" type="tel" bind:value={phone} />
              </div>
              <div class="sm:col-span-2">
                <label class="form-label">Indirizzo</label>
                <input class="form-input" type="text" bind:value={address} required />
              </div>
              <div>
                <label class="form-label">Città</label>
                <input class="form-input" type="text" bind:value={city} required />
              </div>
              <div>
                <label class="form-label">CAP</label>
                <input class="form-input" type="text" bind:value={zip} required />
              </div>
              <div class="sm:col-span-2">
                <label class="form-label">Paese</label>
                <input class="form-input" type="text" bind:value={country} required />
              </div>
            </div>

            <button
              type="submit"
              class="w-full rounded-xl bg-slate-900 py-3 text-sm font-semibold text-white transition hover:bg-amber-800"
            >
              Continua al pagamento →
            </button>
          </form>

        {:else if step === 'payment'}
          <form
            class="surface-glass rounded-2xl border border-white/70 p-6 shadow-md space-y-4"
            on:submit|preventDefault={() => (step = 'review')}
          >
            <h2 class="font-title text-2xl font-semibold text-slate-900">Metodo di pagamento</h2>
            <div class="flex items-center gap-2 rounded-xl bg-amber-50 border border-amber-200 px-4 py-3 text-xs text-amber-800">
              <span class="text-lg">💳</span>
              <span>Ambiente demo — nessun addebito reale verrà effettuato.</span>
            </div>

            <div class="space-y-4">
              <div>
                <label class="form-label">Intestatario carta</label>
                <input class="form-input" type="text" bind:value={cardName} required />
              </div>
              <div>
                <label class="form-label">Numero carta</label>
                <input
                  class="form-input font-mono"
                  type="text"
                  bind:value={cardNumber}
                  maxlength="19"
                  placeholder="1234 5678 9012 3456"
                  required
                />
              </div>
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="form-label">Scadenza (MM/AA)</label>
                  <input class="form-input font-mono" type="text" bind:value={cardExpiry} placeholder="MM/AA" maxlength="5" required />
                </div>
                <div>
                  <label class="form-label">CVC</label>
                  <input class="form-input font-mono" type="text" bind:value={cardCvc} placeholder="123" maxlength="4" required />
                </div>
              </div>
            </div>

            <div class="flex gap-3">
              <button
                type="button"
                on:click={() => (step = 'shipping')}
                class="flex-1 rounded-xl border border-slate-200 py-3 text-sm font-semibold text-slate-600 transition hover:bg-slate-50"
              >
                ← Indietro
              </button>
              <button
                type="submit"
                class="flex-1 rounded-xl bg-slate-900 py-3 text-sm font-semibold text-white transition hover:bg-amber-800"
              >
                Rivedi ordine →
              </button>
            </div>
          </form>

        {:else if step === 'review'}
          <div class="surface-glass rounded-2xl border border-white/70 p-6 shadow-md space-y-5">
            <h2 class="font-title text-2xl font-semibold text-slate-900">Riepilogo finale</h2>

            <!-- Shipping recap -->
            <div class="rounded-xl border border-slate-100 bg-white/60 p-4 text-sm space-y-1">
              <p class="font-semibold text-slate-700 mb-2">📦 Spedizione</p>
              <p class="text-slate-600">{firstName} {lastName}</p>
              <p class="text-slate-600">{address}, {zip} {city}, {country}</p>
              <p class="text-slate-500">{email} · {phone}</p>
            </div>

            <!-- Payment recap -->
            <div class="rounded-xl border border-slate-100 bg-white/60 p-4 text-sm space-y-1">
              <p class="font-semibold text-slate-700 mb-2">💳 Pagamento</p>
              <p class="text-slate-600">{cardName}</p>
              <p class="font-mono text-slate-600">{formatCard(cardNumber)}</p>
            </div>

            <!-- Items recap -->
            <div class="space-y-2">
              {#each items as item}
                <div class="flex items-center gap-3 rounded-xl border border-slate-100 bg-white/60 p-3 text-sm">
                  <img src={getProductImage(item.sku, 80, 60)} alt={item.name} class="h-12 w-16 rounded-lg object-cover" />
                  <div class="flex-1">
                    <p class="font-medium text-slate-800">{item.name}</p>
                    <p class="text-xs text-slate-400">{item.quantity} × {formatCurrency(item.unitPrice)}</p>
                  </div>
                  <p class="font-semibold text-slate-900">{formatCurrency(item.quantity * item.unitPrice)}</p>
                </div>
              {/each}
            </div>

            {#if submitError}
              <div class="rounded-xl border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">
                {submitError}
              </div>
            {/if}

            <div class="flex gap-3 pt-1">
              <button
                on:click={() => (step = 'payment')}
                class="flex-1 rounded-xl border border-slate-200 py-3 text-sm font-semibold text-slate-600 transition hover:bg-slate-50"
              >
                ← Indietro
              </button>
              <button
                on:click={placeOrder}
                disabled={isSubmitting}
                class="flex-1 rounded-xl bg-emerald-600 py-3 text-sm font-semibold text-white transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:bg-emerald-300"
              >
                {isSubmitting ? 'Invio ordine…' : '✓ Conferma ordine'}
              </button>
            </div>
          </div>
        {/if}
      </div>

      <!-- Order summary sidebar -->
      <aside class="lg:sticky lg:top-24">
        <div class="surface-glass rounded-2xl border border-white/70 p-5 shadow-xl text-sm">
          <h3 class="font-title text-xl font-semibold text-slate-900 mb-3">Il tuo ordine</h3>
          <ul class="space-y-2">
            {#each items as item}
              <li class="flex justify-between gap-2 text-slate-600">
                <span class="truncate">{item.name} ×{item.quantity}</span>
                <span class="shrink-0 font-medium">{formatCurrency(item.quantity * item.unitPrice)}</span>
              </li>
            {/each}
          </ul>
          <dl class="mt-4 space-y-1.5 border-t border-slate-200 pt-3 text-slate-600">
            <div class="flex justify-between">
              <dt>Subtotale</dt><dd>{formatCurrency(subtotal)}</dd>
            </div>
            <div class="flex justify-between">
              <dt>Spedizione</dt>
              <dd class:text-emerald-600={shipping === 0}>{shipping === 0 ? 'Gratis' : formatCurrency(shipping)}</dd>
            </div>
            <div class="flex justify-between">
              <dt>IVA (22%)</dt><dd>{formatCurrency(tax)}</dd>
            </div>
            <div class="flex justify-between border-t border-slate-300 pt-2 text-base font-bold text-slate-900">
              <dt>Totale</dt><dd>{formatCurrency(total)}</dd>
            </div>
          </dl>
        </div>
      </aside>
    </div>
  {/if}
</div>


