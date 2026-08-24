using FusionGuitar.Web.Theory;
using Xunit;

namespace FusionGuitar.Tests.Theory;

public class ProgressionTests
{
    [Fact]
    public void ParseSteps_SimpleList()
    {
        var steps = Progressions.ParseSteps("Dm7,G7,Cmaj7");
        Assert.Equal(3, steps.Count);
        Assert.Equal("Dm7", steps[0].ChordSymbol);
        Assert.Equal("Cmaj7", steps[2].ChordSymbol);
        Assert.All(steps, s => Assert.Equal(1, s.Bars));
    }

    [Fact]
    public void ParseSteps_WithBarCounts()
    {
        var steps = Progressions.ParseSteps("Dm7:2,G7:1,Cmaj7");
        Assert.Equal(2, steps[0].Bars);
        Assert.Equal(1, steps[1].Bars);
    }

    [Fact]
    public void ParseSteps_DefaultBars_Applied()
    {
        var steps = Progressions.ParseSteps("Dm7,G7", defaultBars: 2);
        Assert.All(steps, s => Assert.Equal(2, s.Bars));
    }

    [Fact]
    public void ParseSteps_IgnoresEmpty()
    {
        var steps = Progressions.ParseSteps("Dm7,,G7, ,Cmaj7");
        Assert.Equal(3, steps.Count);
    }

    [Fact]
    public void Library_HasKnownProgressions()
    {
        Assert.Contains(Progressions.All, p => p.Name.Contains("ii"));
        Assert.Contains(Progressions.All, p => p.Name.Contains("Autumn"));
        Assert.Contains(Progressions.All, p => p.Name.Contains("Rhythm"));
        Assert.Contains(Progressions.All, p => p.Name.Contains("So What"));
        Assert.Contains(Progressions.All, p => p.Name.Contains("Maiden"));
        Assert.Contains(Progressions.All, p => p.Name.Contains("C Jam"));
    }
}
