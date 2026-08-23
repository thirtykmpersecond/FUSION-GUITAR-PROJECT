using FusionGuitar.Web.Theory;
using Xunit;

namespace FusionGuitar.Tests.Theory;

public class ScaleTests
{
    [Fact]
    public void CMajor_HasCorrectPitches()
    {
        var scale = new Scale(Note.Parse("C4"), ScaleFormulas.Major);
        Assert.Equal(new[] { 60, 62, 64, 65, 67, 69, 71 },
            scale.Notes.Select(n => n.MidiNumber));
    }

    [Fact]
    public void CMajor_ContainsAllWhiteKeys()
    {
        var scale = new Scale(Note.Parse("C4"), ScaleFormulas.Major);
        var pcs = scale.PitchClassesInOctave;
        Assert.Equal(new HashSet<int> { 0, 2, 4, 5, 7, 9, 11 }, pcs);
    }

    [Theory]
    [InlineData(NoteName.C, ScaleFamily.Major, new[] { 0, 2, 4, 5, 7, 9, 11 })]
    [InlineData(NoteName.A, ScaleFamily.NaturalMinor, new[] { 9, 11, 0, 2, 4, 5, 7 })]
    [InlineData(NoteName.G, ScaleFamily.Mixolydian, new[] { 7, 9, 11, 0, 2, 4, 5 })]
    [InlineData(NoteName.E, ScaleFamily.Blues, new[] { 4, 7, 9, 10, 11, 2 })]
    public void PitchClasses_MatchFormula(NoteName root, ScaleFamily family, int[] expected)
    {
        var formula = ScaleFormulas.All.First(f => f.Family == family);
        var scale = new Scale(new Note(root, 4), formula);
        Assert.Equal(new HashSet<int>(expected), scale.PitchClassesInOctave);
    }

    [Fact]
    public void NotesInRange_A4toA5_Pentatonic()
    {
        var scale = new Scale(Note.Parse("A4"), ScaleFormulas.PentatonicMinor);
        var range = scale.NotesInRange(Note.Parse("A4"), Note.Parse("A5"));
        Assert.Equal(new[] { 69, 72, 74, 76, 79, 81 }, range.Select(n => n.MidiNumber));
    }

    [Fact]
    public void AllModes_AreSevenNotes()
    {
        var modes = new[]
        {
            ScaleFormulas.Ionian, ScaleFormulas.Dorian, ScaleFormulas.Phrygian,
            ScaleFormulas.Lydian, ScaleFormulas.Mixolydian, ScaleFormulas.Aeolian,
            ScaleFormulas.Locrian
        };
        Assert.All(modes, m => Assert.Equal(7, m.Intervals.Count));
    }
}
