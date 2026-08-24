// VexFlow 4 notation rendering
// Expects global Vex from vexflow-min.js (UMD build).

function ensureVex() {
    if (typeof Vex === 'undefined') {
        console.warn('[FusionGuitar] VexFlow not loaded');
        return null;
    }
    return Vex.Flow;
}

export function renderStave(elementId, options) {
    const VF = ensureVex();
    if (!VF) return;

    const el = document.getElementById(elementId);
    if (!el) return;
    el.innerHTML = '';

    const {
        width = 480,
        clef = 'treble',
        timeSig = null,
        keySig = null,
        notes = [],
        tabNotes = null,
        autoBeam = true
    } = options || {};

    const rendererHeight = tabNotes ? 280 : 150;
    const renderer = new VF.Renderer(el, VF.Renderer.Backends.SVG);
    renderer.resize(width, rendererHeight);
    const context = renderer.getContext();
    context.setFont('Arial', 10, '').setBackgroundFillStyle('#fff');

    const stave = new VF.Stave(10, 10, width - 20);
    stave.addClef(clef);
    if (timeSig) stave.addTimeSignature(timeSig);
    if (keySig) stave.addKeySignature(keySig);
    stave.setContext(context).draw();

    let tabStave = null;
    if (tabNotes && clef === 'treble') {
        tabStave = new VF.TabStave(10, 130, width - 20);
        tabStave.addClef('tab').setContext(context).draw();
    }

    if (!notes || notes.length === 0) return;

    const vfNotes = notes.map(n => new VF.StaveNote({
        clef: clef,
        keys: n.keys,
        duration: n.duration || 'q'
    }));

    if (autoBeam) {
        const beams = VF.Beam.generateBeams(vfNotes);
        VF.Formatter.FormatAndDraw(context, stave, vfNotes);
        beams.forEach(b => b.setContext(context).draw());
    } else {
        VF.Formatter.FormatAndDraw(context, stave, vfNotes);
    }

    if (tabStave && tabNotes) {
        const tabVF = tabNotes.map(n => new VF.TabNote({
            positions: n.positions.map(p => ({ str: p.str, fret: p.fret })),
            duration: n.duration || 'q'
        }));
        VF.Formatter.FormatAndDraw(context, tabStave, tabVF);
    }
}

export function renderChordDiagram(elementId, options) {
    const VF = ensureVex();
    if (!VF) return;
    // For now use the existing ChordDiagram Razor component; reserved for future.
}
