<script lang="ts">
  import { onMount } from 'svelte';
  import { fetchCart, removeCartItem, type CartItemDto } from '../lib/api';
  import { getProductImage } from '../lib/mock';
  import { formatCurrency } from '../lib/format';
  import { cartId, userId, cartItems, cartTotal, syncCartFromServer, clearCart } from '../stores/cart';
  import { addToast } from '../stores/ui';

  let isLoading = true;
  let removingId: string | null = null;

  $: items = $cartItems;
  $: subtotal = $cartTotal;
  $: shipping = subtotal > 0 ? (subtotal >= 120 ? 0 : 7.9) : 0;
  $: tax = subtotal * 0.22;
  $: total = subtotal + shipping + tax;
  $: isEmpty = items.length === 0;

  async function syncCart() {
    isLoading = true;
    try {
      const cart = await fetchCart($cartId);
      if (cart) syncCartFromServer(cart.items);
      else syncCartFromServer([]);
    } catch {
      // use local state as fallback
    } finally {
      isLoading = false;
    }
  }

  async function removeItem(item: CartItemDto) {
    removingId = item.productId;
    try {
      await removeCartItem($cartId, item.productId);
      const cart = await fetchCart($cartId);
      if (cart) syncCartFromServer(cart.items);
      else syncCartFromServer([]);
      addToast(`${item.name} rimosso dal carrello`, 'info');
    } catch (err) {
      addToast('Impossibile rimuovere il prodotto', 'error');
    } finally {
      removingId = null;
    }
  }

  onMount(syncCart);
</script>

<div class="reveal space-y-6">
  <!-- Header -->
  <div class="flex items-center justify-between">
    <h1 class="font-title text-4xl font-semibold text-slate-900">Il tuo carrello</h1>
    {#if !isEmpty}
      <span
        class="rounded-full bg-slate-900 px-3 py-1 text-xs font-semibold text-white"
      >
        {items.reduce((n, i) => n + i.quantity, 0)} articoli
      </span>
    {/if}
  </div>

  {#if isLoading}
    <div class="space-y-3">
      {#each Array(3) as _}
        <div class="animate-pulse h-20 rounded-2xl bg-slate-200"></div>
      {/each}
    </div>
  {:else if isEmpty}
    <div class="rounded-3xl border border-slate-200 bg-white/70 p-14 text-center shadow-sm">
      <p class="text-5xl">🛒</p>
      <h2 class="mt-4 font-title text-2xl font-semibold text-slate-800">Carrello vuoto</h2>
      <p class="mt-2 text-sm text-slate-500">Aggiungi prodotti dal catalogo per procedere al checkout.</p>
      <a
        href="/"
        class="mt-5 inline-block rounded-xl bg-slate-900 px-6 py-3 text-sm font-semibold text-white transition hover:bg-amber-800"
      >
        Esplora il catalogo
      </a>
    </div>
  {:else}
    <div class="grid gap-6 lg:grid-cols-[minmax(0,1fr)_340px]">
      <!-- Item list -->
      <div class="space-y-3">
        {#each items as item}
          <div
            class="surface-glass flex gap-4 rounded-2xl border border-white/70 p-4 shadow-sm pop-in"
          >
            <img
              src={getProductImage(item.sku, 120, 90)}
              alt={item.name}
              class="h-20 w-28 shrink-0 rounded-xl object-cover"
              loading="lazy"
            />
            <div class="flex flex-1 flex-col gap-2">
              <div class="flex items-start justify-between gap-2">
                <div>
                  <a href="/product/{item.productId}" class="font-semibold text-slate-900 hover:text-amber-800 transition">
                    {item.name}
                  </a>
                  <p class="text-xs text-slate-400">SKU {item.sku}</p>
                </div>
                <button
                  on:click={() => removeItem(item)}
                  disabled={removingId === item.productId}
                  class="shrink-0 rounded-lg border border-rose-200 bg-rose-50 px-2.5 py-1 text-xs font-semibold text-rose-600 transition hover:bg-rose-100 disabled:opacity-40"
                >
                  {removingId === item.productId ? '…' : 'Rimuovi'}
                </button>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-slate-500">
                  {item.quantity} × {formatCurrency(item.unitPrice)}
                </span>
                <span class="font-bold text-slate-900">
                  {formatCurrency(item.quantity * item.unitPrice)}
                </span>
              </div>
            </div>
          </div>
        {/each}
      </div>

      <!-- Order summary -->
      <aside class="lg:sticky lg:top-24">
        <div class="surface-glass rounded-2xl border border-white/70 p-5 shadow-xl">
          <h3 class="font-title text-2xl font-semibold text-slate-900">Riepilogo ordine</h3>

          <dl class="mt-4 space-y-2 text-sm">
            <div class="flex justify-between text-slate-600">
              <dt>Subtotale</dt>
              <dd>{formatCurrency(subtotal)}</dd>
            </div>
            <div class="flex justify-between text-slate-600">
              <dt>Spedizione</dt>
              <dd class:text-emerald-600={shipping === 0}>
                {shipping === 0 ? 'Gratis 🎉' : formatCurrency(shipping)}
              </dd>
            </div>
            {#if shipping > 0}
              <p class="text-xs text-slate-400">Spedizione gratuita sopra {formatCurrency(120)}</p>
            {/if}
            <div class="flex justify-between text-slate-600">
              <dt>IVA (22%)</dt>
              <dd>{formatCurrency(tax)}</dd>
            </div>
            <div
              class="flex justify-between border-t border-slate-200 pt-3 text-base font-bold text-slate-900"
            >
              <dt>Totale</dt>
              <dd>{formatCurrency(total)}</dd>
            </div>
          </dl>

          <a
            href="/checkout"
            class="mt-5 block w-full rounded-xl bg-emerald-600 px-4 py-3 text-center text-sm font-semibold text-white transition hover:bg-emerald-700"
          >
            Procedi al checkout →
          </a>
          <a
            href="/"
            class="mt-3 block w-full rounded-xl border border-slate-200 bg-white/80 px-4 py-2.5 text-center text-sm font-semibold text-slate-600 transition hover:bg-slate-50"
          >
            ← Continua a fare shopping
          </a>
        </div>
      </aside>
    </div>
  {/if}
</div>
