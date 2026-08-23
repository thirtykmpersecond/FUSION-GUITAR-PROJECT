namespace FusionGuitar.Web.Theory;

public sealed record ChordFormula(
    string Name,
    ChordQuality Quality,
    IReadOnlyList<int> Intervals,
    string Symbol)
{
    public IReadOnlySet<int> IntervalSet { get; } = new HashSet<int>(Intervals);
}

public static class ChordFormulas
{
    public static readonly ChordFormula Major =
        new("Major", ChordQuality.Major, new[] { 0, 4, 7 }, "maj");
    public static readonly ChordFormula Minor =
        new("Minor", ChordQuality.Minor, new[] { 0, 3, 7 }, "m");
    public static readonly ChordFormula Augmented =
        new("Augmented", ChordQuality.Augmented, new[] { 0, 4, 8 }, "aug");
    public static readonly ChordFormula Diminished =
        new("Diminished", ChordQuality.Diminished, new[] { 0, 3, 6 }, "dim");
    public static readonly ChordFormula Sus2 =
        new("Sus2", ChordQuality.Sus2, new[] { 0, 2, 7 }, "sus2");
    public static readonly ChordFormula Sus4 =
        new("Sus4", ChordQuality.Sus4, new[] { 0, 5, 7 }, "sus4");

    public static readonly ChordFormula Major7 =
        new("Major 7", ChordQuality.Major7, new[] { 0, 4, 7, 11 }, "maj7");
    public static readonly ChordFormula Dominant7 =
        new("Dominant 7", ChordQuality.Dominant7, new[] { 0, 4, 7, 10 }, "7");
    public static readonly ChordFormula Minor7 =
        new("Minor 7", ChordQuality.Minor7, new[] { 0, 3, 7, 10 }, "m7");
    public static readonly ChordFormula MinorMaj7 =
        new("Minor Major 7", ChordQuality.MinorMajor7, new[] { 0, 3, 7, 11 }, "mMaj7");
    public static readonly ChordFormula Dim7 =
        new("Diminished 7", ChordQuality.Diminished7, new[] { 0, 3, 6, 9 }, "dim7");
    public static readonly ChordFormula HalfDim7 =
        new("Half-Diminished 7", ChordQuality.HalfDiminished7, new[] { 0, 3, 6, 10 }, "m7b5");
    public static readonly ChordFormula AugMaj7 =
        new("Augmented Major 7", ChordQuality.AugmentedMaj7, new[] { 0, 4, 8, 11 }, "maj7#5");
    public static readonly ChordFormula Aug7 =
        new("Augmented 7", ChordQuality.Augmented7, new[] { 0, 4, 8, 10 }, "7#5");

    public static readonly ChordFormula Add9 =
        new("Add9", ChordQuality.Add9, new[] { 0, 4, 7, 14 }, "add9");
    public static readonly ChordFormula Maj9 =
        new("Major 9", ChordQuality.Major9, new[] { 0, 4, 7, 11, 14 }, "maj9");
    public static readonly ChordFormula Dom9 =
        new("Dominant 9", ChordQuality.Dominant9, new[] { 0, 4, 7, 10, 14 }, "9");
    public static readonly ChordFormula Min9 =
        new("Minor 9", ChordQuality.Minor9, new[] { 0, 3, 7, 10, 14 }, "m9");

    public static readonly IReadOnlyList<ChordFormula> All = new[]
    {
        Major, Minor, Augmented, Diminished, Sus2, Sus4,
        Major7, Dominant7, Minor7, MinorMaj7, Dim7, HalfDim7, AugMaj7, Aug7,
        Add9, Maj9, Dom9, Min9
    };

    public static ChordFormula? ByQuality(ChordQuality q) => All.FirstOrDefault(c => c.Quality == q);
}

public sealed class Chord
{
    public Note Root { get; }
    public ChordFormula Formula { get; }
    private static readonly string[] NoteLetters =
        { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    public string Name => $"{NoteLetters[Root.PitchClass]}{Formula.Symbol}";

    public Chord(Note root, ChordFormula formula)
    {
        Root = root;
        Formula = formula;
    }

    public static Chord Create(NoteName root, ChordQuality quality, int octave = 4)
        => new(new Note(root, octave), ChordFormulas.ByQuality(quality)!);

    public IReadOnlyList<Note> Notes =>
        Formula.Intervals.Select(i => Root.Transpose(i)).ToList();

    public IReadOnlySet<int> PitchClasses
    {
        get
        {
            var set = new HashSet<int>();
            foreach (var i in Formula.Intervals)
                set.Add(((Root.PitchClass + i) % 12 + 12) % 12);
            return set;
        }
    }

    public IReadOnlyList<Note> Inversion(int inversion)
    {
        var notes = Notes;
        if (inversion < 0 || inversion >= notes.Count)
            throw new ArgumentOutOfRangeException(nameof(inversion));
        var result = new List<Note>();
        for (int i = inversion; i < notes.Count; i++) result.Add(notes[i]);
        for (int i = 0; i < inversion; i++) result.Add(notes[i].Transpose(12));
        return result;
    }

    public override string ToString() => Name;
}
