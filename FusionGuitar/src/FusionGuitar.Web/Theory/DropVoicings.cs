namespace FusionGuitar.Web.Theory;

/// <summary>Drop-2 / Drop-3 voicing generator for 4-voice 7th chords on guitar.</summary>
public static class DropVoicings
{
    // Open-string MIDI per string index (0 = low E ... 5 = high E).
    private static readonly int[] OpenMidi = { 40, 45, 50, 55, 59, 64 };
    private const int MaxFret = 15;
    private const int MaxSpanFrets = 5;

    public enum DropType
    {
        Drop2,
        Drop3
    }

    // String groups expressed in ascending string indices (low E = 0).
    // "1234" (VexFlow: 1 = high E) => strings 2,3,4,5 etc.
    private static readonly int[][] Groups =
    {
        new[] { 2, 3, 4, 5 }, // ①②③④ (high set)
        new[] { 1, 2, 3, 4 }, // ②③④⑤
        new[] { 0, 1, 2, 3 }, // ③④⑤⑥ (low set)
    };

    public static IReadOnlyList<Voicing> Generate(Chord chord, DropType type, string strings)
    {
        if (chord.Notes.Count != 4)
            throw new ArgumentException("Drop voicings require a 4-voice chord", nameof(chord));

        int[] group = ParseStrings(strings);
        var results = new List<Voicing>();

        // Each inversion gives a distinct close-position voicing to drop from.
        for (int inv = 0; inv < 4; inv++)
        {
            var close = chord.Inversion(inv).Select(n => n.MidiNumber).OrderBy(x => x).ToArray();
            // Drop the 2nd-from-top (index 2) or 3rd-from-top (index 1) down an octave.
            int dropIdx = type == DropType.Drop2 ? 2 : 1;
            var dropped = close.ToList();
            dropped[dropIdx] -= 12;
            dropped.Sort();
            var pitches = dropped.ToArray();

            foreach (var octaveOffset in new[] { -24, -12, 0, 12, 24 })
            {
                var voicing = MapToGroup(chord, group, pitches, octaveOffset, type);
                if (voicing is not null) results.Add(voicing);
            }
        }

        return Dedupe(results);
    }

    private static int[] ParseStrings(string s)
    {
        // Accept "1234" (VexFlow, 1 = high E) -> ascending indices {2,3,4,5}.
        var raw = s.Where(char.IsDigit).Select(c => c - '0').ToArray();
        if (raw.Length != 4)
            throw new ArgumentException("strings must name exactly 4 strings", nameof(s));
        var ascending = raw.Select(x => x - 1).OrderBy(x => x).ToArray();
        // Validate it is a contiguous run matching one of the known groups.
        if (!Groups.Any(g => g.SequenceEqual(ascending)))
            throw new ArgumentException("strings must be a contiguous 4-string run", nameof(s));
        return ascending;
    }

    /// <summary>Map the 4 ascending pitches onto the string group, transposed by octaveOffset.</summary>
    private static Voicing? MapToGroup(Chord chord, int[] group, int[] pitches, int octaveOffset, DropType type)
    {
        // pitch i must land on string group[i] (ascending) => fret = pitch+offset - openMidi.
        var fings = new Fingering[4];
        for (int i = 0; i < 4; i++)
        {
            int fret = pitches[i] + octaveOffset - OpenMidi[group[i]];
            if (fret < 0 || fret > MaxFret) return null;
            fings[i] = new Fingering(group[i], fret);
        }

        int minFret = fings.Where(f => f.Fret > 0).Select(f => f.Fret).DefaultIfEmpty(0).Min();
        int maxFret = fings.Where(f => f.Fret > 0).Select(f => f.Fret).DefaultIfEmpty(0).Max();
        if (maxFret - minFret > MaxSpanFrets) return null;

        // Guard: every fretted string must produce a chord tone.
        var present = new HashSet<int>();
        foreach (var f in fings)
            present.Add(((OpenMidi[f.StringIndex] + f.Fret) % 12 + 12) % 12);
        if (!chord.PitchClasses.IsSubsetOf(present)) return null;

        var all = new List<Fingering>();
        for (int s = 0; s < 6; s++)
        {
            var f = fings.FirstOrDefault(x => x.StringIndex == s);
            all.Add(f ?? new Fingering(s, 0, Muted: true));
        }

        int baseFret = minFret > 0 ? minFret : 0;
        var notes = string.Join(" ", all.Select(f =>
            f.Muted ? "x" : f.Fret > 0 ? f.Fret.ToString() : "0"));
        var label = $"{chord.Name} {Describe(type, group)}";
        return new Voicing(label, chord.Root.Name, chord.Formula.Quality, all, baseFret, notes);
    }

    private static string Describe(DropType type, int[] group)
    {
        var s = string.Join("", group.Select(x => x + 1));
        return $"({type.ToString().ToLowerInvariant()} ①②③④ subset {s})";
    }

    private static IReadOnlyList<Voicing> Dedupe(IReadOnlyList<Voicing> list)
    {
        var seen = new HashSet<string>();
        var result = new List<Voicing>();
        foreach (var v in list)
        {
            var key = v.Notes ?? "";
            if (seen.Add(key)) result.Add(v);
        }
        return result;
    }
}
