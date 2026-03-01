<script lang="ts">
  import { cartCount } from '../stores/cart';
</script>

<header
  class="sticky top-0 z-40 border-b border-white/60 bg-cream/80 shadow-sm backdrop-blur-md"
>
  <div class="mx-auto flex max-w-7xl items-center justify-between gap-4 px-4 py-3 md:px-8">
    <!-- Brand -->
    <a href="/" class="flex items-center gap-2 group" aria-label="CQRS Commerce – Home">
      <span class="flex h-8 w-8 items-center justify-center rounded-full bg-slate-900 text-white text-xs font-bold tracking-tight group-hover:bg-amber-700 transition">CC</span>
      <span class="font-title text-lg font-semibold tracking-tight text-slate-900 hidden sm:inline">CQRS<span class="text-amber-700">Commerce</span></span>
    </a>

    <!-- Nav links -->
    <nav class="hidden md:flex items-center gap-6 text-sm font-medium text-slate-700">
      <a href="/" class="hover:text-slate-900 transition">Catalogo</a>
    </nav>

    <!-- Cart button -->
    <a
      href="/cart"
      class="relative flex items-center gap-2 rounded-xl bg-slate-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-slate-700"
      aria-label="Carrello"
    >
      <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
        <path stroke-linecap="round" stroke-linejoin="round" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13l-1.5 6h13M7 13L5.4 5M9 21a1 1 0 100-2 1 1 0 000 2zm10 0a1 1 0 100-2 1 1 0 000 2z" />
      </svg>
      <span class="hidden sm:inline">Carrello</span>
      {#if $cartCount > 0}
        <span
          class="absolute -right-2 -top-2 flex h-5 w-5 items-center justify-center rounded-full bg-amber-500 text-[10px] font-bold text-white"
        >
          {$cartCount > 99 ? '99+' : $cartCount}
        </span>
      {/if}
    </a>
  </div>
</header>
