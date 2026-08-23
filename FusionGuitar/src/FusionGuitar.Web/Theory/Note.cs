using System.Globalization;

namespace FusionGuitar.Web.Theory;

public readonly struct Note : IEquatable<Note>, IComparable<Note>
{
    public const int MidiA4 = 69;
    public const double FrequencyA4 = 440.0;

    private static readonly string[] SharpNames =
        { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    private static readonly string[] FlatNames =
        { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

    public NoteName Name { get; }
    public int Octave { get; }

    public Note(NoteName name, int octave)
    {
        Name = name;
        Octave = octave;
    }

    public Note(int midiNumber)
    {
        if (midiNumber < 0 || midiNumber > 127)
            throw new ArgumentOutOfRangeException(nameof(midiNumber));
        Name = (NoteName)(midiNumber % 12);
        Octave = midiNumber / 12 - 1;
    }

    public int PitchClass => (int)Name;
    public int MidiNumber => (Octave + 1) * 12 + PitchClass;
    public double Frequency => FrequencyA4 * Math.Pow(2.0, (MidiNumber - MidiA4) / 12.0);

    public Note Transpose(int semitones) => new(MidiNumber + semitones);

    public static Note Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new FormatException("Empty note");

        var s = input.Trim();
        var letter = char.ToUpperInvariant(s[0]);
        if (letter < 'A' || letter > 'G')
            throw new FormatException($"Invalid note: {input}");

        int idx = letter switch
        {
            'C' => 0,
            'D' => 2,
            'E' => 4,
            'F' => 5,
            'G' => 7,
            'A' => 9,
            'B' => 11,
            _ => throw new FormatException($"Invalid note letter: {input}")
        };

        int i = 1;
        while (i < s.Length && (s[i] == '#' || s[i] == 'b' || s[i] == '♯' || s[i] == '♭'))
        {
            idx += s[i] switch
            {
                '#' or '♯' => 1,
                'b' or '♭' => -1,
                _ => 0
            };
            i++;
        }

        idx = ((idx % 12) + 12) % 12;

        int octave = 4;
        if (i < s.Length)
        {
            var oct = s[i..];
            if (!int.TryParse(oct, NumberStyles.Integer, CultureInfo.InvariantCulture, out octave))
                throw new FormatException($"Invalid octave: {input}");
        }

        return new Note((NoteName)idx, octave);
    }

    public string ToString(bool preferFlat)
    {
        var names = preferFlat ? FlatNames : SharpNames;
        return names[PitchClass] + Octave.ToString(CultureInfo.InvariantCulture);
    }

    public override string ToString() => ToString(false);

    public bool Equals(Note other) => MidiNumber == other.MidiNumber;
    public override bool Equals(object? obj) => obj is Note n && Equals(n);
    public override int GetHashCode() => MidiNumber;
    public int CompareTo(Note other) => MidiNumber.CompareTo(other.MidiNumber);

    public static bool operator ==(Note a, Note b) => a.Equals(b);
    public static bool operator !=(Note a, Note b) => !a.Equals(b);
    public static Note operator +(Note n, Interval i) => n.Transpose(i.Semitones);
    public static Note operator -(Note n, Interval i) => n.Transpose(-i.Semitones);
    public static int operator -(Note a, Note b) => a.MidiNumber - b.MidiNumber;
}
