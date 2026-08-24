namespace FusionGuitar.Web.Theory;

/// <summary>Parses compact chord symbols like "Cmaj7", "Gm7", "Am7b5", "Dm9".</summary>
public static class ChordName
{
    public static Chord? Parse(string name)
    {
        var text = name?.Trim() ?? "";
        if (text.Length == 0) return null;
        var rootChar = char.ToUpperInvariant(text[0]);
        if (rootChar < 'A' || rootChar > 'G') return null;

        var root = rootChar switch
        {
            'C' => NoteName.C, 'D' => NoteName.D, 'E' => NoteName.E,
            'F' => NoteName.F, 'G' => NoteName.G, 'A' => NoteName.A, 'B' => NoteName.B,
            _ => NoteName.C
        };

        int i = 1;
        if (i < text.Length && (text[i] == '#' || text[i] == 'b'))
        {
            root = root + (text[i] == '#' ? 1 : -1);
            root = (NoteName)(((int)root % 12 + 12) % 12);
            i++;
        }

        var suffix = text[i..].ToLowerInvariant();
        var quality = suffix switch
        {
            "" or "maj" => ChordQuality.Major,
            "m" or "min" => ChordQuality.Minor,
            "maj7" => ChordQuality.Major7,
            "7" => ChordQuality.Dominant7,
            "m7" => ChordQuality.Minor7,
            "mmaj7" => ChordQuality.MinorMajor7,
            "dim" => ChordQuality.Diminished,
            "dim7" => ChordQuality.Diminished7,
            "m7b5" or "ø7" => ChordQuality.HalfDiminished7,
            "maj7#5" => ChordQuality.AugmentedMaj7,
            "7#5" => ChordQuality.Augmented7,
            "aug" => ChordQuality.Augmented,
            "add9" => ChordQuality.Add9,
            "maj9" => ChordQuality.Major9,
            "9" => ChordQuality.Dominant9,
            "m9" => ChordQuality.Minor9,
            _ => ChordQuality.Major7
        };
        return Chord.Create(root, quality);
    }
}
