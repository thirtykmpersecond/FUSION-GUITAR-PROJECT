using FusionGuitar.Web.Components.Common;
using Xunit;

namespace FusionGuitar.Tests;

public class LessonParserTests
{
    [Fact]
    public void PlainMarkdown_ProducesSingleHtmlSegment()
    {
        var segs = LessonParser.Parse("# Title\n\nHello **world**.");
        Assert.Single(segs);
        Assert.True(segs[0].IsHtml);
        Assert.Contains("<strong>world</strong>", segs[0].Content);
    }

    [Fact]
    public void Directive_WithQuotedArgs_Parses()
    {
        var segs = LessonParser.Parse(":::fretboard root=\"C\" scale=\"Major\" frets=12");
        Assert.Single(segs);
        Assert.False(segs[0].IsHtml);
        Assert.Equal("fretboard", segs[0].Component);
        Assert.Equal("C", LessonParser.AsString(segs[0].Args["root"]));
        Assert.Equal(12, LessonParser.AsInt(segs[0].Args["frets"], 0));
    }

    [Fact]
    public void MixedContent_SplitsMarkdownAndDirectives()
    {
        var md = "Intro text\n\n:::piano root=\"A\" type=\"chord\" name=\"Minor7\"\n\nOutro";
        var segs = LessonParser.Parse(md);
        Assert.Equal(3, segs.Count);
        Assert.True(segs[0].IsHtml);
        Assert.False(segs[1].IsHtml);
        Assert.Equal("piano", segs[1].Component);
        Assert.True(segs[2].IsHtml);
        Assert.Equal("A", LessonParser.AsString(segs[1].Args["root"]));
        Assert.Equal("Minor7", LessonParser.AsString(segs[1].Args["name"]));
    }

    [Fact]
    public void ChordDirective_Parses()
    {
        var seg = Assert.Single(LessonParser.Parse(":::chord root=\"G\" quality=\"Major\""));
        Assert.Equal("chord", seg.Component);
        Assert.Equal("G", LessonParser.AsString(seg.Args["root"]));
        Assert.Equal("Major", LessonParser.AsString(seg.Args["quality"]));
    }

    [Fact]
    public void FretboardDirective_ParsesFretsAsInt()
    {
        var seg = Assert.Single(LessonParser.Parse(":::fretboard root=\"A\" scale=\"Blues\" frets=15"));
        Assert.Equal("fretboard", seg.Component);
        Assert.Equal("A", LessonParser.AsString(seg.Args["root"]));
        Assert.Equal("Blues", LessonParser.AsString(seg.Args["scale"]));
        Assert.Equal(15, LessonParser.AsInt(seg.Args["frets"], 0));
    }

    [Fact]
    public void Directive_QuotedValueWithSpaces_PreservesWholeValue()
    {
        // Regression: Tokenizer used to split at the first space inside a
        // quoted value, mangling notes="..." / scale="Pentatonic Minor" and
        // causing VexFlow "Invalid key name" errors.
        var seg = Assert.Single(LessonParser.Parse(
            ":::staff clef=\"treble\" key=\"Am\" notes=\"a/4/8 c/5/8 d/5/8\""));
        Assert.Equal("staff", seg.Component);
        Assert.Equal("treble", LessonParser.AsString(seg.Args["clef"]));
        Assert.Equal("Am", LessonParser.AsString(seg.Args["key"]));
        Assert.Equal("a/4/8 c/5/8 d/5/8", LessonParser.AsString(seg.Args["notes"]));
    }

    [Fact]
    public void Directive_MultipleQuotedValuesWithSpaces_ParsesAll()
    {
        var seg = Assert.Single(LessonParser.Parse(
            ":::staff clef=\"treble\" key=\"C\" time=\"4/4\" " +
            "notes=\"c/4/q d/4/8\" tab=\"6:8 5:10 4:12\""));
        Assert.Equal("C", LessonParser.AsString(seg.Args["key"]));
        Assert.Equal("4/4", LessonParser.AsString(seg.Args["time"]));
        Assert.Equal("c/4/q d/4/8", LessonParser.AsString(seg.Args["notes"]));
        Assert.Equal("6:8 5:10 4:12", LessonParser.AsString(seg.Args["tab"]));
    }

    [Fact]
    public void Directive_ScaleNameWithSpace_Preserves()
    {
        var seg = Assert.Single(LessonParser.Parse(
            ":::fretboard root=\"A\" scale=\"Pentatonic Minor\" frets=12"));
        Assert.Equal("Pentatonic Minor", LessonParser.AsString(seg.Args["scale"]));
    }
}
