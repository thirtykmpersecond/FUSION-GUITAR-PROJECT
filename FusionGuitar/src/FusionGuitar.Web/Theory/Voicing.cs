namespace FusionGuitar.Web.Theory;

public sealed record Fingering(int StringIndex, int Fret, int? Finger = null, bool Muted = false, bool Open = false);
public sealed record Voicing(
    string Name,
    NoteName Root,
    ChordQuality Quality,
    IReadOnlyList<Fingering> Fingerings,
    int BaseFret = 0,
    string? Notes = null);

public static class Voicings
{
    // StringIndex convention: 0 = low E, 5 = high E (matches GuitarFretboard).
    private static readonly List<Voicing> Open = new()
    {
        new("C", NoteName.C, ChordQuality.Major, new Fingering[]
        {
            new(0, 0, Muted: true),
            new(1, 3, 3), new(2, 2, 2), new(3, 0, Open: true),
            new(4, 1, 1), new(5, 0, Open: true)
        }, Notes: "x 3 2 0 1 0"),
        new("D", NoteName.D, ChordQuality.Major, new Fingering[]
        {
            new(0, 0, Muted: true), new(1, 0, Muted: true),
            new(2, 0, Open: true), new(3, 2, 1), new(4, 3, 3), new(5, 2, 2)
        }, Notes: "x x 0 2 3 2"),
        new("E", NoteName.E, ChordQuality.Major, new Fingering[]
        {
            new(0, 0, Open: true), new(1, 2, 2), new(2, 2, 3),
            new(3, 1, 1), new(4, 0, Open: true), new(5, 0, Open: true)
        }, Notes: "0 2 2 1 0 0"),
        new("G", NoteName.G, ChordQuality.Major, new Fingering[]
        {
            new(0, 3, 2), new(1, 3, 3), new(2, 0, Open: true),
            new(3, 0, Open: true), new(4, 0, Open: true), new(5, 3, 4)
        }, Notes: "3 2 0 0 0 3"),
        new("A", NoteName.A, ChordQuality.Major, new Fingering[]
        {
            new(0, 0, Muted: true),
            new(1, 0, Open: true), new(2, 2, 1), new(3, 2, 2), new(4, 2, 3),
            new(5, 0, Open: true)
        }, Notes: "x 0 2 2 2 0"),

        new("Am", NoteName.A, ChordQuality.Minor, new Fingering[]
        {
            new(0, 0, Muted: true),
            new(1, 0, Open: true), new(2, 2, 2), new(3, 2, 3), new(4, 1, 1),
            new(5, 0, Open: true)
        }, Notes: "x 0 2 2 1 0"),
        new("Em", NoteName.E, ChordQuality.Minor, new Fingering[]
        {
            new(0, 0, Open: true), new(1, 2, 2), new(2, 2, 3),
            new(3, 0, Open: true), new(4, 0, Open: true), new(5, 0, Open: true)
        }, Notes: "0 2 2 0 0 0"),
        new("Dm", NoteName.D, ChordQuality.Minor, new Fingering[]
        {
            new(0, 0, Muted: true), new(1, 0, Muted: true),
            new(2, 0, Open: true), new(3, 2, 2), new(4, 3, 3), new(5, 1, 1)
        }, Notes: "x x 0 2 3 1"),

        new("A7", NoteName.A, ChordQuality.Dominant7, new Fingering[]
        {
            new(0, 0, Muted: true),
            new(1, 0, Open: true), new(2, 2, 1), new(3, 0, Open: true), new(4, 2, 2),
            new(5, 0, Open: true)
        }, Notes: "x 0 2 0 2 0"),
        new("D7", NoteName.D, ChordQuality.Dominant7, new Fingering[]
        {
            new(0, 0, Muted: true), new(1, 0, Muted: true),
            new(2, 0, Open: true), new(3, 2, 1), new(4, 1, 2), new(5, 2, 3)
        }, Notes: "x x 0 2 1 2"),
        new("E7", NoteName.E, ChordQuality.Dominant7, new Fingering[]
        {
            new(0, 0, Open: true), new(1, 2, 1), new(2, 0, Open: true),
            new(3, 0, Open: true), new(4, 0, Open: true), new(5, 0, Open: true)
        }, Notes: "0 2 0 0 0 0"),
        new("C7", NoteName.C, ChordQuality.Dominant7, new Fingering[]
        {
            new(0, 0, Muted: true),
            new(1, 3, 3), new(2, 2, 2), new(3, 3, 4), new(4, 1, 1), new(5, 0, Open: true)
        }, Notes: "x 3 2 3 1 0"),
    };

    public static Voicing? OpenFor(Chord chord)
        => Open.FirstOrDefault(v => v.Root == chord.Root.Name && v.Quality == chord.Formula.Quality);

    public static Voicing? ByName(string name, NoteName root)
        => Open.FirstOrDefault(v =>
            v.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && v.Root == root);

    public static IReadOnlyList<Voicing> All => Open;
}
