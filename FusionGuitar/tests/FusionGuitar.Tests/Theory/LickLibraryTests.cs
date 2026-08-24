using FusionGuitar.Web.Theory;
using Xunit;

namespace FusionGuitar.Tests.Theory;

public class LickLibraryTests
{
    [Fact]
    public void Library_HasLicksAcrossStyles()
    {
        Assert.NotEmpty(LickLibrary.All);
        Assert.Contains(LickLibrary.All, l => l.Style == "Dorian");
        Assert.Contains(LickLibrary.All, l => l.Style == "Blues");
        Assert.Contains(LickLibrary.All, l => l.Style == "Bebop");
        Assert.Contains(LickLibrary.All, l => l.Style == "Fusion");
    }

    [Fact]
    public void EveryLick_HasNotes()
    {
        foreach (var l in LickLibrary.All)
        {
            Assert.NotNull(l.Notes);
            Assert.NotEmpty(l.Notes);
        }
    }

    [Fact]
    public void ByStyle_Filters()
    {
        var dorian = LickLibrary.ByStyle("Dorian");
        Assert.NotEmpty(dorian);
        Assert.All(dorian, l => Assert.Equal("Dorian", l.Style));
    }

    [Fact]
    public void ByStyle_All_ReturnsAll()
    {
        Assert.Equal(LickLibrary.All.Count, LickLibrary.ByStyle("全部").Count);
        Assert.Equal(LickLibrary.All.Count, LickLibrary.ByStyle("").Count);
    }

    [Fact]
    public void ByName_ResolvesOrNull()
    {
        Assert.NotNull(LickLibrary.ByName("Dorian 上行动机"));
        Assert.Null(LickLibrary.ByName("nope"));
    }
}
