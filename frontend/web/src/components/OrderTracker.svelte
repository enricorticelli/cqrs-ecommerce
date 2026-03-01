<script lang="ts">
  import { onMount, onDestroy } from 'svelte';
  import { fetchOrder, type OrderView } from '../lib/api';
  import { getProductImage } from '../lib/mock';
  import { formatCurrency } from '../lib/format';

  export let orderId: string;

  let order: OrderView | null = null;
  let isLoading = true;
  let notFound = false;
  let loadError = '';
  let pollingActive = true;
  let pollAttempts = 0;
  const MAX_POLL = 50;

  // ─── Status config ────────────────────────────────────────────────────────

  type StatusInfo = {
    label: string;
    color: string;
    bg: string;
    icon: string;
    description: string;
  };

  const STATUS_MAP: Record<string, StatusInfo> = {
    Pending: {
      label: 'In attesa',
      color: 'text-amber-700',
      bg: 'bg-amber-50 border-amber-200',
      icon: '⏳',
      description: 'Il tuo ordine è stato ricevuto e verrà elaborato a breve.',
    },
    Processing: {
      label: 'Elaborazione',
      color: 'text-sky-700',
      bg: 'bg-sky-50 border-sky-200',
      icon: '⚙️',
      description: 'Stiamo verificando la disponibilità e processando il pagamento.',
    },
    Completed: {
      label: 'Completato',
      color: 'text-emerald-700',
      bg: 'bg-emerald-50 border-emerald-200',
      icon: '✅',
      description: 'Ordine confermato! La spedizione è in preparazione.',
    },
    Failed: {
      label: 'Non elaborato',
      color: 'text-rose-700',
      bg: 'bg-rose-50 border-rose-200',
      icon: '❌',
      description: 'L\'ordine non ha potuto essere completato.',
    },
  };

  const PIPELINE_STEPS = [
    { key: 'received', label: 'Ricevuto', description: 'Ordine creato nel sistema' },
    { key: 'stock', label: 'Magazzino', description: 'Riserva scorte verificata' },
    { key: 'payment', label: 'Pagamento', description: 'Transazione autorizzata' },
    { key: 'shipping', label: 'Spedizione', description: 'Etichetta di spedizione creata' },
  ];

  $: statusInfo = order
    ? (STATUS_MAP[order.status] ?? {
        label: order.status,
        color: 'text-slate-700',
        bg: 'bg-slate-50 border-slate-200',
        icon: '📋',
        description: 'Stato in aggiornamento…',
      })
    : null;

  $: isDone = order?.status === 'Completed' || order?.status === 'Failed';

  $: pipelineStep = order
    ? order.status === 'Completed'
      ? 4
      : order.status === 'Failed'
      ? -1
      : order.status === 'Processing'
      ? 1
      : 0
    : 0;

  // ─── Polling ──────────────────────────────────────────────────────────────

  async function poll() {
    while (pollingActive && pollAttempts < MAX_POLL) {
      try {
        order = await fetchOrder(orderId);
        if (order.status === 'Completed' || order.status === 'Failed') {
          pollingActive = false;
          break;
        }
      } catch (err) {
        if (err instanceof Error && err.name === 'NotFoundError') {
          notFound = true;
          pollingActive = false;
          break;
        }
        // transient — keep polling
      }
      pollAttempts += 1;
      await new Promise((r) => setTimeout(r, 1200));
    }
  }

  onMount(async () => {
    isLoading = true;
    try {
      order = await fetchOrder(orderId);
      isLoading = false;
      if (!isDone) await poll();
    } catch (err) {
      if (err instanceof Error && err.name === 'NotFoundError') {
        notFound = true;
      } else {
        loadError = 'Impossibile recuperare i dati dell\'ordine.';
      }
      isLoading = false;
    }
  });

  onDestroy(() => {
    pollingActive = false;
  });
</script>

<div class="reveal space-y-6">
  <div>
    <a href="/" class="text-sm text-slate-500 hover:text-slate-900 transition">← Catalogo</a>
    <h1 class="font-title mt-1 text-4xl font-semibold text-slate-900">Stato ordine</h1>
  </div>

  {#if isLoading}
    <div class="space-y-4">
      <div class="animate-pulse h-28 rounded-2xl bg-slate-200"></div>
      <div class="animate-pulse h-40 rounded-2xl bg-slate-200"></div>
    </div>

  {:else if notFound}
    <div class="rounded-3xl border border-slate-200 bg-white/70 p-12 text-center">
      <p class="text-5xl">🔍</p>
      <h2 class="mt-4 font-title text-2xl font-semibold">Ordine non trovato</h2>
      <p class="mt-2 text-sm text-slate-500">L'ID ordine non corrisponde a nessun record esistente.</p>
      <a href="/" class="mt-5 inline-block rounded-xl bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-amber-800 transition">
        Torna al catalogo
      </a>
    </div>

  {:else if loadError}
    <div class="rounded-2xl border border-rose-200 bg-rose-50 p-5 text-sm text-rose-700">
      {loadError}
    </div>

  {:else if order}
    <!-- Status banner -->
    {#if statusInfo}
      <div class="flex items-center gap-4 rounded-2xl border p-5 {statusInfo.bg}">
        <span class="text-4xl">{statusInfo.icon}</span>
        <div>
          <p class="font-semibold {statusInfo.color} text-lg">{statusInfo.label}</p>
          <p class="text-sm text-slate-600">{statusInfo.description}</p>
          {#if order.failureReason}
            <p class="mt-1 text-xs text-rose-600">Motivo: {order.failureReason}</p>
          {/if}
        </div>
        {#if !isDone}
          <span class="ml-auto flex items-center gap-1 text-xs text-slate-500">
            <span class="inline-block h-2 w-2 animate-ping rounded-full bg-sky-400"></span>
            Aggiornamento live
          </span>
        {/if}
      </div>
    {/if}

    <!-- Pipeline progress -->
    <div class="surface-glass rounded-2xl border border-white/70 p-5 shadow-sm">
      <h2 class="text-sm font-semibold uppercase tracking-wider text-slate-500 mb-4">Pipeline di fulfillment</h2>
      <div class="flex items-start gap-0">
        {#each PIPELINE_STEPS as step, idx}
          {@const done = pipelineStep > idx || (order.status === 'Completed')}
          {@const active = pipelineStep === idx && !isDone}
          {@const failed = order.status === 'Failed' && idx > 0 && !done}
          <div class="flex flex-1 flex-col items-center text-center gap-1">
            <div class="relative flex items-center w-full">
              {#if idx > 0}
                <div
                  class="h-0.5 flex-1 transition-colors duration-500"
                  class:bg-emerald-400={pipelineStep >= idx}
                  class:bg-slate-200={pipelineStep < idx}
                ></div>
              {:else}
                <div class="flex-1"></div>
              {/if}
              <div
                class="relative z-10 flex h-9 w-9 shrink-0 items-center justify-center rounded-full border-2 text-sm font-bold transition-all duration-500"
                class:bg-emerald-500={done && !failed}
                class:border-emerald-500={done && !failed}
                class:bg-sky-500={active}
                class:border-sky-500={active}
                class:text-white={(done && !failed) || active}
                class:bg-white={!done && !active}
                class:border-slate-200={!done && !active}
                class:text-slate-400={!done && !active}
                class:animate-pulse={active}
              >
                {#if done && !failed}✓
                {:else if active}…
                {:else}{idx + 1}{/if}
              </div>
              {#if idx < PIPELINE_STEPS.length - 1}
                <div
                  class="h-0.5 flex-1 transition-colors duration-500"
                  class:bg-emerald-400={pipelineStep > idx}
                  class:bg-slate-200={pipelineStep <= idx}
                ></div>
              {:else}
                <div class="flex-1"></div>
              {/if}
            </div>
            <p class="text-xs font-semibold text-slate-700 mt-1">{step.label}</p>
            <p class="text-[10px] text-slate-400 leading-tight px-1">{step.description}</p>
          </div>
        {/each}
      </div>
    </div>

    <!-- Order details grid -->
    <div class="grid gap-4 sm:grid-cols-2">
      <!-- Order info -->
      <div class="surface-glass rounded-2xl border border-white/70 p-5 shadow-sm text-sm space-y-3">
        <h3 class="font-semibold text-slate-700 uppercase tracking-wide text-xs">Dettagli ordine</h3>
        <dl class="space-y-2 text-sm">
          <div class="flex justify-between gap-2">
            <dt class="text-slate-500">Order ID</dt>
            <dd class="font-mono text-xs text-slate-700 truncate max-w-[180px]">{order.id}</dd>
          </div>
          <div class="flex justify-between gap-2">
            <dt class="text-slate-500">Importo totale</dt>
            <dd class="font-bold text-slate-900">{formatCurrency(order.totalAmount)}</dd>
          </div>
          {#if order.transactionId}
            <div class="flex justify-between gap-2">
              <dt class="text-slate-500">Transaction ID</dt>
              <dd class="font-mono text-xs text-slate-700 truncate max-w-[180px]">{order.transactionId}</dd>
            </div>
          {/if}
          {#if order.trackingCode}
            <div class="flex justify-between gap-2">
              <dt class="text-slate-500">Tracking</dt>
              <dd class="font-mono text-xs font-semibold text-emerald-700">{order.trackingCode}</dd>
            </div>
          {/if}
        </dl>
      </div>

      <!-- Shipping info (mocked) -->
      <div class="surface-glass rounded-2xl border border-white/70 p-5 shadow-sm text-sm space-y-3">
        <h3 class="font-semibold text-slate-700 uppercase tracking-wide text-xs">Spedizione stimata</h3>
        <p class="text-slate-500 text-xs">* Dati stimati per la demo</p>
        <ul class="space-y-1.5 text-slate-600">
          <li class="flex items-center gap-2"><span class="text-emerald-500">✓</span>Corriere espresso tracciato</li>
          <li class="flex items-center gap-2"><span class="text-emerald-500">✓</span>Consegna stimata in 2-3 giorni lavorativi</li>
          <li class="flex items-center gap-2"><span class="text-sky-500">→</span>Notifica email all'avvenuta spedizione</li>
        </ul>
        {#if order.trackingCode}
          <div class="rounded-lg bg-emerald-50 border border-emerald-200 px-3 py-2 text-xs text-emerald-800 font-mono">
            Tracking: {order.trackingCode}
          </div>
        {/if}
      </div>
    </div>

    <!-- Items -->
    {#if order.items && order.items.length > 0}
      <div class="surface-glass rounded-2xl border border-white/70 p-5 shadow-sm">
        <h3 class="font-semibold text-slate-700 uppercase tracking-wide text-xs mb-3">Articoli ordinati</h3>
        <div class="space-y-3">
          {#each order.items as item}
            <div class="flex items-center gap-4 rounded-xl border border-slate-100 bg-white/60 p-3">
              <img
                src={getProductImage(item.sku, 80, 60)}
                alt={item.name}
                class="h-12 w-16 rounded-lg object-cover"
                loading="lazy"
              />
              <div class="flex-1">
                <p class="font-medium text-slate-800">{item.name}</p>
                <p class="text-xs text-slate-400">SKU {item.sku} · Qty {item.quantity}</p>
              </div>
              <p class="font-semibold text-slate-900">{formatCurrency(item.quantity * item.unitPrice)}</p>
            </div>
          {/each}
        </div>
      </div>
    {/if}

    <!-- Actions -->
    <div class="flex flex-wrap gap-3">
      <a href="/" class="rounded-xl bg-slate-900 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-amber-800">
        Continua a fare shopping
      </a>
      {#if isDone}
        <a href="/" class="rounded-xl border border-slate-200 bg-white/80 px-5 py-2.5 text-sm font-semibold text-slate-600 transition hover:bg-slate-50">
          ← Torna al catalogo
        </a>
      {/if}
    </div>
  {/if}
</div>
