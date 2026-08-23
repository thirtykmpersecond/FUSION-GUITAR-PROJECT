using FusionGuitar.Web.Theory;
using Xunit;

namespace FusionGuitar.Tests.Theory;

public class FretboardTests
{
    private static readonly GuitarFretboard Fb = new();

    [Fact]
    public void StandardTuning_OpenStringsAreCorrect()
    {
        Assert.Equal(40, Fb.NoteAt(0, 0).MidiNumber); // low E2
        Assert.Equal(45, Fb.NoteAt(1, 0).MidiNumber); // A2
        Assert.Equal(50, Fb.NoteAt(2, 0).MidiNumber); // D3
        Assert.Equal(55, Fb.NoteAt(3, 0).MidiNumber); // G3
        Assert.Equal(59, Fb.NoteAt(4, 0).MidiNumber); // B3
        Assert.Equal(64, Fb.NoteAt(5, 0).MidiNumber); // E4
    }

    [Fact]
    public void TwelfthFret_IsOctave()
    {
        for (int s = 0; s < 6; s++)
        {
            var open = Fb.NoteAt(s, 0);
            var at12 = Fb.NoteAt(s, 12);
            Assert.Equal(open.PitchClass, at12.PitchClass);
            Assert.Equal(open.MidiNumber + 12, at12.MidiNumber);
        }
    }

    [Fact]
    public void FindScale_CMajor_HasRootsOnEveryString()
    {
        var scale = new Scale(Note.Parse("C4"), ScaleFormulas.Major);
        var notes = Fb.FindScale(scale);
        foreach (var s in Enumerable.Range(0, 6))
        {
            Assert.Contains(notes, n => n.StringIndex == s && n.IsRoot);
        }
    }

    [Fact]
    public void FindChord_CMajor_HasCorrectPitches()
    {
        var chord = Chord.Create(NoteName.C, ChordQuality.Major);
        var notes = Fb.FindChord(chord, 0, 12);
        var expectedPcs = new HashSet<int> { 0, 4, 7 };
        Assert.All(notes, n => Assert.Contains(n.Note.PitchClass, expectedPcs));
    }
}
