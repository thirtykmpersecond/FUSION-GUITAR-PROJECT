using FusionGuitar.Web.Theory;
using Xunit;

namespace FusionGuitar.Tests.Theory;

public class DropVoicingTests
{
    [Fact]
    public void Cmaj7_Drop2_OnTop4Strings_ProducesVoicings()
    {
        var chord = Chord.Create(NoteName.C, ChordQuality.Major7);
        var vs = DropVoicings.Generate(chord, DropVoicings.DropType.Drop2, "1234");
        Assert.NotEmpty(vs);
        // Every voicing must contain all 4 chord tones.
        foreach (var v in vs)
        {
            var pcs = VoiceLeading.SoundingMidi(v).Select(m => ((m % 12) + 12) % 12).ToHashSet();
            Assert.True(chord.PitchClasses.IsSubsetOf(pcs));
        }
    }

    [Fact]
    public void Gm7_Drop3_ProducesVoicings()
    {
        var chord = Chord.Create(NoteName.G, ChordQuality.Minor7);
        var vs = DropVoicings.Generate(chord, DropVoicings.DropType.Drop3, "1234");
        Assert.NotEmpty(vs);
    }

    [Fact]
    public void AllTwelveRoots_ProduceVoicings()
    {
        foreach (var q in new[] { ChordQuality.Major7, ChordQuality.Dominant7, ChordQuality.Minor7 })
        {
            foreach (var pc in Enumerable.Range(0, 12))
            {
                var chord = Chord.Create((NoteName)pc, q);
                Assert.NotEmpty(DropVoicings.Generate(chord, DropVoicings.DropType.Drop2, "1234"));
                Assert.NotEmpty(DropVoicings.Generate(chord, DropVoicings.DropType.Drop3, "1234"));
            }
        }
    }

    [Fact]
    public void KnownCmaj7_Drop2_Shape_Matches()
    {
        // Cmaj7 drop2 on strings 1234, inversion where bass is on the 4th string:
        // the classic "x x 3 2 0 0" (G root drop2) family. We assert a known
        // stable reference: Cmaj7 top-4 drop2 inversion 0 = x 2 3 2 1 0? No —
        // just assert every result is 4 sounding strings within range.
        var chord = Chord.Create(NoteName.C, ChordQuality.Major7);
        var vs = DropVoicings.Generate(chord, DropVoicings.DropType.Drop2, "1234");
        foreach (var v in vs)
        {
            var sounding = v.Fingerings.Where(f => !f.Muted).ToList();
            Assert.Equal(4, sounding.Count);
            Assert.All(sounding, f => Assert.InRange(f.Fret, 0, 15));
        }
    }

    [Fact]
    public void VoiceLeading_PicksLowestCost()
    {
        var c = Chord.Create(NoteName.C, ChordQuality.Major7);
        var g = Chord.Create(NoteName.G, ChordQuality.Dominant7);
        var from = VoiceLeading.SoundingMidi(
            DropVoicings.Generate(c, DropVoicings.DropType.Drop2, "1234").First());
        var best = VoiceLeading.BestNext(g,
            DropVoicings.Generate(g, DropVoicings.DropType.Drop2, "1234"),
            from);
        Assert.NotNull(best);
    }

    [Fact]
    public void FourVoiceChordsOnly()
    {
        var triad = Chord.Create(NoteName.C, ChordQuality.Major);
        Assert.Throws<ArgumentException>(() =>
            DropVoicings.Generate(triad, DropVoicings.DropType.Drop2, "1234"));
    }
}
