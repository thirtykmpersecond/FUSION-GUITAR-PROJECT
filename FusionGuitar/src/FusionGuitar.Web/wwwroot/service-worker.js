// Fusion Guitar service worker: network-first with cache fallback.
// Offline works after the first successful visit.

const CACHE = 'fusion-guitar-v1';
const CORE = [
    '/',
    '/index.html',
    '/css/app.css',
    '/FusionGuitar.Web.styles.css',
    '/manifest.webmanifest',
    '/favicon.png',
    '/icon-192.png'
];

self.addEventListener('install', (event) => {
    event.waitUntil(
        caches.open(CACHE).then((cache) => cache.addAll(CORE)).then(() => self.skipWaiting())
    );
});

self.addEventListener('activate', (event) => {
    event.waitUntil(
        caches.keys()
            .then((keys) => Promise.all(keys.filter((k) => k !== CACHE).map((k) => caches.delete(k))))
            .then(() => self.clients.claim())
    );
});

self.addEventListener('fetch', (event) => {
    const req = event.request;
    if (req.method !== 'GET') return;

    // Network-first with cache fallback: always try the network so updates
    // propagate, but fall back to cache when offline.
    event.respondWith(
        fetch(req)
            .then((res) => {
                // Cache successful same-origin responses.
                if (res && res.ok && new URL(req.url).origin === self.location.origin) {
                    const clone = res.clone();
                    caches.open(CACHE).then((cache) => cache.put(req, clone));
                }
                return res;
            })
            .catch(() => caches.match(req).then((cached) => {
                // Offline navigation fallback: serve the app shell.
                if (cached) return cached;
                if (req.mode === 'navigate') return caches.match('/index.html');
                return Response.error();
            }))
    );
});
