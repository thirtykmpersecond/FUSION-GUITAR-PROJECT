namespace FusionGuitar.Web.Theory;

public enum NoteName
{
    C = 0,
    CSharp = 1,
    D = 2,
    DSharp = 3,
    E = 4,
    F = 5,
    FSharp = 6,
    G = 7,
    GSharp = 8,
    A = 9,
    ASharp = 10,
    B = 11
}

public enum Accidental
{
    DoubleFlat = -2,
    Flat = -1,
    Natural = 0,
    Sharp = 1,
    DoubleSharp = 2
}

public enum IntervalQuality
{
    Diminished,
    Minor,
    Perfect,
    Major,
    Augmented
}

public enum ChordQuality
{
    Major,
    Minor,
    Augmented,
    Diminished,
    Sus2,
    Sus4,
    Major7,
    Dominant7,
    Minor7,
    MinorMajor7,
    Diminished7,
    HalfDiminished7,
    AugmentedMaj7,
    Augmented7,
    Add9,
    Major9,
    Dominant9,
    Minor9
}

public enum ScaleFamily
{
    Major,
    NaturalMinor,
    HarmonicMinor,
    MelodicMinor,
    Ionian,
    Dorian,
    Phrygian,
    Lydian,
    Mixolydian,
    Aeolian,
    Locrian,
    PentatonicMajor,
    PentatonicMinor,
    Blues,
    WholeTone,
    DiminishedHalfWhole,
    DiminishedWholeHalf,
    BebopDominant,
    LydianDominant,
    LydianAugmented,
    MixolydianFlat6,
    HalfDiminished,
    PhrygianDominant,
    Altered
}
