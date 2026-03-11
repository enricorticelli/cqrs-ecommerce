const ACCESS_TOKEN_KEY = 'store:accessToken';
const REFRESH_TOKEN_KEY = 'store:refreshToken';

export type AuthTokens = {
  accessToken: string;
  refreshToken: string;
};

export function saveAuthTokens(tokens: AuthTokens): void {
  if (typeof localStorage === 'undefined') return;
  localStorage.setItem(ACCESS_TOKEN_KEY, tokens.accessToken);
  localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);
}

export function clearAuthTokens(): void {
  if (typeof localStorage === 'undefined') return;
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
}

export function getAccessToken(): string | null {
  if (typeof localStorage === 'undefined') return null;
  return localStorage.getItem(ACCESS_TOKEN_KEY);
}

export function getRefreshToken(): string | null {
  if (typeof localStorage === 'undefined') return null;
  return localStorage.getItem(REFRESH_TOKEN_KEY);
}

export function isLoggedIn(): boolean {
  const payload = parseAccessTokenPayload();
  if (!payload) return false;

  const exp = Number(payload.exp ?? 0);
  if (!Number.isFinite(exp) || exp <= 0) return false;

  return exp > Math.floor(Date.now() / 1000);
}

export function getCurrentUserId(): string | null {
  const payload = parseAccessTokenPayload();
  if (!payload) return null;

  const subject = typeof payload.sub === 'string' ? payload.sub : null;
  return subject && subject.length > 0 ? subject : null;
}

export function getAuthorizationHeader(): Record<string, string> {
  const token = getAccessToken();
  if (!token) return {};
  return { Authorization: `Bearer ${token}` };
}

export function parseAccessTokenPayload(): Record<string, unknown> | null {
  const token = getAccessToken();
  if (!token) return null;
  return parseJwtPayload(token);
}

export function parseJwtPayload(token: string): Record<string, unknown> | null {
  const chunks = token.split('.');
  if (chunks.length < 2) return null;

  const base64 = chunks[1].replace(/-/g, '+').replace(/_/g, '/');
  const padded = base64 + '='.repeat((4 - (base64.length % 4 || 4)) % 4);

  try {
    return JSON.parse(atob(padded)) as Record<string, unknown>;
  } catch {
    return null;
  }
}
