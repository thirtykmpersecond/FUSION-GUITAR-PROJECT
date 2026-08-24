using FusionGuitar.Web.Theory;
using Xunit;

namespace FusionGuitar.Tests.Theory;

public class LickBuilderTests
{
    [Fact]
    public void FromMidi_ParsesTokens()
    {
        var notes = LickBuilder.FromMidi("60:1 62:0.5 64:0.5");
        Assert.Equal(3, notes.Count);
        Assert.Equal(60, notes[0].Midi);
        Assert.Equal(1.0, notes[0].Beats);
        Assert.Equal(62, notes[1].Midi);
        Assert.Equal(0.5, notes[1].Beats);
    }

    [Fact]
    public void FromMidi_IgnoresInvalid()
    {
        var notes = LickBuilder.FromMidi("60:1 junk 64:0.5");
        Assert.Equal(2, notes.Count);
    }

    [Fact]
    public void FromFrets_ComputesMidi()
    {
        // String 1 = high E (guitar index 5, open MIDI 64), fret 0 => E4 = 64.
        var notes = LickBuilder.FromFrets("1:0:1 6:3:0.5");
        Assert.Equal(2, notes.Count);
        Assert.Equal(64, notes[0].Midi);
        Assert.Equal(5, notes[0].StringIndex);
        Assert.Equal(0, notes[0].Fret);
        // String 6 (low E, guitar index 0, open 40) fret 3 => 43 (G2).
        Assert.Equal(43, notes[1].Midi);
        Assert.Equal(0, notes[1].StringIndex);
        Assert.Equal(3, notes[1].Fret);
    }

    [Fact]
    public void FromFrets_RejectsOutOfRangeString()
    {
        var notes = LickBuilder.FromFrets("7:0:1");
        Assert.Empty(notes);
    }
}
