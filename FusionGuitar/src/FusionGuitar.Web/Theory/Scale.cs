namespace FusionGuitar.Web.Theory;

public sealed record ScaleFormula(string Name, ScaleFamily Family, IReadOnlyList<int> Intervals)
{
    public IReadOnlySet<int> IntervalSet { get; } = new HashSet<int>(Intervals);
}

public static class ScaleFormulas
{
    public static readonly ScaleFormula Major =
        new("Major", ScaleFamily.Major, new[] { 0, 2, 4, 5, 7, 9, 11 });
    public static readonly ScaleFormula NaturalMinor =
        new("Natural Minor", ScaleFamily.NaturalMinor, new[] { 0, 2, 3, 5, 7, 8, 10 });
    public static readonly ScaleFormula HarmonicMinor =
        new("Harmonic Minor", ScaleFamily.HarmonicMinor, new[] { 0, 2, 3, 5, 7, 8, 11 });
    public static readonly ScaleFormula MelodicMinor =
        new("Melodic Minor", ScaleFamily.MelodicMinor, new[] { 0, 2, 3, 5, 7, 9, 11 });

    public static readonly ScaleFormula Ionian =
        new("Ionian", ScaleFamily.Ionian, new[] { 0, 2, 4, 5, 7, 9, 11 });
    public static readonly ScaleFormula Dorian =
        new("Dorian", ScaleFamily.Dorian, new[] { 0, 2, 3, 5, 7, 9, 10 });
    public static readonly ScaleFormula Phrygian =
        new("Phrygian", ScaleFamily.Phrygian, new[] { 0, 1, 3, 5, 7, 8, 10 });
    public static readonly ScaleFormula Lydian =
        new("Lydian", ScaleFamily.Lydian, new[] { 0, 2, 4, 6, 7, 9, 11 });
    public static readonly ScaleFormula Mixolydian =
        new("Mixolydian", ScaleFamily.Mixolydian, new[] { 0, 2, 4, 5, 7, 9, 10 });
    public static readonly ScaleFormula Aeolian =
        new("Aeolian", ScaleFamily.Aeolian, new[] { 0, 2, 3, 5, 7, 8, 10 });
    public static readonly ScaleFormula Locrian =
        new("Locrian", ScaleFamily.Locrian, new[] { 0, 1, 3, 5, 6, 8, 10 });

    public static readonly ScaleFormula PentatonicMajor =
        new("Major Pentatonic", ScaleFamily.PentatonicMajor, new[] { 0, 2, 4, 7, 9 });
    public static readonly ScaleFormula PentatonicMinor =
        new("Minor Pentatonic", ScaleFamily.PentatonicMinor, new[] { 0, 3, 5, 7, 10 });
    public static readonly ScaleFormula Blues =
        new("Blues", ScaleFamily.Blues, new[] { 0, 3, 5, 6, 7, 10 });
    public static readonly ScaleFormula WholeTone =
        new("Whole Tone", ScaleFamily.WholeTone, new[] { 0, 2, 4, 6, 8, 10 });
    public static readonly ScaleFormula DiminishedHalfWhole =
        new("Diminished (H-W)", ScaleFamily.DiminishedHalfWhole, new[] { 0, 1, 3, 4, 6, 7, 9, 10 });
    public static readonly ScaleFormula DiminishedWholeHalf =
        new("Diminished (W-H)", ScaleFamily.DiminishedWholeHalf, new[] { 0, 2, 3, 5, 6, 8, 9, 11 });
    public static readonly ScaleFormula BebopDominant =
        new("Bebop Dominant", ScaleFamily.BebopDominant, new[] { 0, 2, 4, 5, 7, 9, 10, 11 });
    public static readonly ScaleFormula LydianDominant =
        new("Lydian Dominant", ScaleFamily.LydianDominant, new[] { 0, 2, 4, 6, 7, 9, 10 });
    public static readonly ScaleFormula LydianAugmented =
        new("Lydian Augmented", ScaleFamily.LydianAugmented, new[] { 0, 2, 4, 6, 8, 9, 11 });
    public static readonly ScaleFormula MixolydianFlat6 =
        new("Mixolydian b6", ScaleFamily.MixolydianFlat6, new[] { 0, 2, 4, 5, 7, 8, 10 });
    public static readonly ScaleFormula HalfDiminished =
        new("Half-Diminished (Locrian #2)", ScaleFamily.HalfDiminished, new[] { 0, 2, 3, 5, 6, 8, 10 });
    public static readonly ScaleFormula PhrygianDominant =
        new("Phrygian Dominant", ScaleFamily.PhrygianDominant, new[] { 0, 1, 4, 5, 7, 8, 10 });
    public static readonly ScaleFormula Altered =
        new("Altered", ScaleFamily.Altered, new[] { 0, 1, 3, 4, 6, 8, 10 });

    public static readonly IReadOnlyList<ScaleFormula> All = new[]
    {
        Major, NaturalMinor, HarmonicMinor, MelodicMinor,
        Ionian, Dorian, Phrygian, Lydian, Mixolydian, Aeolian, Locrian,
        PentatonicMajor, PentatonicMinor, Blues, WholeTone,
        DiminishedHalfWhole, DiminishedWholeHalf,
        BebopDominant, LydianDominant, LydianAugmented,
        MixolydianFlat6, HalfDiminished, PhrygianDominant, Altered
    };

    public static ScaleFormula? ByName(string name) =>
        All.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}

public sealed class Scale
{
    public Note Root { get; }
    public ScaleFormula Formula { get; }

    public Scale(Note root, ScaleFormula formula)
    {
        Root = root;
        Formula = formula;
    }

    public static Scale Create(NoteName root, ScaleFormula formula, int octave = 4)
        => new(new Note(root, octave), formula);

    public IReadOnlyList<Note> Notes =>
        Formula.Intervals.Select(i => Root.Transpose(i)).ToList();

    public IReadOnlySet<int> PitchClassesInOctave
    {
        get
        {
            var set = new HashSet<int>();
            foreach (var i in Formula.Intervals)
                set.Add(((Root.PitchClass + i) % 12 + 12) % 12);
            return set;
        }
    }

    public IReadOnlyList<Note> NotesInRange(Note low, Note high)
    {
        if (high.MidiNumber < low.MidiNumber)
            (low, high) = (high, low);
        var result = new List<Note>();
        int pc = Root.PitchClass;
        for (int midi = low.MidiNumber; midi <= high.MidiNumber; midi++)
        {
            if (Formula.IntervalSet.Contains(((midi - pc) % 12 + 12) % 12))
                result.Add(new Note(midi));
        }
        return result;
    }

    public bool Contains(Note note) =>
        Formula.IntervalSet.Contains(((note.PitchClass - Root.PitchClass) % 12 + 12) % 12);

    public override string ToString() => $"{Root} {Formula.Name}";
}
