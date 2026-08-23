using FusionGuitar.Web.Theory;
using Xunit;

namespace FusionGuitar.Tests.Theory;

public class ChordTests
{
    [Theory]
    [InlineData(NoteName.C, ChordQuality.Major, new[] { 0, 4, 7 })]
    [InlineData(NoteName.C, ChordQuality.Minor, new[] { 0, 3, 7 })]
    [InlineData(NoteName.C, ChordQuality.Dominant7, new[] { 0, 4, 7, 10 })]
    [InlineData(NoteName.C, ChordQuality.Major7, new[] { 0, 4, 7, 11 })]
    [InlineData(NoteName.C, ChordQuality.HalfDiminished7, new[] { 0, 3, 6, 10 })]
    [InlineData(NoteName.C, ChordQuality.Diminished7, new[] { 0, 3, 6, 9 })]
    public void Chord_IntervalsMatch(NoteName root, ChordQuality q, int[] offsets)
    {
        var chord = Chord.Create(root, q);
        var expected = offsets.Select(o => (int)root + o).Select(p => ((p % 12) + 12) % 12).ToHashSet();
        Assert.Equal(expected, chord.PitchClasses);
    }

    [Fact]
    public void Inversion_MovesBassUp()
    {
        var cmaj7 = Chord.Create(NoteName.C, ChordQuality.Major7);
        var first = cmaj7.Inversion(1);
        Assert.Equal(NoteName.E, first[0].Name);
        Assert.Equal(NoteName.C, first[3].Name);
        Assert.True(first[3].Octave > first[0].Octave);
    }

    [Fact]
    public void Name_ContainsSymbol()
    {
        Assert.Equal("Cmaj7", Chord.Create(NoteName.C, ChordQuality.Major7).Name);
        Assert.Equal("Am7", Chord.Create(NoteName.A, ChordQuality.Minor7).Name);
    }
}
