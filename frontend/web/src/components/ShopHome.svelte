<script lang="ts">
  import { onMount } from 'svelte';
  import { fetchProducts, addCartItem, fetchCart, type Product } from '../lib/api';
  import { getProductImage, getProductRating, getProductReviewCount, getProductStock, getProductCategory, STOCK_LABELS, STOCK_COLORS, getProductAccent, stableHash } from '../lib/mock';
  import { formatCurrency } from '../lib/format';
  import { cartId, userId, cartItems, syncCartFromServer } from '../stores/cart';
  import { addToast } from '../stores/ui';

  type SortMode = 'featured' | 'price-asc' | 'price-desc' | 'name';

  let products: Product[] = [];
  let isLoading = true;
  let loadError = '';
  let searchTerm = '';
  let selectedCategory = 'all';
  let sortMode: SortMode = 'featured';
  let addingProductId: string | null = null;
  let quantityByProduct: Record<string, number> = {};

  $: categories = Array.from(
    new Set(products.map((p) => getProductCategory(p)))
  ).sort((a, b) => a.localeCompare(b));

  $: filtered = products
    .filter((p) => {
      const q = searchTerm.trim().toLowerCase();
      const matchQuery =
        !q ||
        p.name.toLowerCase().includes(q) ||
        p.description.toLowerCase().includes(q) ||
        p.sku.toLowerCase().includes(q);
      const matchCat =
        selectedCategory === 'all' || getProductCategory(p) === selectedCategory;
      return matchQuery && matchCat;
    })
    .sort((a, b) => {
      if (sortMode === 'price-asc') return a.price - b.price;
      if (sortMode === 'price-desc') return b.price - a.price;
      if (sortMode === 'name') return a.name.localeCompare(b.name);
      // featured: stable deterministic sort
      return (stableHash(b.sku + b.name) % 100) - (stableHash(a.sku + a.name) % 100);
    });

  function getQty(id: string): number {
    return quantityByProduct[id] ?? 1;
  }

  function setQty(id: string, raw: string) {
    const n = Math.min(10, Math.max(1, parseInt(raw, 10) || 1));
    quantityByProduct = { ...quantityByProduct, [id]: n };
  }

  function renderStars(rating: number): string {
    const full = Math.floor(rating);
    const half = rating - full >= 0.5 ? 1 : 0;
    const empty = 5 - full - half;
    return '★'.repeat(full) + (half ? '½' : '') + '☆'.repeat(empty);
  }

  async function load() {
    isLoading = true;
    loadError = '';
    try {
      products = await fetchProducts();
    } catch (err) {
      loadError = 'Impossibile caricare il catalogo. Verifica che i servizi siano attivi.';
    } finally {
      isLoading = false;
    }
  }

  async function addToCart(product: Product) {
    addingProductId = product.id;
    try {
      await addCartItem($cartId, {
        userId: $userId,
        productId: product.id,
        sku: product.sku,
        name: product.name,
        quantity: getQty(product.id),
        unitPrice: product.price,
      });
      // sync cart
      const cart = await fetchCart($cartId);
      if (cart) syncCartFromServer(cart.items);
      addToast(`${product.name} aggiunto al carrello`, 'success');
    } catch (err) {
      addToast(err instanceof Error ? err.message : 'Errore aggiunta al carrello', 'error');
    } finally {
      addingProductId = null;
    }
  }

  onMount(load);
</script>

<!-- ─── Hero ──────────────────────────────────────────────────────────────── -->
<section
  class="reveal overflow-hidden rounded-3xl border border-white/70 bg-gradient-to-br from-amber-100/90 via-orange-50/80 to-sky-100/80 shadow-lg"
>
  <div class="grid gap-6 p-6 md:grid-cols-5 md:p-10">
    <div class="space-y-3 md:col-span-3">
      <span
        class="inline-block rounded-full bg-amber-700/10 px-3 py-1 text-xs font-semibold uppercase tracking-widest text-amber-800"
      >
        Demo storefront
      </span>
      <h1 class="font-title text-4xl font-semibold leading-tight text-slate-900 md:text-5xl">
        Acquista con un checkout<br />event-driven in tempo reale
      </h1>
      <p class="max-w-lg text-sm text-slate-600 md:text-base">
        Catalogo, carrello e ordini gestiti da microservizi CQRS. Ogni click invia comandi reali al
        backend — la saga di fulfillment gira in background mentre tu monitori il progresso.
      </p>
      <div class="flex flex-wrap gap-3 pt-2">
        <a
          href="#catalog"
          class="rounded-xl bg-slate-900 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-slate-700"
        >
          Esplora il catalogo
        </a>
        <a
          href="/cart"
          class="rounded-xl border border-slate-300 bg-white/80 px-5 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-white"
        >
          Vai al carrello →
        </a>
      </div>
    </div>
    <div class="md:col-span-2 flex items-center">
      <div class="w-full rounded-2xl border border-white/80 bg-white/70 p-5 shadow-md backdrop-blur-sm">
        <p class="text-xs font-semibold uppercase tracking-widest text-slate-500">
          Sessione corrente
        </p>
        <dl class="mt-3 space-y-2 text-sm">
          <div class="flex justify-between gap-2 text-slate-700">
            <dt class="text-slate-500">Prodotti disponibili</dt>
            <dd class="font-semibold">{products.length || '—'}</dd>
          </div>
          <div class="flex justify-between gap-2 text-slate-700">
            <dt class="text-slate-500">Articoli nel carrello</dt>
            <dd class="font-semibold">{$cartItems.reduce((n, i) => n + i.quantity, 0)}</dd>
          </div>
          <div class="flex justify-between gap-2 text-slate-700">
            <dt class="text-slate-500">Categorie</dt>
            <dd class="font-semibold">{categories.length || '—'}</dd>
          </div>
          <div class="mt-3 border-t border-slate-200 pt-3">
            <dt class="text-xs text-slate-500">User ID (demo fisso)</dt>
            <dd class="mt-0.5 truncate font-mono text-xs text-slate-600">{$userId}</dd>
          </div>
        </dl>
      </div>
    </div>
  </div>
</section>

<!-- ─── Catalog ───────────────────────────────────────────────────────────── -->
<section id="catalog" class="space-y-4">
  <!-- Filters bar -->
  <div class="surface-glass rounded-2xl border border-white/70 p-4 shadow-sm">
    <div class="grid gap-3 sm:grid-cols-3">
      <label class="sm:col-span-2">
        <span class="mb-1 block text-xs font-semibold uppercase tracking-wide text-slate-500"
          >Cerca prodotto</span
        >
        <input
          type="search"
          placeholder="Nome, SKU o descrizione…"
          bind:value={searchTerm}
          class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm outline-none ring-amber-300 transition focus:border-amber-400 focus:ring"
        />
      </label>
      <label>
        <span class="mb-1 block text-xs font-semibold uppercase tracking-wide text-slate-500"
          >Ordina per</span
        >
        <select
          bind:value={sortMode}
          class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm"
        >
          <option value="featured">Consigliati</option>
          <option value="price-asc">Prezzo crescente</option>
          <option value="price-desc">Prezzo decrescente</option>
          <option value="name">Nome</option>
        </select>
      </label>
    </div>

    <!-- Category pills -->
    <div class="mt-3 flex flex-wrap gap-2">
      {#each ['all', ...categories] as cat}
        <button
          on:click={() => (selectedCategory = cat)}
          class="rounded-full border px-3 py-1 text-xs font-semibold transition"
          class:bg-slate-900={selectedCategory === cat}
          class:text-white={selectedCategory === cat}
          class:border-slate-900={selectedCategory === cat}
          class:border-slate-200={selectedCategory !== cat}
          class:bg-white={selectedCategory !== cat}
          class:text-slate-600={selectedCategory !== cat}
        >
          {cat === 'all' ? 'Tutti' : cat}
        </button>
      {/each}
    </div>

    {#if !isLoading}
      <p class="mt-2 text-xs text-slate-400">
        {filtered.length} di {products.length} prodotti
      </p>
    {/if}
  </div>

  <!-- States -->
  {#if isLoading}
    <div class="col-span-full grid grid-cols-2 gap-4 md:grid-cols-3 xl:grid-cols-4">
      {#each Array(8) as _}
        <div class="animate-pulse rounded-2xl bg-slate-200 h-72"></div>
      {/each}
    </div>
  {:else if loadError}
    <div
      class="rounded-2xl border border-rose-200 bg-rose-50 p-6 text-sm font-medium text-rose-700"
    >
      {loadError}
      <button on:click={load} class="ml-3 underline hover:no-underline">Riprova</button>
    </div>
  {:else if filtered.length === 0}
    <div class="rounded-2xl border border-slate-200 bg-white/70 p-10 text-center text-slate-500">
      <p class="text-4xl">🔍</p>
      <p class="mt-3 text-sm font-medium">Nessun prodotto trovato con i filtri selezionati.</p>
      <button
        on:click={() => { searchTerm = ''; selectedCategory = 'all'; }}
        class="mt-3 text-sm underline text-amber-700 hover:no-underline"
      >
        Azzera filtri
      </button>
    </div>
  {:else}
    <!-- Product grid -->
    <div class="grid gap-4 sm:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4">
      {#each filtered as product, i}
        {@const stock = getProductStock(product.id)}
        {@const rating = getProductRating(product.id)}
        {@const reviews = getProductReviewCount(product.id)}
        {@const category = getProductCategory(product)}
        {@const accent = getProductAccent(product.id)}
        <article
          class="surface-glass pop-in flex flex-col overflow-hidden rounded-2xl border border-white/70 shadow-md transition hover:shadow-lg hover:-translate-y-0.5"
          style="animation-delay: {Math.min(i, 9) * 45}ms"
        >
          <!-- Product image -->
          <a href="/product/{product.id}" class="block overflow-hidden">
            <img
              src={getProductImage(product.sku, 600, 400)}
              alt={product.name}
              class="aspect-[3/2] w-full object-cover transition duration-300 hover:scale-105"
              loading="lazy"
            />
          </a>

          <div class="flex flex-1 flex-col gap-3 p-4">
            <!-- Category + rating row -->
            <div class="flex items-center justify-between gap-2">
              <span
                class="rounded-full {accent} px-2.5 py-0.5 text-[11px] font-semibold text-slate-700"
              >
                {category}
              </span>
              <span class="text-xs text-amber-600" title="{rating}/5 su {reviews} recensioni">
                ★ {rating} <span class="text-slate-400">({reviews})</span>
              </span>
            </div>

            <!-- Name + description -->
            <div class="flex-1">
              <a href="/product/{product.id}">
                <h2 class="font-title text-xl font-semibold leading-snug text-slate-900 hover:text-amber-800 transition">
                  {product.name}
                </h2>
              </a>
              <p class="mt-1 line-clamp-2 text-sm text-slate-500">
                {product.description || 'Descrizione non disponibile.'}
              </p>
            </div>

            <!-- Stock + SKU -->
            <div class="flex items-center justify-between text-xs">
              <span class="font-semibold {STOCK_COLORS[stock]}">{STOCK_LABELS[stock]}</span>
              <span class="text-slate-400">SKU {product.sku}</span>
            </div>

            <!-- Price + add to cart -->
            <div class="flex items-end justify-between gap-2 border-t border-slate-100 pt-3">
              <div>
                <p class="text-xs text-slate-400">Prezzo</p>
                <p class="text-2xl font-bold text-slate-900">{formatCurrency(product.price)}</p>
              </div>
              <div class="flex items-center gap-1.5">
                <input
                  type="number"
                  min="1"
                  max="10"
                  value={getQty(product.id)}
                  on:input={(e) => setQty(product.id, (e.currentTarget).value)}
                  class="w-14 rounded-lg border border-slate-200 bg-white px-2 py-1.5 text-center text-sm"
                  aria-label="Quantità"
                />
                <button
                  on:click={() => addToCart(product)}
                  disabled={addingProductId === product.id}
                  class="rounded-xl bg-slate-900 px-3 py-2 text-sm font-semibold text-white transition hover:bg-amber-800 disabled:cursor-not-allowed disabled:bg-slate-300"
                >
                  {addingProductId === product.id ? '…' : '+ Carrello'}
                </button>
              </div>
            </div>
          </div>
        </article>
      {/each}
    </div>
  {/if}
</section>
