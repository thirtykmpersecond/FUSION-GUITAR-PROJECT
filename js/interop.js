let synth = null;
let poly = null;
let clickSynth = null;
let started = false;
let metronomeLoop = null;
let scheduledIds = [];

const NOTE_NAMES = ['C', 'C#', 'D', 'D#', 'E', 'F', 'F#', 'G', 'G#', 'A', 'A#', 'B'];

function midiToNote(midi) {
    const pc = ((midi % 12) + 12) % 12;
    const oct = Math.floor(midi / 12) - 1;
    return NOTE_NAMES[pc] + oct;
}

function ensureContext() {
    if (started) return;
    if (typeof Tone === 'undefined') return;
    synth = new Tone.Synth({
        oscillator: { type: 'triangle' },
        envelope: { attack: 0.005, decay: 0.1, sustain: 0.3, release: 0.8 }
    }).toDestination();
    poly = new Tone.PolySynth(Tone.Synth, {
        oscillator: { type: 'triangle8' },
        envelope: { attack: 0.005, decay: 0.2, sustain: 0.4, release: 1.0 }
    }).toDestination();
    poly.volume.value = -6;
    clickSynth = new Tone.MembraneSynth({
        pitchDecay: 0.01,
        octaves: 4
    }).toDestination();
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

export function setBpm(bpm) {
    if (typeof Tone !== 'undefined') Tone.Transport.bpm.value = bpm;
}

export async function startMetronome(bpm) {
    await init();
    if (typeof Tone === 'undefined') return;
    stopMetronome();
    setBpm(bpm);
    let beat = 0;
    metronomeLoop = new Tone.Loop((time) => {
        const isDownbeat = beat % 4 === 0;
        clickSynth.triggerAttackRelease(isDownbeat ? 'C2' : 'G2', '8n', time);
        beat++;
    }, '4n');
    metronomeLoop.start(0);
    await Tone.Transport.start();
}

export function stopMetronome() {
    if (metronomeLoop) {
        metronomeLoop.stop();
        metronomeLoop.dispose();
        metronomeLoop = null;
    }
    if (typeof Tone !== 'undefined' && Tone.Transport.state === 'started') {
        Tone.Transport.stop();
    }
}

// chords: [{ notes: ["C4","E4","G4"], dur: "2n" }, ...]
export async function playProgression(chords, bpm) {
    await init();
    if (typeof Tone === 'undefined') return;
    stopProgression();
    setBpm(bpm);
    let cursor = 0;
    chords.forEach((chord) => {
        const dur = chord.dur || '2n';
        const when = cursor;
        const id = Tone.Transport.schedule((time) => {
            poly.triggerAttackRelease(chord.notes, dur, time);
        }, when);
        scheduledIds.push(id);
        cursor += Tone.Time(dur).toSeconds();
    });
    Tone.Transport.loop = true;
    Tone.Transport.loopEnd = cursor;
    await Tone.Transport.start();
}

export function stopProgression() {
    if (typeof Tone === 'undefined') return;
    scheduledIds.forEach(id => Tone.Transport.clear(id));
    scheduledIds = [];
    Tone.Transport.loop = false;
    Tone.Transport.loopEnd = 0;
    if (Tone.Transport.state === 'started') Tone.Transport.stop();
}

// notes: [{ midi, beats }]; plays each note in sequence, calling onNote(index)
// as each note sounds. Returns the number of notes scheduled.
export async function scheduleSequence(notes, bpm, onNote) {
    await init();
    if (typeof Tone === 'undefined' || !notes || !notes.length) return 0;
    stopProgression();
    setBpm(bpm);
    let cursor = 0;
    notes.forEach((n, i) => {
        const secs = n.beats * (60 / bpm);
        const dur = Math.max(secs * 0.9, 0.12);
        const id = Tone.Transport.schedule((time) => {
            poly.triggerAttackRelease(midiToNote(n.midi), dur, time);
            if (onNote && typeof onNote === 'function') onNote(i);
        }, cursor);
        scheduledIds.push(id);
        cursor += secs;
    });
    Tone.Transport.loop = false;
    await Tone.Transport.start();
    return notes.length;
}

export function stopSequence() {
    stopProgression();
}

export function transposeNotes(notes, semitones) {
    return notes.map(n => {
        if (typeof n === 'number') return midiToNote(n + semitones);
        const m = n.match(/^([A-G][#b]?)(\d)$/);
        if (!m) return n;
        const pc = NOTE_NAMES.indexOf(m[1].replace('b', 'b'));
        const oct = parseInt(m[2], 10);
        return midiToNote((oct + 1) * 12 + pc + semitones);
    });
}
