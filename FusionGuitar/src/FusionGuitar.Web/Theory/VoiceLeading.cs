namespace FusionGuitar.Web.Theory;

/// <summary>Minimal voice-leading scoring for chaining voicings across a progression.</summary>
public static class VoiceLeading
{
    /// <summary>
    /// Given a chord and a previous voicing's sounding pitches, pick the voicing
    /// whose sounding pitches move the least (shared tones stay put).
    /// </summary>
    public static Voicing? BestNext(
        Chord chord,
        IReadOnlyList<Voicing> candidates,
        IReadOnlyList<int>? previousMidi = null)
    {
        if (candidates.Count == 0) return null;
        if (previousMidi is null || previousMidi.Count == 0)
            return candidates.First();

        Voicing? best = null;
        int bestCost = int.MaxValue;
        foreach (var v in candidates)
        {
            var midi = SoundingMidi(v).OrderBy(x => x).ToArray();
            int cost = 0;
            // Pair the sorted sounding notes with the sorted previous notes.
            for (int i = 0; i < Math.Min(midi.Length, previousMidi.Count); i++)
                cost += Math.Abs(midi[i] - previousMidi[i]);
            if (cost < bestCost)
            {
                bestCost = cost;
                best = v;
            }
        }
        return best;
    }

    /// <summary>Actual sounding MIDI (unmuted strings), from low to high.</summary>
    public static IReadOnlyList<int> SoundingMidi(Voicing v)
        => v.Fingerings
            .Where(f => !f.Muted)
            .OrderBy(f => f.StringIndex)
            .Select(f => OpenMidi[f.StringIndex] + f.Fret)
            .ToList();

    // Open-string MIDI per string index (0 = low E ... 5 = high E).
    private static readonly int[] OpenMidi = { 40, 45, 50, 55, 59, 64 };
}
