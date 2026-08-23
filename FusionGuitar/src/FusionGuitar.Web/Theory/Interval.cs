namespace FusionGuitar.Web.Theory;

public readonly struct Interval : IEquatable<Interval>
{
    public int Number { get; }
    public IntervalQuality Quality { get; }
    public int Semitones { get; }

    private Interval(int number, IntervalQuality quality, int semitones)
    {
        Number = number;
        Quality = quality;
        Semitones = semitones;
    }

    public static Interval FromSemitones(int semitones)
    {
        var normalized = ((semitones % 12) + 12) % 12;
        var (number, quality) = normalized switch
        {
            0 => (1, IntervalQuality.Perfect),
            1 => (2, IntervalQuality.Minor),
            2 => (2, IntervalQuality.Major),
            3 => (3, IntervalQuality.Minor),
            4 => (3, IntervalQuality.Major),
            5 => (4, IntervalQuality.Perfect),
            6 => (5, IntervalQuality.Diminished),
            7 => (5, IntervalQuality.Perfect),
            8 => (6, IntervalQuality.Minor),
            9 => (6, IntervalQuality.Major),
            10 => (7, IntervalQuality.Minor),
            11 => (7, IntervalQuality.Major),
            _ => throw new InvalidOperationException()
        };
        return new Interval(number, quality, semitones);
    }

    public static readonly Interval Unison = new(1, IntervalQuality.Perfect, 0);
    public static readonly Interval MinorSecond = new(2, IntervalQuality.Minor, 1);
    public static readonly Interval MajorSecond = new(2, IntervalQuality.Major, 2);
    public static readonly Interval MinorThird = new(3, IntervalQuality.Minor, 3);
    public static readonly Interval MajorThird = new(3, IntervalQuality.Major, 4);
    public static readonly Interval PerfectFourth = new(4, IntervalQuality.Perfect, 5);
    public static readonly Interval Tritone = new(4, IntervalQuality.Augmented, 6);
    public static readonly Interval DiminishedFifth = new(5, IntervalQuality.Diminished, 6);
    public static readonly Interval AugmentedFourth = new(4, IntervalQuality.Augmented, 6);
    public static readonly Interval PerfectFifth = new(5, IntervalQuality.Perfect, 7);
    public static readonly Interval MinorSixth = new(6, IntervalQuality.Minor, 8);
    public static readonly Interval MajorSixth = new(6, IntervalQuality.Major, 9);
    public static readonly Interval DiminishedSeventh = new(7, IntervalQuality.Diminished, 9);
    public static readonly Interval MinorSeventh = new(7, IntervalQuality.Minor, 10);
    public static readonly Interval MajorSeventh = new(7, IntervalQuality.Major, 11);
    public static readonly Interval Octave = new(8, IntervalQuality.Perfect, 12);
    public static readonly Interval MinorNinth = new(9, IntervalQuality.Minor, 13);
    public static readonly Interval MajorNinth = new(9, IntervalQuality.Major, 14);
    public static readonly Interval PerfectEleventh = new(11, IntervalQuality.Perfect, 17);
    public static readonly Interval SharpEleventh = new(11, IntervalQuality.Augmented, 18);
    public static readonly Interval FlatThirteenth = new(13, IntervalQuality.Minor, 20);
    public static readonly Interval MajorThirteenth = new(13, IntervalQuality.Major, 21);

    public Interval Invert() => FromSemitones(12 - ((Semitones % 12 + 12) % 12));

    public bool Equals(Interval other) => Semitones == other.Semitones;
    public override bool Equals(object? obj) => obj is Interval i && Equals(i);
    public override int GetHashCode() => Semitones;
    public static bool operator ==(Interval a, Interval b) => a.Equals(b);
    public static bool operator !=(Interval a, Interval b) => !a.Equals(b);

    public override string ToString()
    {
        var q = Quality switch
        {
            IntervalQuality.Diminished => "d",
            IntervalQuality.Minor => "m",
            IntervalQuality.Perfect => "P",
            IntervalQuality.Major => "M",
            IntervalQuality.Augmented => "A",
            _ => "?"
        };
        return $"{q}{Number}";
    }
}
