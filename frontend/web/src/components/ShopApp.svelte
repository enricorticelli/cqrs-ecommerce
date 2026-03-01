<script lang="ts">
  import { onMount } from 'svelte';
  import type { CartItem } from '../stores/cart';
  import { cartId, userId, cartItems, cartTotal } from '../stores/cart';

  const gatewayUrl = import.meta.env.PUBLIC_GATEWAY_URL ?? 'http://localhost:8080';

  type Product = {
    id: string;
    sku: string;
    name: string;
    description: string;
    price: number;
  };

  type SortMode = 'featured' | 'price-asc' | 'price-desc' | 'name';

  let products: Product[] = [];
  let searchTerm = '';
  let selectedCategory = 'all';
  let sortMode: SortMode = 'featured';

  let isLoadingProducts = true;
  let isUpdatingCart = false;
  let isCheckingOut = false;
  let error = '';

  let orderId = '';
  let orderStatus = '';

  let quantityByProduct: Record<string, number> = {};

  const categoryLabels: Record<string, string> = {
    ELEC: 'Electronics',
    HOME: 'Home',
    FASH: 'Fashion',
    SPORT: 'Sport',
    BEAUTY: 'Beauty'
  };

  $: categoryOptions = Array.from(new Set(products.map((product) => resolveCategory(product)))).sort((a, b) =>
    a.localeCompare(b)
  );

  $: filteredProducts = products
    .filter((product) => {
      const query = searchTerm.trim().toLowerCase();
      const matchesQuery =
        query.length === 0 ||
        product.name.toLowerCase().includes(query) ||
        product.description.toLowerCase().includes(query) ||
        product.sku.toLowerCase().includes(query);
      const matchesCategory = selectedCategory === 'all' || resolveCategory(product) === selectedCategory;
      return matchesQuery && matchesCategory;
    })
    .sort((a, b) => {
      if (sortMode === 'price-asc') {
        return a.price - b.price;
      }

      if (sortMode === 'price-desc') {
        return b.price - a.price;
      }

      if (sortMode === 'name') {
        return a.name.localeCompare(b.name);
      }

      return scoreProduct(b) - scoreProduct(a);
    });

  $: cartItemCount = $cartItems.reduce((count, item) => count + item.quantity, 0);
  $: subtotal = $cartTotal;
  $: shipping = subtotal > 0 ? (subtotal >= 120 ? 0 : 7.9) : 0;
  $: tax = subtotal * 0.22;
  $: grandTotal = subtotal + shipping + tax;

  function formatCurrency(value: number) {
    return new Intl.NumberFormat('it-IT', {
      style: 'currency',
      currency: 'EUR',
      maximumFractionDigits: 2
    }).format(value);
  }

  function resolveCategory(product: Product): string {
    const skuPrefix = product.sku.split('-')[0]?.toUpperCase() ?? '';
    if (skuPrefix.length > 0 && categoryLabels[skuPrefix]) {
      return categoryLabels[skuPrefix];
    }

    const name = `${product.name} ${product.description}`.toLowerCase();
    if (name.includes('shoe') || name.includes('shirt') || name.includes('jacket')) {
      return 'Fashion';
    }

    if (name.includes('kitchen') || name.includes('desk') || name.includes('lamp')) {
      return 'Home';
    }

    if (name.includes('headphone') || name.includes('laptop') || name.includes('watch')) {
      return 'Electronics';
    }

    return 'Lifestyle';
  }

  function scoreProduct(product: Product) {
    return stableHash(`${product.sku}|${product.name}`) % 100;
  }

  function stockLabel(product: Product): string {
    const bucket = stableHash(`stock:${product.id}`) % 3;
    if (bucket === 0) {
      return 'In stock';
    }

    if (bucket === 1) {
      return 'Only a few left';
    }

    return 'Ships in 48h';
  }

  function rating(product: Product): string {
    const value = 4.0 + (stableHash(`rating:${product.id}`) % 10) / 10;
    return value.toFixed(1);
  }

  function stableHash(value: string): number {
    let hash = 0;
    for (let index = 0; index < value.length; index += 1) {
      hash = (hash * 31 + value.charCodeAt(index)) >>> 0;
    }

    return hash;
  }

  function getQuantity(productId: string): number {
    return quantityByProduct[productId] ?? 1;
  }

  function setQuantity(productId: string, rawValue: string) {
    const parsed = Number.parseInt(rawValue, 10);
    const quantity = Number.isFinite(parsed) ? Math.min(10, Math.max(1, parsed)) : 1;
    quantityByProduct = { ...quantityByProduct, [productId]: quantity };
  }

  async function loadProducts() {
    isLoadingProducts = true;
    error = '';

    try {
      const response = await fetch(`${gatewayUrl}/api/catalog/v1/products`);
      if (!response.ok) {
        error = 'Non riesco a caricare il catalogo in questo momento.';
        return;
      }

      products = await response.json();
    } catch {
      error = 'Gateway non raggiungibile. Verifica che i servizi siano attivi.';
    } finally {
      isLoadingProducts = false;
    }
  }

  async function refreshCart() {
    try {
      const response = await fetch(`${gatewayUrl}/api/cart/v1/carts/${cartId.get()}`);
      if (response.status === 404) {
        cartItems.set([]);
        return;
      }

      if (!response.ok) {
        error = 'Non riesco ad aggiornare il carrello.';
        return;
      }

      const cart = await response.json();
      cartItems.set((cart.items ?? []) as CartItem[]);
    } catch {
      error = 'Errore di connessione durante il refresh del carrello.';
    }
  }

  async function addToCart(product: Product) {
    isUpdatingCart = true;
    error = '';

    try {
      const response = await fetch(`${gatewayUrl}/api/cart/v1/carts/${cartId.get()}/items`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          userId: userId.get(),
          productId: product.id,
          sku: product.sku,
          name: product.name,
          quantity: getQuantity(product.id),
          unitPrice: product.price
        })
      });

      if (!response.ok) {
        error = "Impossibile aggiungere l'articolo al carrello.";
        return;
      }

      await refreshCart();
    } catch {
      error = 'Errore di rete durante aggiunta al carrello.';
    } finally {
      isUpdatingCart = false;
    }
  }

  async function removeFromCart(item: CartItem) {
    isUpdatingCart = true;
    error = '';

    try {
      const response = await fetch(`${gatewayUrl}/api/cart/v1/carts/${cartId.get()}/items/${item.productId}`, {
        method: 'DELETE'
      });

      if (!response.ok) {
        error = 'Non riesco a rimuovere questo articolo adesso.';
        return;
      }

      await refreshCart();
    } catch {
      error = 'Errore di rete durante rimozione dal carrello.';
    } finally {
      isUpdatingCart = false;
    }
  }

  async function checkout() {
    isCheckingOut = true;
    error = '';
    orderStatus = '';

    try {
      const response = await fetch(`${gatewayUrl}/api/order/v1/orders`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ cartId: cartId.get(), userId: userId.get() })
      });

      if (!response.ok) {
        error = 'Checkout fallito. Verifica il contenuto del carrello e riprova.';
        return;
      }

      const body = await response.json();
      orderId = body.orderId;
      orderStatus = body.status;
      await pollOrder();
    } catch {
      error = 'Errore di rete durante il checkout.';
    } finally {
      isCheckingOut = false;
    }
  }

  async function pollOrder() {
    if (!orderId) {
      return;
    }

    for (let attempt = 0; attempt < 25; attempt += 1) {
      try {
        const response = await fetch(`${gatewayUrl}/api/order/v1/orders/${orderId}`);
        if (response.ok) {
          const order = await response.json();
          orderStatus = order.status;
          if (orderStatus === 'Completed' || orderStatus === 'Failed') {
            return;
          }
        }
      } catch {
        error = 'Monitoraggio ordine temporaneamente non disponibile.';
      }

      await new Promise((resolve) => setTimeout(resolve, 900));
    }
  }

  onMount(async () => {
    await Promise.all([loadProducts(), refreshCart()]);
  });
</script>

<section class="space-y-6 reveal">
  <header class="surface-glass overflow-hidden rounded-3xl border border-white/70 shadow-xl">
    <div class="grid gap-6 bg-gradient-to-r from-amber-100/80 via-orange-100/70 to-sky-100/80 p-6 md:grid-cols-5 md:p-8">
      <div class="md:col-span-3">
        <p class="text-sm font-semibold uppercase tracking-[0.28em] text-slate-600">Demo storefront</p>
        <h1 class="mt-3 text-4xl font-semibold leading-tight text-slate-900 md:text-5xl">Acquista in pochi click con un checkout event-driven</h1>
        <p class="mt-3 max-w-xl text-sm text-slate-700 md:text-base">
          Catalogo, carrello e ordine collegati ai microservizi CQRS tramite Gateway API. UI realistica con filtri, prezzi e stato ordine live.
        </p>
      </div>
      <div class="md:col-span-2">
        <div class="rounded-2xl border border-white/70 bg-white/75 p-4 text-sm shadow-md">
          <p class="font-semibold text-slate-700">Sessione cliente</p>
          <dl class="mt-3 space-y-2 text-xs text-slate-600">
            <div class="flex justify-between gap-3">
              <dt>Cart ID</dt>
              <dd class="max-w-[180px] truncate text-right font-mono">{$cartId}</dd>
            </div>
            <div class="flex justify-between gap-3">
              <dt>Utente</dt>
              <dd class="max-w-[180px] truncate text-right font-mono">{$userId}</dd>
            </div>
            <div class="flex justify-between gap-3">
              <dt>Prodotti</dt>
              <dd>{products.length}</dd>
            </div>
            <div class="flex justify-between gap-3">
              <dt>Articoli nel carrello</dt>
              <dd>{cartItemCount}</dd>
            </div>
          </dl>
        </div>
      </div>
    </div>
  </header>

  {#if error}
    <div class="pop-in rounded-xl border border-rose-300 bg-rose-50/90 p-3 text-sm text-rose-700">
      {error}
    </div>
  {/if}

  <div class="grid gap-6 lg:grid-cols-[minmax(0,1fr)_360px]">
    <section class="space-y-4">
      <div class="surface-glass rounded-2xl border border-white/70 p-4 shadow-sm">
        <div class="grid gap-3 md:grid-cols-3">
          <label class="md:col-span-2">
            <span class="mb-1 block text-xs font-semibold uppercase tracking-wide text-slate-600">Cerca prodotto</span>
            <input
              type="search"
              placeholder="Nome, SKU o descrizione"
              class="w-full rounded-xl border border-slate-300 bg-white px-3 py-2 text-sm outline-none ring-amber-300 transition focus:ring"
              bind:value={searchTerm}
            />
          </label>

          <label>
            <span class="mb-1 block text-xs font-semibold uppercase tracking-wide text-slate-600">Ordina per</span>
            <select class="w-full rounded-xl border border-slate-300 bg-white px-3 py-2 text-sm" bind:value={sortMode}>
              <option value="featured">Consigliati</option>
              <option value="price-asc">Prezzo crescente</option>
              <option value="price-desc">Prezzo decrescente</option>
              <option value="name">Nome</option>
            </select>
          </label>
        </div>

        <div class="mt-3 flex flex-wrap gap-2">
          <button
            class="rounded-full border px-3 py-1 text-xs font-semibold transition"
            class:bg-slate-900={selectedCategory === 'all'}
            class:text-white={selectedCategory === 'all'}
            class:border-slate-900={selectedCategory === 'all'}
            class:border-slate-300={selectedCategory !== 'all'}
            on:click={() => {
              selectedCategory = 'all';
            }}
          >
            Tutti
          </button>
          {#each categoryOptions as category}
            <button
              class="rounded-full border px-3 py-1 text-xs font-semibold transition"
              class:bg-slate-900={selectedCategory === category}
              class:text-white={selectedCategory === category}
              class:border-slate-900={selectedCategory === category}
              class:border-slate-300={selectedCategory !== category}
              on:click={() => {
                selectedCategory = category;
              }}
            >
              {category}
            </button>
          {/each}
        </div>
      </div>

      {#if isLoadingProducts}
        <div class="rounded-2xl border border-white/70 bg-white/70 p-6 text-sm text-slate-600 shadow-sm">Caricamento catalogo...</div>
      {:else if filteredProducts.length === 0}
        <div class="rounded-2xl border border-white/70 bg-white/70 p-6 text-sm text-slate-600 shadow-sm">
          Nessun prodotto trovato con i filtri selezionati.
        </div>
      {:else}
        <div class="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {#each filteredProducts as product, index}
            <article
              class="surface-glass pop-in rounded-2xl border border-white/70 p-4 shadow-md"
              style={`animation-delay: ${Math.min(index, 7) * 55}ms`}
            >
              <div class="flex items-start justify-between gap-3">
                <span class="rounded-full bg-slate-900 px-2 py-1 text-[11px] font-semibold text-white">{resolveCategory(product)}</span>
                <span class="text-xs text-amber-700">★ {rating(product)}</span>
              </div>

              <h2 class="mt-3 text-2xl font-semibold leading-snug text-slate-900">{product.name}</h2>
              <p class="mt-2 min-h-[54px] text-sm text-slate-600">{product.description || 'Descrizione non disponibile'}</p>

              <div class="mt-4 flex items-center justify-between text-sm">
                <span class="font-semibold text-emerald-700">{stockLabel(product)}</span>
                <span class="text-xs text-slate-500">SKU {product.sku}</span>
              </div>

              <div class="mt-4 flex items-end justify-between gap-3">
                <div>
                  <p class="text-xs text-slate-500">Prezzo</p>
                  <p class="text-2xl font-bold text-slate-900">{formatCurrency(product.price)}</p>
                </div>

                <div class="flex items-center gap-2">
                  <input
                    type="number"
                    min="1"
                    max="10"
                    class="w-16 rounded-lg border border-slate-300 bg-white px-2 py-1 text-center text-sm"
                    value={getQuantity(product.id)}
                    on:input={(event) => setQuantity(product.id, (event.currentTarget as HTMLInputElement).value)}
                  />
                  <button
                    class="rounded-xl bg-slate-900 px-3 py-2 text-sm font-semibold text-white transition hover:bg-slate-700 disabled:cursor-not-allowed disabled:bg-slate-400"
                    disabled={isUpdatingCart}
                    on:click={() => addToCart(product)}
                  >
                    Aggiungi
                  </button>
                </div>
              </div>
            </article>
          {/each}
        </div>
      {/if}
    </section>

    <aside class="lg:sticky lg:top-6">
      <section class="surface-glass rounded-2xl border border-white/70 p-5 shadow-xl">
        <div class="flex items-center justify-between">
          <h3 class="text-3xl font-semibold text-slate-900">Carrello</h3>
          <span class="rounded-full bg-slate-900 px-3 py-1 text-xs font-semibold text-white">{cartItemCount} articoli</span>
        </div>

        {#if $cartItems.length === 0}
          <p class="mt-4 text-sm text-slate-500">Il carrello e' vuoto. Aggiungi almeno un prodotto per procedere al checkout.</p>
        {:else}
          <ul class="mt-4 space-y-3">
            {#each $cartItems as item}
              <li class="rounded-xl border border-slate-200 bg-white/80 p-3 text-sm">
                <div class="flex items-start justify-between gap-2">
                  <div>
                    <p class="font-semibold text-slate-900">{item.name}</p>
                    <p class="text-xs text-slate-500">{item.quantity} x {formatCurrency(item.unitPrice)}</p>
                  </div>
                  <button class="text-xs font-semibold text-rose-600 hover:text-rose-700" on:click={() => removeFromCart(item)}>Rimuovi</button>
                </div>
                <p class="mt-2 text-right font-semibold text-slate-900">{formatCurrency(item.quantity * item.unitPrice)}</p>
              </li>
            {/each}
          </ul>
        {/if}

        <div class="mt-5 space-y-2 border-t border-slate-200 pt-4 text-sm">
          <div class="flex justify-between text-slate-600">
            <span>Subtotale</span>
            <span>{formatCurrency(subtotal)}</span>
          </div>
          <div class="flex justify-between text-slate-600">
            <span>Spedizione</span>
            <span>{shipping === 0 ? 'Gratis' : formatCurrency(shipping)}</span>
          </div>
          <div class="flex justify-between text-slate-600">
            <span>IVA stimata (22%)</span>
            <span>{formatCurrency(tax)}</span>
          </div>
          <div class="flex justify-between border-t border-slate-300 pt-3 text-base font-bold text-slate-900">
            <span>Totale</span>
            <span>{formatCurrency(grandTotal)}</span>
          </div>
        </div>

        <button
          class="mt-5 w-full rounded-xl bg-emerald-600 px-4 py-3 text-sm font-semibold text-white transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:bg-emerald-300"
          disabled={$cartItems.length === 0 || isCheckingOut}
          on:click={checkout}
        >
          {isCheckingOut ? 'Checkout in corso...' : 'Conferma ordine'}
        </button>

        {#if orderId}
          <div class="mt-4 rounded-xl border border-sky-300 bg-sky-50 p-3 text-sm text-sky-900">
            <p class="font-semibold">Ordine creato</p>
            <p class="mt-1 text-xs break-all">ID: {orderId}</p>
            <p class="mt-1">Stato: <strong>{orderStatus || 'Processing'}</strong></p>
          </div>
        {/if}
      </section>
    </aside>
  </div>
</section>
