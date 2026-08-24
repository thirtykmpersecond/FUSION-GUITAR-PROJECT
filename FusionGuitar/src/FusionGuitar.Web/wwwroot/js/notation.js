// VexFlow 4 notation rendering.
// Expects global Vex from vexflow-min.js (UMD build loaded in index.html).

function getVF() {
    if (typeof Vex === 'undefined') {
        console.warn('[FusionGuitar] VexFlow not loaded');
        return null;
    }
    return Vex.Flow;
}

export function renderStave(elementId, options) {
    const VF = getVF();
    if (!VF) return;
    const el = document.getElementById(elementId);
    if (!el) return;
    el.innerHTML = '';

    const {
        width = 520,
        clef = 'treble',
        timeSig = null,
        keySig = null,
        notes = []
    } = options || {};

    const renderer = new VF.Renderer(el, VF.Renderer.Backends.SVG);
    renderer.resize(width, 160);
    const ctx = renderer.getContext();
    ctx.setFont('Arial', 10, '');

    const stave = new VF.Stave(10, 20, width - 20);
    stave.addClef(clef);
    if (timeSig) stave.addTimeSignature(timeSig);
    if (keySig) stave.addKeySignature(keySig);
    stave.setContext(ctx).draw();

    if (!notes || notes.length === 0) return;

    try {
        const vfNotes = notes.map(n => {
            const keys = n.keys || [];
            const duration = n.duration || 'q';
            return new VF.StaveNote({
                clef: clef,
                keys: keys,
                duration: duration
            });
        });

        const beams = VF.Beam.generateBeams(vfNotes);
        VF.Formatter.FormatAndDraw(ctx, stave, vfNotes);
        beams.forEach(b => b.setContext(ctx).draw());
    } catch (e) {
        console.error('[FusionGuitar] VexFlow render error:', e);
        el.innerHTML = '<div style="color:#ef4444;font-size:12px;padding:8px;">乐谱渲染失败: ' + e.message + '</div>';
    }
}
