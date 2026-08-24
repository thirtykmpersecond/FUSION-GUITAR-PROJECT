using FusionGuitar.Web.Theory;
using Xunit;

namespace FusionGuitar.Tests.Theory;

public class ChordNameTests
{
    [Theory]
    [InlineData("Cmaj7", NoteName.C, ChordQuality.Major7)]
    [InlineData("Gm7", NoteName.G, ChordQuality.Minor7)]
    [InlineData("C7", NoteName.C, ChordQuality.Dominant7)]
    [InlineData("Am7b5", NoteName.A, ChordQuality.HalfDiminished7)]
    [InlineData("F#m7", NoteName.FSharp, ChordQuality.Minor7)]
    [InlineData("Bbm7", NoteName.ASharp, ChordQuality.Minor7)]
    [InlineData("Dm9", NoteName.D, ChordQuality.Minor9)]
    [InlineData("Cmaj9", NoteName.C, ChordQuality.Major9)]
    [InlineData("G7#5", NoteName.G, ChordQuality.Augmented7)]
    [InlineData("Em", NoteName.E, ChordQuality.Minor)]
    [InlineData("C", NoteName.C, ChordQuality.Major)]
    public void Parse_VariousSymbols_ReturnsCorrectChord(string symbol, NoteName root, ChordQuality quality)
    {
        var chord = ChordName.Parse(symbol);
        Assert.NotNull(chord);
        Assert.Equal(root, chord!.Root.Name);
        Assert.Equal(quality, chord.Formula.Quality);
    }

    [Theory]
    [InlineData("")]
    [InlineData("H7")]
    [InlineData("  ")]
    public void Parse_Invalid_ReturnsNull(string symbol)
    {
        Assert.Null(ChordName.Parse(symbol));
    }
}
