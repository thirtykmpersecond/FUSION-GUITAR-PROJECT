let synth = null;
let poly = null;
let started = false;

const NOTE_NAMES = ['C', 'C#', 'D', 'D#', 'E', 'F', 'F#', 'G', 'G#', 'A', 'A#', 'B'];

function midiToNote(midi) {
    const pc = ((midi % 12) + 12) % 12;
    const oct = Math.floor(midi / 12) - 1;
    return NOTE_NAMES[pc] + oct;
}

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

export async function init() {
    ensureContext();
    if (typeof Tone !== 'undefined' && Tone.context.state !== 'running') {
        await Tone.start();
    }
}

export function playNote(note, duration = 0.6, velocity = 0.8) {
    init().then(() => {
        if (!synth) return;
        synth.volume.value = -14 + velocity * 6;
        synth.triggerAttackRelease(note, duration);
    });
}

export function playMidi(midi, duration = 0.6, velocity = 0.8) {
    playNote(midiToNote(midi), duration, velocity);
}

export function playChord(notes, duration = 1.2, velocity = 0.7) {
    init().then(() => {
        if (!poly) return;
        poly.volume.value = -12 + velocity * 6;
        poly.triggerAttackRelease(notes, duration);
    });
}
