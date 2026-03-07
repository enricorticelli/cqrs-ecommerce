import { defineMiddleware } from 'astro:middleware';
import { isAuthenticated } from './lib/auth';

const PUBLIC_PATH_PREFIXES = ['/login'];

export const onRequest = defineMiddleware((context, next) => {
  const pathname = context.url.pathname;

  const isPublicPath = PUBLIC_PATH_PREFIXES.some((prefix) => pathname.startsWith(prefix));
  if (isPublicPath) {
    return next();
  }

  if (!isAuthenticated(context.cookies)) {
    const redirectUrl = new URL('/login', context.url);
    if (pathname !== '/') {
      redirectUrl.searchParams.set('next', pathname);
    }

    return context.redirect(redirectUrl.toString());
  }

  return next();
});
