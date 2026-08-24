// Theme management for Fusion Guitar.
// Persists to localStorage ('fg-theme') and toggles the 'dark' class on <html>.

const KEY = 'fg-theme';

export function current() {
    const v = localStorage.getItem(KEY);
    if (v === 'dark' || v === 'light') return v;
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

export function apply(theme) {
    const root = document.documentElement;
    if (theme === 'dark') {
        root.classList.add('dark');
    } else {
        root.classList.remove('dark');
    }
    root.style.colorScheme = theme;
    try { localStorage.setItem(KEY, theme); } catch (e) { }
}

export function toggle() {
    const next = current() === 'dark' ? 'light' : 'dark';
    apply(next);
    return next;
}
