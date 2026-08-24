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
        tabNotes = null
    } = options || {};

    const showTab = !!(tabNotes && tabNotes.length);
    const height = showTab ? 300 : 170;
    const renderer = new VF.Renderer(el, VF.Renderer.Backends.SVG);
    renderer.resize(width, height);
    const ctx = renderer.getContext();
    ctx.setFont('Arial', 10, '');

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
            const stdNotes = notes.map(n => new VF.StaveNote({
                clef: clef,
                keys: n.keys || [],
                duration: n.duration || 'q'
            }));
            const beams = VF.Beam.generateBeams(stdNotes, {
                groups: [new VF.Fraction(2, 8), new VF.Fraction(3, 8)]
            });
            VF.Formatter.FormatAndDraw(ctx, stave, stdNotes);
            beams.forEach(b => b.setContext(ctx).draw());

            if (hasTab) {
                // Align tab notes with standard notes using a common formatter.
                const tabVF = tabNotes.map(n => new VF.TabNote({
                    positions: (n.positions || []).map(p => ({ str: p.str, fret: p.fret })),
                    duration: n.duration || 'q'
                }));
                VF.Formatter.FormatAndDraw(ctx, tabStave, tabVF);
            }
        } else if (hasTab) {
            const tabVF = tabNotes.map(n => new VF.TabNote({
                positions: (n.positions || []).map(p => ({ str: p.str, fret: p.fret })),
                duration: n.duration || 'q'
            }));
            const beams = VF.Beam.generateBeams(tabVF);
            VF.Formatter.FormatAndDraw(ctx, tabStave, tabVF);
            beams.forEach(b => b.setContext(ctx).draw());
        }
    } catch (e) {
        console.error('[FusionGuitar] VexFlow error:', e);
        renderError(el, e.message || String(e));
    }
}
