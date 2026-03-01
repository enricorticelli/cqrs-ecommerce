const gatewayUrl = (): string =>
  (typeof window !== 'undefined'
    ? (import.meta.env.PUBLIC_GATEWAY_URL as string | undefined)
    : undefined) ?? 'http://localhost:8080';

// ─── Types ────────────────────────────────────────────────────────────────────

export type Product = {
  id: string;
  sku: string;
  name: string;
  description: string;
  price: number;
};

export type CartItemDto = {
  productId: string;
  sku: string;
  name: string;
  quantity: number;
  unitPrice: number;
};

export type CartView = {
  cartId: string;
  userId: string;
  items: CartItemDto[];
  totalAmount: number;
};

export type OrderItemDto = {
  productId: string;
  sku: string;
  name: string;
  quantity: number;
  unitPrice: number;
};

export type OrderView = {
  id: string;
  cartId: string;
  userId: string;
  status: string;
  totalAmount: number;
  items: OrderItemDto[];
  trackingCode: string | null;
  transactionId: string | null;
  failureReason: string | null;
};

export type CreateOrderResult = {
  orderId: string;
  status: string;
};

export type ApiError = {
  status: number;
  title: string;
  detail?: string;
};

// ─── Catalog ─────────────────────────────────────────────────────────────────

export async function fetchProducts(): Promise<Product[]> {
  const res = await fetch(`${gatewayUrl()}/api/catalog/v1/products`);
  if (!res.ok) throw new Error(`Catalog error: ${res.status}`);
  return res.json();
}

export async function fetchProduct(id: string): Promise<Product> {
  const res = await fetch(`${gatewayUrl()}/api/catalog/v1/products/${id}`);
  if (res.status === 404) throw new NotFoundError(`Product ${id} not found`);
  if (!res.ok) throw new Error(`Catalog error: ${res.status}`);
  return res.json();
}

// ─── Cart ─────────────────────────────────────────────────────────────────────

export async function fetchCart(cartId: string): Promise<CartView | null> {
  const res = await fetch(`${gatewayUrl()}/api/cart/v1/carts/${cartId}`);
  if (res.status === 404) return null;
  if (!res.ok) throw new Error(`Cart error: ${res.status}`);
  return res.json();
}

export async function addCartItem(
  cartId: string,
  payload: {
    userId: string;
    productId: string;
    sku: string;
    name: string;
    quantity: number;
    unitPrice: number;
  }
): Promise<void> {
  const res = await fetch(`${gatewayUrl()}/api/cart/v1/carts/${cartId}/items`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => null);
    throw new Error(err?.detail ?? `Cart add error: ${res.status}`);
  }
}

export async function removeCartItem(cartId: string, productId: string): Promise<void> {
  const res = await fetch(
    `${gatewayUrl()}/api/cart/v1/carts/${cartId}/items/${productId}`,
    { method: 'DELETE' }
  );
  if (!res.ok) throw new Error(`Cart remove error: ${res.status}`);
}

// ─── Order ────────────────────────────────────────────────────────────────────

export async function createOrder(cartId: string, userId: string): Promise<CreateOrderResult> {
  const res = await fetch(`${gatewayUrl()}/api/order/v1/orders`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ cartId, userId }),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => null);
    throw new Error(err?.detail ?? `Order create error: ${res.status}`);
  }
  return res.json();
}

export async function fetchOrder(orderId: string): Promise<OrderView> {
  const res = await fetch(`${gatewayUrl()}/api/order/v1/orders/${orderId}`);
  if (res.status === 404) throw new NotFoundError(`Order ${orderId} not found`);
  if (!res.ok) throw new Error(`Order fetch error: ${res.status}`);
  return res.json();
}

// ─── Utilities ───────────────────────────────────────────────────────────────

export class NotFoundError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'NotFoundError';
  }
}

/** Poll order until it reaches a terminal state. Calls onUpdate on each poll tick. */
export async function pollOrderUntilDone(
  orderId: string,
  onUpdate: (order: OrderView) => void,
  maxAttempts = 40,
  intervalMs = 1000
): Promise<OrderView | null> {
  for (let i = 0; i < maxAttempts; i++) {
    try {
      const order = await fetchOrder(orderId);
      onUpdate(order);
      if (order.status === 'Completed' || order.status === 'Failed') return order;
    } catch {
      // transient error — keep polling
    }
    await new Promise((r) => setTimeout(r, intervalMs));
  }
  return null;
}
