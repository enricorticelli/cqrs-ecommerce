<script lang="ts">
  import { onMount } from 'svelte';
  import {
    fetchBrands,
    createBrand,
    updateBrand,
    deleteBrand,
    fetchCategories,
    createCategory,
    updateCategory,
    deleteCategory,
    fetchCollections,
    createCollection,
    updateCollection,
    deleteCollection,
    fetchProducts,
    createProduct,
    updateProduct,
    deleteProduct,
    type Brand,
    type Category,
    type Collection,
    type Product,
  } from '../lib/api';
  import { formatCurrency } from '../lib/format';
  import { addToast } from '../stores/ui';

  let isLoading = true;
  let isSaving = false;

  let brands: Brand[] = [];
  let categories: Category[] = [];
  let collections: Collection[] = [];
  let products: Product[] = [];

  let editingBrandId: string | null = null;
  let brandForm = { name: '', slug: '', description: '' };

  let editingCategoryId: string | null = null;
  let categoryForm = { name: '', slug: '', description: '' };

  let editingCollectionId: string | null = null;
  let collectionForm = { name: '', slug: '', description: '', isFeatured: false };

  let editingProductId: string | null = null;
  let productForm = {
    sku: '',
    name: '',
    description: '',
    price: 0,
    brandId: '',
    categoryId: '',
    collectionIds: [] as string[],
    isNewArrival: false,
    isBestSeller: false,
  };

  async function loadAll() {
    isLoading = true;
    try {
      const [brandData, categoryData, collectionData, productData] = await Promise.all([
        fetchBrands(),
        fetchCategories(),
        fetchCollections(),
        fetchProducts(),
      ]);

      brands = brandData;
      categories = categoryData;
      collections = collectionData;
      products = productData;
    } catch (err) {
      addToast(err instanceof Error ? err.message : 'Errore caricamento catalogo', 'error');
    } finally {
      isLoading = false;
    }
  }

  function resetBrandForm() {
    editingBrandId = null;
    brandForm = { name: '', slug: '', description: '' };
  }

  function resetCategoryForm() {
    editingCategoryId = null;
    categoryForm = { name: '', slug: '', description: '' };
  }

  function resetCollectionForm() {
    editingCollectionId = null;
    collectionForm = { name: '', slug: '', description: '', isFeatured: false };
  }

  function resetProductForm() {
    editingProductId = null;
    productForm = {
      sku: '',
      name: '',
      description: '',
      price: 0,
      brandId: brands[0]?.id ?? '',
      categoryId: categories[0]?.id ?? '',
      collectionIds: [],
      isNewArrival: false,
      isBestSeller: false,
    };
  }

  function startEditBrand(brand: Brand) {
    editingBrandId = brand.id;
    brandForm = { name: brand.name, slug: brand.slug, description: brand.description };
  }

  function startEditCategory(category: Category) {
    editingCategoryId = category.id;
    categoryForm = { name: category.name, slug: category.slug, description: category.description };
  }

  function startEditCollection(collection: Collection) {
    editingCollectionId = collection.id;
    collectionForm = {
      name: collection.name,
      slug: collection.slug,
      description: collection.description,
      isFeatured: collection.isFeatured,
    };
  }

  function startEditProduct(product: Product) {
    editingProductId = product.id;
    productForm = {
      sku: product.sku,
      name: product.name,
      description: product.description,
      price: product.price,
      brandId: product.brandId,
      categoryId: product.categoryId,
      collectionIds: [...product.collectionIds],
      isNewArrival: product.isNewArrival,
      isBestSeller: product.isBestSeller,
    };
  }

  async function saveBrand() {
    isSaving = true;
    try {
      if (editingBrandId) {
        await updateBrand(editingBrandId, brandForm);
        addToast('Brand aggiornato', 'success');
      } else {
        await createBrand(brandForm);
        addToast('Brand creato', 'success');
      }
      resetBrandForm();
      brands = await fetchBrands();
    } catch (err) {
      addToast(err instanceof Error ? err.message : 'Errore salvataggio brand', 'error');
    } finally {
      isSaving = false;
    }
  }

  async function saveCategory() {
    isSaving = true;
    try {
      if (editingCategoryId) {
        await updateCategory(editingCategoryId, categoryForm);
        addToast('Categoria aggiornata', 'success');
      } else {
        await createCategory(categoryForm);
        addToast('Categoria creata', 'success');
      }
      resetCategoryForm();
      categories = await fetchCategories();
    } catch (err) {
      addToast(err instanceof Error ? err.message : 'Errore salvataggio categoria', 'error');
    } finally {
      isSaving = false;
    }
  }

  async function saveCollection() {
    isSaving = true;
    try {
      if (editingCollectionId) {
        await updateCollection(editingCollectionId, collectionForm);
        addToast('Collezione aggiornata', 'success');
      } else {
        await createCollection(collectionForm);
        addToast('Collezione creata', 'success');
      }
      resetCollectionForm();
      collections = await fetchCollections();
    } catch (err) {
      addToast(err instanceof Error ? err.message : 'Errore salvataggio collezione', 'error');
    } finally {
      isSaving = false;
    }
  }

  async function saveProduct() {
    isSaving = true;
    try {
      if (!productForm.brandId || !productForm.categoryId) {
        throw new Error('Brand e categoria sono obbligatori');
      }

      if (editingProductId) {
        await updateProduct(editingProductId, productForm);
        addToast('Prodotto aggiornato', 'success');
      } else {
        await createProduct(productForm);
        addToast('Prodotto creato', 'success');
      }

      resetProductForm();
      products = await fetchProducts();
    } catch (err) {
      addToast(err instanceof Error ? err.message : 'Errore salvataggio prodotto', 'error');
    } finally {
      isSaving = false;
    }
  }

  async function removeBrand(id: string) {
    if (!confirm('Eliminare brand?')) return;
    await deleteBrand(id);
    brands = await fetchBrands();
    addToast('Brand eliminato', 'info');
  }

  async function removeCategory(id: string) {
    if (!confirm('Eliminare categoria?')) return;
    await deleteCategory(id);
    categories = await fetchCategories();
    addToast('Categoria eliminata', 'info');
  }

  async function removeCollection(id: string) {
    if (!confirm('Eliminare collezione?')) return;
    await deleteCollection(id);
    collections = await fetchCollections();
    addToast('Collezione eliminata', 'info');
  }

  async function removeProduct(id: string) {
    if (!confirm('Eliminare prodotto?')) return;
    await deleteProduct(id);
    products = await fetchProducts();
    addToast('Prodotto eliminato', 'info');
  }

  onMount(async () => {
    await loadAll();
    resetProductForm();
  });
</script>

<div class="space-y-6 reveal">
  <div>
    <p class="text-xs font-semibold uppercase tracking-[0.2em] text-[#008060]">Catalog admin</p>
    <h1 class="mt-2 font-title text-4xl font-extrabold text-[#202223]">Gestione catalogo</h1>
    <p class="mt-2 text-sm text-[#616161]">CRUD completo di brand, categorie, collezioni e prodotti.</p>
  </div>

  {#if isLoading}
    <div class="surface-card h-56 animate-pulse"></div>
  {:else}
    <section class="grid gap-6 xl:grid-cols-2">
      <div class="surface-card p-5">
        <h2 class="font-title text-2xl font-bold text-[#202223]">Brand</h2>
        <div class="mt-3 grid gap-2">
          <input class="form-input" bind:value={brandForm.name} placeholder="Nome" />
          <input class="form-input" bind:value={brandForm.slug} placeholder="Slug" />
          <textarea class="form-input" bind:value={brandForm.description} rows="2" placeholder="Descrizione"></textarea>
          <div class="flex gap-2">
            <button class="btn-primary" on:click={saveBrand} disabled={isSaving}>{editingBrandId ? 'Aggiorna' : 'Crea'}</button>
            {#if editingBrandId}<button class="btn-secondary" on:click={resetBrandForm}>Annulla</button>{/if}
          </div>
        </div>
        <ul class="mt-4 space-y-2 text-sm">
          {#each brands as brand}
            <li class="surface-muted flex items-center justify-between p-3">
              <div><p class="font-semibold text-[#202223]">{brand.name}</p><p class="text-xs text-[#6d7175]">{brand.slug}</p></div>
              <div class="flex gap-2"><button class="btn-secondary" on:click={() => startEditBrand(brand)}>Modifica</button><button class="btn-secondary" on:click={() => removeBrand(brand.id)}>Elimina</button></div>
            </li>
          {/each}
        </ul>
      </div>

      <div class="surface-card p-5">
        <h2 class="font-title text-2xl font-bold text-[#202223]">Categorie</h2>
        <div class="mt-3 grid gap-2">
          <input class="form-input" bind:value={categoryForm.name} placeholder="Nome" />
          <input class="form-input" bind:value={categoryForm.slug} placeholder="Slug" />
          <textarea class="form-input" bind:value={categoryForm.description} rows="2" placeholder="Descrizione"></textarea>
          <div class="flex gap-2">
            <button class="btn-primary" on:click={saveCategory} disabled={isSaving}>{editingCategoryId ? 'Aggiorna' : 'Crea'}</button>
            {#if editingCategoryId}<button class="btn-secondary" on:click={resetCategoryForm}>Annulla</button>{/if}
          </div>
        </div>
        <ul class="mt-4 space-y-2 text-sm">
          {#each categories as category}
            <li class="surface-muted flex items-center justify-between p-3">
              <div><p class="font-semibold text-[#202223]">{category.name}</p><p class="text-xs text-[#6d7175]">{category.slug}</p></div>
              <div class="flex gap-2"><button class="btn-secondary" on:click={() => startEditCategory(category)}>Modifica</button><button class="btn-secondary" on:click={() => removeCategory(category.id)}>Elimina</button></div>
            </li>
          {/each}
        </ul>
      </div>

      <div class="surface-card p-5 xl:col-span-2">
        <h2 class="font-title text-2xl font-bold text-[#202223]">Collezioni</h2>
        <div class="mt-3 grid gap-2 md:grid-cols-2">
          <input class="form-input" bind:value={collectionForm.name} placeholder="Nome" />
          <input class="form-input" bind:value={collectionForm.slug} placeholder="Slug" />
          <textarea class="form-input md:col-span-2" bind:value={collectionForm.description} rows="2" placeholder="Descrizione"></textarea>
          <label class="flex items-center gap-2 text-sm text-[#4a4f55] md:col-span-2"><input type="checkbox" bind:checked={collectionForm.isFeatured} />In evidenza</label>
          <div class="flex gap-2 md:col-span-2">
            <button class="btn-primary" on:click={saveCollection} disabled={isSaving}>{editingCollectionId ? 'Aggiorna' : 'Crea'}</button>
            {#if editingCollectionId}<button class="btn-secondary" on:click={resetCollectionForm}>Annulla</button>{/if}
          </div>
        </div>
        <ul class="mt-4 grid gap-2 md:grid-cols-2 text-sm">
          {#each collections as collection}
            <li class="surface-muted flex items-center justify-between p-3">
              <div><p class="font-semibold text-[#202223]">{collection.name}</p><p class="text-xs text-[#6d7175]">{collection.slug}</p></div>
              <div class="flex gap-2"><button class="btn-secondary" on:click={() => startEditCollection(collection)}>Modifica</button><button class="btn-secondary" on:click={() => removeCollection(collection.id)}>Elimina</button></div>
            </li>
          {/each}
        </ul>
      </div>
    </section>

    <section class="surface-card p-5">
      <h2 class="font-title text-2xl font-bold text-[#202223]">Prodotti</h2>
      <div class="mt-3 grid gap-2 md:grid-cols-2 xl:grid-cols-3">
        <input class="form-input" bind:value={productForm.sku} placeholder="SKU" />
        <input class="form-input" bind:value={productForm.name} placeholder="Nome" />
        <input class="form-input" type="number" min="0" step="0.01" bind:value={productForm.price} placeholder="Prezzo" />
        <textarea class="form-input md:col-span-2 xl:col-span-3" bind:value={productForm.description} rows="2" placeholder="Descrizione"></textarea>

        <select class="form-input" bind:value={productForm.brandId}>
          <option value="">Seleziona brand</option>
          {#each brands as brand}<option value={brand.id}>{brand.name}</option>{/each}
        </select>

        <select class="form-input" bind:value={productForm.categoryId}>
          <option value="">Seleziona categoria</option>
          {#each categories as category}<option value={category.id}>{category.name}</option>{/each}
        </select>

        <select class="form-input" multiple size="4" bind:value={productForm.collectionIds}>
          {#each collections as collection}<option value={collection.id}>{collection.name}</option>{/each}
        </select>

        <label class="flex items-center gap-2 text-sm text-[#4a4f55]"><input type="checkbox" bind:checked={productForm.isNewArrival} />New arrival</label>
        <label class="flex items-center gap-2 text-sm text-[#4a4f55]"><input type="checkbox" bind:checked={productForm.isBestSeller} />Best seller</label>

        <div class="flex gap-2 xl:col-span-3">
          <button class="btn-primary" on:click={saveProduct} disabled={isSaving}>{editingProductId ? 'Aggiorna prodotto' : 'Crea prodotto'}</button>
          {#if editingProductId}<button class="btn-secondary" on:click={resetProductForm}>Annulla</button>{/if}
        </div>
      </div>

      <div class="mt-5 overflow-auto">
        <table class="min-w-full text-sm">
          <thead>
            <tr class="border-b border-[#e1e3e5] text-left text-[#6d7175]"><th class="py-2">Prodotto</th><th>Brand</th><th>Categoria</th><th>Prezzo</th><th>Flags</th><th></th></tr>
          </thead>
          <tbody>
            {#each products as product}
              <tr class="border-b border-[#f1f2f3]"><td class="py-2"><p class="font-semibold text-[#202223]">{product.name}</p><p class="text-xs text-[#8c9196]">{product.sku}</p></td><td>{product.brandName}</td><td>{product.categoryName}</td><td>{formatCurrency(product.price)}</td><td>{product.isNewArrival ? 'New ' : ''}{product.isBestSeller ? 'Best' : ''}</td><td><div class="flex gap-2"><button class="btn-secondary" on:click={() => startEditProduct(product)}>Modifica</button><button class="btn-secondary" on:click={() => removeProduct(product.id)}>Elimina</button></div></td></tr>
            {/each}
          </tbody>
        </table>
      </div>
    </section>
  {/if}
</div>
