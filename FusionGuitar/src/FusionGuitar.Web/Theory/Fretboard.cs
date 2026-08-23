namespace FusionGuitar.Web.Theory;

public readonly record struct GuitarTuning(string Name, IReadOnlyList<NoteName> Strings)
{
    public int StringCount => Strings.Count;

    public static readonly GuitarTuning Standard = new(
        "Standard",
        new[] { NoteName.E, NoteName.A, NoteName.D, NoteName.G, NoteName.B, NoteName.E });

    public static readonly GuitarTuning DropD = new(
        "Drop D",
        new[] { NoteName.D, NoteName.A, NoteName.D, NoteName.G, NoteName.B, NoteName.E });

    public static readonly GuitarTuning DADGAD = new(
        "DADGAD",
        new[] { NoteName.D, NoteName.A, NoteName.D, NoteName.G, NoteName.A, NoteName.D });
}

public sealed record FretNote(int StringIndex, int Fret, Note Note, bool IsRoot, string? Role = null);

public sealed class GuitarFretboard
{
    public GuitarTuning Tuning { get; }
    public int Frets { get; }
    public int StartOctave { get; }

    public GuitarFretboard(GuitarTuning? tuning = null, int frets = 22, int startOctave = 1)
    {
        Tuning = tuning ?? GuitarTuning.Standard;
        Frets = frets;
        StartOctave = startOctave;
    }

    public Note NoteAt(int stringIndex, int fret)
    {
        if (stringIndex < 0 || stringIndex >= Tuning.StringCount)
            throw new ArgumentOutOfRangeException(nameof(stringIndex));
        if (fret < 0 || fret > Frets)
            throw new ArgumentOutOfRangeException(nameof(fret));

        int octaveForString = stringIndex switch
        {
            // Standard tuning: low E (6th)=E2, A=A2, D=D3, G=G3, B=B3, high E=E4
            // stringIndex 0 = low E
            0 => 2,
            1 => 2,
            2 => 3,
            3 => 3,
            4 => 3,
            5 => 4,
            _ => StartOctave
        };

        // Override for non-standard tunings in a generic way: use lowest string octave = 2
        if (!Tuning.Equals(GuitarTuning.Standard))
            octaveForString = StartOctave + 1;

        var open = new Note(Tuning.Strings[stringIndex], octaveForString);
        return open.Transpose(fret);
    }

    public IReadOnlyList<FretNote> FindScale(Scale scale, int startFret = 0, int? endFret = null)
    {
        endFret ??= Frets;
        var rootPc = scale.Root.PitchClass;
        var result = new List<FretNote>();
        for (int s = 0; s < Tuning.StringCount; s++)
        {
            for (int f = startFret; f <= endFret; f++)
            {
                var n = NoteAt(s, f);
                if (scale.Contains(n))
                {
                    result.Add(new FretNote(
                        s, f, n,
                        n.PitchClass == rootPc,
                        n.PitchClass == rootPc ? "R" : null));
                }
            }
        }
        return result;
    }

    public IReadOnlyList<FretNote> FindChord(Chord chord, int startFret = 0, int? endFret = null)
    {
        endFret ??= Frets;
        var rootPc = chord.Root.PitchClass;
        var result = new List<FretNote>();
        for (int s = 0; s < Tuning.StringCount; s++)
        {
            for (int f = startFret; f <= endFret; f++)
            {
                var n = NoteAt(s, f);
                if (chord.PitchClasses.Contains(n.PitchClass))
                {
                    result.Add(new FretNote(s, f, n, n.PitchClass == rootPc));
                }
            }
        }
        return result;
    }
}
