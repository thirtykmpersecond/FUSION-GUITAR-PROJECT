// VexFlow 4 renderer for Fusion Guitar.
// Supports:
//   - Standard 5-line staff (clef/key/time/notes)
//   - Guitar TAB staff (tabNotes: [{ positions: [{str, fret}], duration }])
//   - Both connected with StaveConnector
// Expects global Vex from vexflow-min.js (UMD).

function getVF() {
    if (typeof Vex === 'undefined') {
        console.warn('[FusionGuitar] VexFlow not loaded');
        return null;
    }
    return Vex.Flow;
}

function renderError(el, msg) {
    el.innerHTML = '<div style="color:#ef4444;font-size:12px;padding:8px;border:1px solid #fecaca;border-radius:8px;">乐谱渲染失败：' + msg + '</div>';
}

export function renderStave(elementId, options) {
    const VF = getVF();
    if (!VF) return;
    const el = document.getElementById(elementId);
    if (!el) return;
    el.innerHTML = '';

    const {
        width = 540,
        clef = 'treble',
        timeSig = null,
        keySig = null,
        notes = [],
        tabNotes = null,
        onNoteClick = null
    } = options || {};

    const showTab = !!(tabNotes && tabNotes.length);
    const height = showTab ? 300 : 170;
    const renderer = new VF.Renderer(el, VF.Renderer.Backends.SVG);
    renderer.resize(width, height);
    const ctx = renderer.getContext();
    ctx.setFont('Arial', 10, '');
    __noteBoxes.delete(elementId);
    window.__fusionNoteRegistry = [];

    try {
        const stave = new VF.Stave(10, 20, width - 20);
        stave.addClef(clef);
        if (timeSig) stave.addTimeSignature(timeSig);
        // Key signature: VexFlow 4 rejects "C" / "" as invalid; only set for non-C keys.
        if (keySig && keySig !== 'C' && keySig !== 'Am') {
            stave.addKeySignature(keySig);
        }
        stave.setContext(ctx).draw();

        let tabStave = null;
        if (showTab) {
            tabStave = new VF.TabStave(10, 150, width - 20);
            tabStave.addClef('tab');
            tabStave.setContext(ctx).draw();
            new VF.StaveConnector(stave, tabStave).setContext(ctx).draw();
        }

        const hasStdNotes = notes && notes.length > 0;
        const hasTab = showTab && tabNotes.length > 0;

        if (hasStdNotes) {
            const stdNotes = notes.map((n, i) => {
                const note = new VF.StaveNote({
                    clef: clef,
                    keys: n.keys || [],
                    duration: n.duration || 'q'
                });
                wireClick(note, i, onNoteClick);
                return note;
            });
            const beams = VF.Beam.generateBeams(stdNotes, {
                groups: [new VF.Fraction(2, 8), new VF.Fraction(3, 8)]
            });
            VF.Formatter.FormatAndDraw(ctx, stave, stdNotes);
            beams.forEach(b => b.setContext(ctx).draw());
            registerBoxes(elementId, stdNotes);

            if (hasTab) {
                // Align tab notes with standard notes using a common formatter.
                const tabVF = tabNotes.map((n, i) => {
                    const note = new VF.TabNote({
                        positions: (n.positions || []).map(p => ({ str: p.str, fret: p.fret })),
                        duration: n.duration || 'q'
                    });
                    wireClick(note, i, onNoteClick);
                    return note;
                });
                VF.Formatter.FormatAndDraw(ctx, tabStave, tabVF);
                registerBoxes(elementId, tabVF);
            }
        } else if (hasTab) {
            const tabVF = tabNotes.map((n, i) => {
                const note = new VF.TabNote({
                    positions: (n.positions || []).map(p => ({ str: p.str, fret: p.fret })),
                    duration: n.duration || 'q'
                });
                wireClick(note, i, onNoteClick);
                return note;
            });
            const beams = VF.Beam.generateBeams(tabVF);
            VF.Formatter.FormatAndDraw(ctx, tabStave, tabVF);
            beams.forEach(b => b.setContext(ctx).draw());
            registerBoxes(elementId, tabVF);
        }

        wireInteractivity(elementId);
    } catch (e) {
        console.error('[FusionGuitar] VexFlow error:', e);
        renderError(el, e.message || String(e));
    }
}

function wireClick(note, index, onNoteClick) {
    if (!onNoteClick) return;
    // Record the note so we can wire hit areas after the SVG is drawn.
    window.__fusionNoteRegistry = window.__fusionNoteRegistry || [];
    window.__fusionNoteRegistry.push({ note, index, cb: makeInvoker(onNoteClick) });
}

function makeInvoker(onNoteClick) {
    // Supports both a plain JS function and a DotNetObjectReference-shaped object
    // ({ invoke, target } from the C# interop layer).
    if (typeof onNoteClick === 'function') return onNoteClick;
    if (onNoteClick && typeof onNoteClick.target?.invokeMethodAsync === 'function') {
        return (index) => onNoteClick.target.invokeMethodAsync('OnNoteClick', index);
    }
    return null;
}

// After the stave(s) are drawn, attach click + highlight capability by wrapping
// each note in a transparent hit-area rect positioned from the note bounding box.
function wireInteractivity(elementId) {
    const el = document.getElementById(elementId);
    if (!el) return;
    const svg = el.querySelector('svg');
    if (!svg) return;
    const reg = (window.__fusionNoteRegistry || []).slice();
    window.__fusionNoteRegistry = [];
    const NS = 'http://www.w3.org/2000/svg';
    reg.forEach(({ note, index, cb }) => {
        const bb = note.getBoundingBox();
        if (!bb) return;
        const x = bb.getX(), y = bb.getY(), w = bb.getW(), h = bb.getH();
        const hit = document.createElementNS(NS, 'rect');
        hit.setAttribute('x', x - 4);
        hit.setAttribute('y', y - 6);
        hit.setAttribute('width', Math.max(w + 8, 14));
        hit.setAttribute('height', Math.max(h + 12, 20));
        hit.setAttribute('fill', 'transparent');
        hit.style.cursor = 'pointer';
        hit.addEventListener('click', () => cb && cb(index));
        svg.appendChild(hit);
    });
}

function registerBoxes(elementId, notes) {
    const boxes = [];
    notes.forEach((note, index) => {
        const bb = note.getBoundingBox();
        if (!bb) return;
        boxes.push({
            index,
            x: bb.getX(), y: bb.getY(), w: bb.getW(), h: bb.getH()
        });
    });
    __noteBoxes.set(elementId, boxes);
}

// Highlight a note (used by LickPlayer): adds a colored ring via a stored registry
// of note bounding boxes.
const __noteBoxes = new Map(); // elementId -> [ {x,y,w,h,index} ]

export function highlightNote(elementId, index, color = '#ef4444') {
    const el = document.getElementById(elementId);
    if (!el) return;
    const svg = el.querySelector('svg');
    if (!svg) return;
    clearHighlight(elementId);
    const boxes = __noteBoxes.get(elementId) || [];
    const box = boxes.find(b => b.index === index);
    if (!box) return;
    const NS = 'http://www.w3.org/2000/svg';
    const ring = document.createElementNS(NS, 'rect');
    ring.setAttribute('x', box.x - 5);
    ring.setAttribute('y', box.y - 7);
    ring.setAttribute('width', Math.max(box.w + 10, 16));
    ring.setAttribute('height', Math.max(box.h + 14, 22));
    ring.setAttribute('fill', 'none');
    ring.setAttribute('stroke', color);
    ring.setAttribute('stroke-width', '2.5');
    ring.setAttribute('rx', '4');
    ring.setAttribute('data-highlight', '1');
    svg.appendChild(ring);
}

export function clearHighlight(elementId) {
    const el = document.getElementById(elementId);
    if (!el) return;
    const svg = el.querySelector('svg');
    if (!svg) return;
    svg.querySelectorAll('[data-highlight]').forEach(n => n.remove());
}
