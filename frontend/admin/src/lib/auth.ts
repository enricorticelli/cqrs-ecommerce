const AUTH_COOKIE_NAME = 'bo_auth';
const AUTH_COOKIE_VALUE = 'admin_session';

const ADMIN_USERNAME = 'admin';
const ADMIN_PASSWORD = 'admin';

type CookieStore = {
  get: (name: string) => { value: string } | undefined;
  set: (name: string, value: string, options?: Record<string, unknown>) => void;
  delete: (name: string, options?: Record<string, unknown>) => void;
};

export function isAuthenticated(cookies: CookieStore): boolean {
  return cookies.get(AUTH_COOKIE_NAME)?.value === AUTH_COOKIE_VALUE;
}

export function validateCredentials(username: string, password: string): boolean {
  return username === ADMIN_USERNAME && password === ADMIN_PASSWORD;
}

export function setAuthCookie(cookies: CookieStore): void {
  cookies.set(AUTH_COOKIE_NAME, AUTH_COOKIE_VALUE, {
    path: '/',
    httpOnly: true,
    sameSite: 'lax',
    secure: import.meta.env.PROD,
    maxAge: 60 * 60 * 8
  });
}

export function clearAuthCookie(cookies: CookieStore): void {
  cookies.delete(AUTH_COOKIE_NAME, { path: '/' });
}
