window.FusionGuitar = window.FusionGuitar || {};

(function (fg) {
    let synth = null;
    let poly = null;
    let started = false;

    function ensureContext() {
        if (started) return;
        if (typeof Tone === 'undefined') {
            console.warn('[FusionGuitar] Tone.js not loaded');
            return;
        }
        synth = new Tone.Synth({
            oscillator: { type: 'triangle' },
            envelope: { attack: 0.005, decay: 0.1, sustain: 0.3, release: 0.8 }
        }).toDestination();
        poly = new Tone.PolySynth(Tone.Synth, {
            oscillator: { type: 'triangle8' },
            envelope: { attack: 0.005, decay: 0.2, sustain: 0.4, release: 1.0 }
        }).toDestination();
        poly.volume.value = -6;
        started = true;
    }

    async function start() {
        ensureContext();
        if (Tone && Tone.context.state !== 'running') {
            await Tone.start();
        }
    }

    const NOTE_NAMES = ['C', 'C#', 'D', 'D#', 'E', 'F', 'F#', 'G', 'G#', 'A', 'A#', 'B'];
    function midiToNote(midi) {
        const pc = ((midi % 12) + 12) % 12;
        const oct = Math.floor(midi / 12) - 1;
        return NOTE_NAMES[pc] + oct;
    }

    fg.init = start;

    fg.playNote = function (note, duration, velocity) {
        start().then(() => {
            if (!synth) return;
            synth.volume.value = -12 + (velocity || 0.8) * 6;
            synth.triggerAttackRelease(note, duration || 0.6);
        });
    };

    fg.playMidi = function (midi, duration, velocity) {
        fg.playNote(midiToNote(midi), duration, velocity);
    };

    fg.playChord = function (notes, duration, velocity) {
        start().then(() => {
            if (!poly) return;
            poly.volume.value = -10 + (velocity || 0.7) * 6;
            poly.triggerAttackRelease(notes, duration || 1.2);
        });
    };
})(window.FusionGuitar);
