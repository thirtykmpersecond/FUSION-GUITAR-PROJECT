using FusionGuitar.Web.Theory;
using Xunit;

namespace FusionGuitar.Tests.Theory;

public class AdvancedScaleTests
{
    [Theory]
    [InlineData(ScaleFamily.LydianDominant, new[] { 0, 2, 4, 6, 7, 9, 10 })]
    [InlineData(ScaleFamily.LydianAugmented, new[] { 0, 2, 4, 6, 8, 9, 11 })]
    [InlineData(ScaleFamily.MixolydianFlat6, new[] { 0, 2, 4, 5, 7, 8, 10 })]
    [InlineData(ScaleFamily.HalfDiminished, new[] { 0, 2, 3, 5, 6, 8, 10 })]
    [InlineData(ScaleFamily.PhrygianDominant, new[] { 0, 1, 4, 5, 7, 8, 10 })]
    public void MelodicHarmonicMinorModes_HaveCorrectIntervals(ScaleFamily family, int[] expected)
    {
        var formula = ScaleFormulas.All.First(f => f.Family == family);
        Assert.Equal(expected, formula.Intervals);
    }

    [Fact]
    public void LydianDominant_IsFourthModeOfMelodicMinor()
    {
        // C melodic minor's 4th mode starts on F = F Lydian Dominant
        var c = new Scale(Note.Parse("C4"), ScaleFormulas.MelodicMinor);
        var f = new Scale(Note.Parse("F4"), ScaleFormulas.LydianDominant);
        Assert.Equal(c.PitchClassesInOctave.OrderBy(x => x).ToArray(),
                     f.PitchClassesInOctave.OrderBy(x => x).ToArray());
    }

    [Fact]
    public void Altered_IsSeventhModeOfMelodicMinor()
    {
        // C melodic minor's 7th mode is B Altered
        var c = new Scale(Note.Parse("C4"), ScaleFormulas.MelodicMinor);
        var b = new Scale(Note.Parse("B4"), ScaleFormulas.Altered);
        Assert.Equal(c.PitchClassesInOctave.OrderBy(x => x).ToArray(),
                     b.PitchClassesInOctave.OrderBy(x => x).ToArray());
    }

    [Fact]
    public void MixolydianFlat6_IsFifthModeOfMelodicMinor()
    {
        var c = new Scale(Note.Parse("C4"), ScaleFormulas.MelodicMinor);
        var g = new Scale(Note.Parse("G4"), ScaleFormulas.MixolydianFlat6);
        Assert.Equal(c.PitchClassesInOctave.OrderBy(x => x).ToArray(),
                     g.PitchClassesInOctave.OrderBy(x => x).ToArray());
    }

    [Fact]
    public void PhrygianDominant_IsFifthModeOfHarmonicMinor()
    {
        var a = new Scale(Note.Parse("A4"), ScaleFormulas.HarmonicMinor);
        var e = new Scale(Note.Parse("E4"), ScaleFormulas.PhrygianDominant);
        Assert.Equal(a.PitchClassesInOctave.OrderBy(x => x).ToArray(),
                     e.PitchClassesInOctave.OrderBy(x => x).ToArray());
    }

    [Fact]
    public void AllFormulas_AreUniqueByName()
    {
        var names = ScaleFormulas.All.Select(f => f.Name).ToList();
        Assert.Equal(names.Distinct().Count(), names.Count);
    }
}
