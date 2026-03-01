<script lang="ts">
  import { onMount } from 'svelte';
  import { fetchProduct, addCartItem, fetchCart, type Product } from '../lib/api';
  import {
    getProductImage,
    getProductRating,
    getProductReviewCount,
    getProductStock,
    getProductCategory,
    STOCK_LABELS,
    STOCK_COLORS,
    getProductAccent,
    stableHash,
  } from '../lib/mock';
  import { formatCurrency } from '../lib/format';
  import { cartId, userId, syncCartFromServer } from '../stores/cart';
  import { addToast } from '../stores/ui';

  export let productId: string;

  let product: Product | null = null;
  let isLoading = true;
  let notFound = false;
  let loadError = '';
  let quantity = 1;
  let adding = false;
  let addedToCart = false;

  $: stock = product ? getProductStock(product.id) : 'in_stock';
  $: rating = product ? getProductRating(product.id) : 0;
  $: reviews = product ? getProductReviewCount(product.id) : 0;
  $: category = product ? getProductCategory(product) : '';
  $: accent = product ? getProductAccent(product.id) : 'bg-slate-100';

  /** Deterministic "related products" bullet points (mocked) */
  $: features = product
    ? generateFeatures(product)
    : [];

  function generateFeatures(p: Product): string[] {
    const base = [
      'Garanzia 24 mesi inclusa',
      'Reso gratuito entro 30 giorni',
      'Spedizione tracciata con corriere espresso',
    ];
    const extras = [
      'Imballaggio riciclato al 100%',
      'Prodotto certificato CE',
      'Disponibile in più varianti colore',
      'Premio qualità 2024',
      'Raccomandato dai nostri esperti',
    ];
    const seed = stableHash(`features:${p.id}`);
    return [...base, extras[seed % extras.length], extras[(seed + 2) % extras.length]];
  }

  function renderStars(r: number): string {
    return '★'.repeat(Math.floor(r)) + (r % 1 >= 0.5 ? '½' : '') + '☆'.repeat(5 - Math.ceil(r));
  }

  async function load() {
    isLoading = true;
    notFound = false;
    loadError = '';
    try {
      product = await fetchProduct(productId);
    } catch (err) {
      if (err instanceof Error && err.name === 'NotFoundError') {
        notFound = true;
      } else {
        loadError = 'Impossibile caricare il prodotto. Verifica la connessione.';
      }
    } finally {
      isLoading = false;
    }
  }

  async function addToCart() {
    if (!product) return;
    adding = true;
    addedToCart = false;
    try {
      await addCartItem($cartId, {
        userId: $userId,
        productId: product.id,
        sku: product.sku,
        name: product.name,
        quantity,
        unitPrice: product.price,
      });
      const cart = await fetchCart($cartId);
      if (cart) syncCartFromServer(cart.items);
      addToast(`${product.name} aggiunto al carrello`, 'success');
      addedToCart = true;
    } catch (err) {
      addToast(err instanceof Error ? err.message : 'Errore aggiunta al carrello', 'error');
    } finally {
      adding = false;
    }
  }

  onMount(load);
</script>

{#if isLoading}
  <!-- Skeleton -->
  <div class="reveal grid gap-8 md:grid-cols-2">
    <div class="animate-pulse rounded-3xl bg-slate-200 aspect-[4/3]"></div>
    <div class="space-y-4">
      <div class="h-4 w-24 animate-pulse rounded bg-slate-200"></div>
      <div class="h-8 w-3/4 animate-pulse rounded bg-slate-200"></div>
      <div class="h-4 w-full animate-pulse rounded bg-slate-200"></div>
      <div class="h-4 w-2/3 animate-pulse rounded bg-slate-200"></div>
    </div>
  </div>
{:else if notFound}
  <div class="reveal rounded-3xl border border-slate-200 bg-white/70 p-12 text-center">
    <p class="text-5xl">😶</p>
    <h2 class="mt-4 font-title text-2xl font-semibold text-slate-800">Prodotto non trovato</h2>
    <p class="mt-2 text-sm text-slate-500">L'articolo richiesto non esiste o è stato rimosso.</p>
    <a href="/" class="mt-5 inline-block rounded-xl bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-700 transition">
      ← Torna al catalogo
    </a>
  </div>
{:else if loadError}
  <div class="reveal rounded-2xl border border-rose-200 bg-rose-50 p-6 text-sm text-rose-700">
    {loadError}
  </div>
{:else if product}
  <div class="reveal space-y-8">
    <!-- Breadcrumb -->
    <nav class="text-sm text-slate-500 flex items-center gap-2">
      <a href="/" class="hover:text-slate-900 transition">Catalogo</a>
      <span>›</span>
      <span class="rounded-full {accent} px-2 py-0.5 text-xs font-semibold text-slate-700">{category}</span>
      <span>›</span>
      <span class="text-slate-700 font-medium truncate">{product.name}</span>
    </nav>

    <div class="grid gap-8 md:grid-cols-2">
      <!-- Image -->
      <div class="overflow-hidden rounded-3xl border border-white/70 shadow-lg">
        <img
          src={getProductImage(product.sku, 800, 600)}
          alt={product.name}
          class="aspect-[4/3] w-full object-cover"
        />
      </div>

      <!-- Details -->
      <div class="flex flex-col gap-5">
        <div>
          <span class="rounded-full {accent} px-3 py-1 text-xs font-semibold text-slate-700">
            {category}
          </span>
          <h1 class="font-title mt-3 text-4xl font-semibold leading-tight text-slate-900">
            {product.name}
          </h1>
          <p class="mt-1 text-sm text-slate-400">SKU: {product.sku}</p>
        </div>

        <!-- Rating -->
        <div class="flex items-center gap-2 text-sm">
          <span class="text-amber-500 text-base tracking-tight">{renderStars(rating)}</span>
          <span class="font-semibold text-slate-700">{rating}</span>
          <span class="text-slate-400">({reviews} recensioni)</span>
        </div>

        <!-- Description -->
        <p class="text-slate-600 leading-relaxed">
          {product.description || 'Nessuna descrizione disponibile per questo prodotto.'}
        </p>

        <!-- Features -->
        <ul class="space-y-1.5">
          {#each features as feat}
            <li class="flex items-center gap-2 text-sm text-slate-600">
              <span class="text-emerald-500 font-bold">✓</span>
              {feat}
            </li>
          {/each}
        </ul>

        <!-- Stock -->
        <p class="text-sm font-semibold {STOCK_COLORS[stock]}">
          {STOCK_LABELS[stock]}
        </p>

        <!-- Price + buy -->
        <div class="rounded-2xl border border-slate-200 bg-white/80 p-5 shadow-sm space-y-4">
          <div class="flex items-baseline gap-2">
            <span class="text-4xl font-bold text-slate-900">{formatCurrency(product.price)}</span>
            <span class="text-sm text-slate-400">IVA inclusa</span>
          </div>

          <div class="flex items-center gap-3">
            <label class="text-sm font-medium text-slate-700">Quantità</label>
            <div class="flex items-center">
              <button
                on:click={() => (quantity = Math.max(1, quantity - 1))}
                class="rounded-l-lg border border-r-0 border-slate-200 bg-white px-3 py-1.5 text-slate-600 hover:bg-slate-50 transition"
              >−</button>
              <input
                type="number"
                min="1"
                max="10"
                bind:value={quantity}
                class="w-14 border-y border-slate-200 bg-white px-1 py-1.5 text-center text-sm"
              />
              <button
                on:click={() => (quantity = Math.min(10, quantity + 1))}
                class="rounded-r-lg border border-l-0 border-slate-200 bg-white px-3 py-1.5 text-slate-600 hover:bg-slate-50 transition"
              >+</button>
            </div>
          </div>

          <div class="flex gap-3">
            <button
              on:click={addToCart}
              disabled={adding}
              class="flex-1 rounded-xl bg-slate-900 px-5 py-3 text-sm font-semibold text-white transition hover:bg-amber-800 disabled:cursor-not-allowed disabled:bg-slate-300"
            >
              {adding ? 'Aggiunta in corso…' : addedToCart ? '✓ Aggiunto!' : 'Aggiungi al carrello'}
            </button>
            <a
              href="/cart"
              class="rounded-xl border border-slate-300 bg-white px-5 py-3 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
            >
              Carrello →
            </a>
          </div>
        </div>
      </div>
    </div>

    <!-- Back link -->
    <a href="/" class="inline-flex items-center gap-1 text-sm text-slate-500 hover:text-slate-900 transition">
      ← Torna al catalogo
    </a>
  </div>
{/if}
