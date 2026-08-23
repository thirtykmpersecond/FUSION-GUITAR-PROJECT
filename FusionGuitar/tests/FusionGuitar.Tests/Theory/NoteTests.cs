using FusionGuitar.Web.Theory;
using Xunit;

namespace FusionGuitar.Tests.Theory;

public class NoteTests
{
    [Fact]
    public void MidiNumber_RoundTrips()
    {
        var n = new Note(69);
        Assert.Equal(NoteName.A, n.Name);
        Assert.Equal(4, n.Octave);
        Assert.Equal(69, n.MidiNumber);
    }

    [Fact]
    public void A4_FrequencyIs440()
    {
        var a4 = new Note(NoteName.A, 4);
        Assert.Equal(440.0, a4.Frequency, 3);
    }

    [Theory]
    [InlineData("C", 60)]
    [InlineData("C4", 60)]
    [InlineData("A4", 69)]
    [InlineData("C#4", 61)]
    [InlineData("Bb4", 70)]
    [InlineData("Eb2", 39)]
    public void Parse_Works(string input, int expectedMidi)
    {
        var n = Note.Parse(input);
        Assert.Equal(expectedMidi, n.MidiNumber);
    }

    [Fact]
    public void Transpose_WrapsOctaves()
    {
        var b3 = new Note(NoteName.B, 3);
        var c4 = b3.Transpose(1);
        Assert.Equal(NoteName.C, c4.Name);
        Assert.Equal(4, c4.Octave);
    }

    [Fact]
    public void Equality_IsByMidi()
    {
        Assert.Equal(new Note(NoteName.C, 4), new Note(60));
        Assert.Equal(new Note(NoteName.B, 3).GetHashCode(), new Note(59).GetHashCode());
    }
}
