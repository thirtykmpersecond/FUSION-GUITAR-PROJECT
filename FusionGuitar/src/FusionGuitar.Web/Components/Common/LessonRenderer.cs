using FusionGuitar.Web.Components.Common;
using FusionGuitar.Web.Interop;
using FusionGuitar.Web.Theory;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using FretboardComponent = FusionGuitar.Web.Components.Fretboard.Fretboard;
using PianoComponent = FusionGuitar.Web.Components.PianoKeyboard.PianoKeyboard;
using ChordDiagramComponent = FusionGuitar.Web.Components.ChordDiagram.ChordDiagram;
using CircleComponent = FusionGuitar.Web.Components.CircleOfFifths.CircleOfFifths;
using HarmonyComponent = FusionGuitar.Web.Components.HarmonyMap.HarmonyMap;
using NotationComponent = FusionGuitar.Web.Components.Notation.Notation;
using ProgressionPlayerComponent = FusionGuitar.Web.Components.AudioPlayer.ProgressionPlayer;

namespace FusionGuitar.Web.Components.Common;

public sealed class LessonRenderer : ComponentBase
{
    [Parameter, EditorRequired] public string Markdown { get; set; } = "";

    private List<LessonSegment>? _segments;

    protected override void OnParametersSet()
    {
        _segments = string.IsNullOrEmpty(Markdown)
            ? new List<LessonSegment>()
            : LessonParser.Parse(Markdown);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (_segments is null) return;
        var seq = 0;
        foreach (var seg in _segments)
        {
            if (seg.IsHtml)
            {
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", "prose-lesson");
                builder.AddMarkupContent(seq++, seg.Content);
                builder.CloseElement();
            }
            else
            {
                RenderComponent(builder, ref seq, seg);
            }
        }
    }

    private static void RenderComponent(RenderTreeBuilder b, ref int seq, LessonSegment seg)
    {
        switch (seg.Component.ToLowerInvariant())
        {
            case "fretboard":
                RenderFretboard(b, ref seq, seg);
                break;
            case "piano":
                RenderPiano(b, ref seq, seg);
                break;
            case "chord":
                RenderChord(b, ref seq, seg);
                break;
            case "circle":
                RenderCircle(b, ref seq, seg);
                break;
            case "harmony":
                RenderHarmony(b, ref seq, seg);
                break;
            case "staff":
                RenderStaff(b, ref seq, seg);
                break;
            case "tab":
                RenderTab(b, ref seq, seg);
                break;
            case "voicing":
                RenderVoicing(b, ref seq, seg);
                break;
            case "progression":
                RenderProgression(b, ref seq, seg);
                break;
            case "callout":
                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class", "callout");
                b.AddMarkupContent(seq++,
                    "<strong>" + LessonParser.AsString(seg.Args.GetValueOrDefault("title")) + "</strong>");
                b.CloseElement();
                break;
            default:
                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class",
                    "text-xs text-amber-600 bg-amber-50 border border-amber-200 rounded-xl px-3 py-2");
                b.AddContent(seq++, $"Unknown component: {seg.Component}");
                b.CloseElement();
                break;
        }
    }

    private static void RenderFretboard(RenderTreeBuilder b, ref int seq, LessonSegment seg)
    {
        var rootName = LessonParser.AsString(seg.Args.GetValueOrDefault("root"), "C");
        var scaleName = LessonParser.AsString(seg.Args.GetValueOrDefault("scale"), "Major");
        var frets = LessonParser.AsInt(seg.Args.GetValueOrDefault("frets"), 12);

        if (!Enum.TryParse<NoteName>(rootName, ignoreCase: true, out var root)) root = NoteName.C;
        var formula = ScaleFormulas.ByName(scaleName) ?? ScaleFormulas.Major;
        var scale = new Scale(new Note(root, 4), formula);
        var fb = new GuitarFretboard(frets: frets);
        var notes = fb.FindScale(scale, 0, frets);

        b.OpenComponent<FretboardComponent>(seq++);
        b.AddAttribute(seq++, "Notes", notes);
        b.AddAttribute(seq++, "Frets", frets);
        b.CloseComponent();
    }

    private static void RenderPiano(RenderTreeBuilder b, ref int seq, LessonSegment seg)
    {
        var rootName = LessonParser.AsString(seg.Args.GetValueOrDefault("root"), "C");
        var type = LessonParser.AsString(seg.Args.GetValueOrDefault("type"), "chord").ToLowerInvariant();
        var name = LessonParser.AsString(seg.Args.GetValueOrDefault("name"), "Major7");
        var octaves = LessonParser.AsInt(seg.Args.GetValueOrDefault("octaves"), 2);
        var startOctave = LessonParser.AsInt(seg.Args.GetValueOrDefault("startOctave"), 3);

        if (!Enum.TryParse<NoteName>(rootName, ignoreCase: true, out var root)) root = NoteName.C;

        IReadOnlySet<int> pcs;
        if (type == "scale")
        {
            var f = ScaleFormulas.ByName(name) ?? ScaleFormulas.Major;
            pcs = new Scale(new Note(root, 4), f).PitchClassesInOctave;
        }
        else
        {
            Enum.TryParse<ChordQuality>(name, ignoreCase: true, out var q);
            pcs = Chord.Create(root, q).PitchClasses;
        }

        b.OpenComponent<PianoComponent>(seq++);
        b.AddAttribute(seq++, "HighlightPcs", pcs);
        b.AddAttribute(seq++, "RootPc", (int)root);
        b.AddAttribute(seq++, "Octaves", octaves);
        b.AddAttribute(seq++, "StartOctave", startOctave);
        b.CloseComponent();
    }

    private static void RenderChord(RenderTreeBuilder b, ref int seq, LessonSegment seg)
    {
        var rootName = LessonParser.AsString(seg.Args.GetValueOrDefault("root"), "C");
        var quality = LessonParser.AsString(seg.Args.GetValueOrDefault("quality"), "Major");
        var voicingName = LessonParser.AsString(seg.Args.GetValueOrDefault("voicing"), "");

        if (!Enum.TryParse<NoteName>(rootName, ignoreCase: true, out var root)) root = NoteName.C;
        Enum.TryParse<ChordQuality>(quality, ignoreCase: true, out var q);

        var chord = Chord.Create(root, q);
        var voicing = string.IsNullOrEmpty(voicingName)
            ? Voicings.OpenFor(chord)
            : Voicings.ByName(voicingName, root);

        b.OpenComponent<ChordDiagramComponent>(seq++);
        b.AddAttribute(seq++, "Chord", chord);
        b.AddAttribute(seq++, "Voicing", voicing);
        b.CloseComponent();
    }

    // :::voicing chord="Cmaj7" type="drop2" strings="1234" inversion="0"
    // Renders a specific drop voicing (or first matching) as a chord diagram.
    private static void RenderVoicing(RenderTreeBuilder b, ref int seq, LessonSegment seg)
    {
        var chordName = LessonParser.AsString(seg.Args.GetValueOrDefault("chord"), "Cmaj7");
        var type = LessonParser.AsString(seg.Args.GetValueOrDefault("type"), "drop2");
        var strings = LessonParser.AsString(seg.Args.GetValueOrDefault("strings"), "1234");
        var inversion = LessonParser.AsInt(seg.Args.GetValueOrDefault("inversion"), -1);

        var chord = ChordName.Parse(chordName);
        if (chord is null)
        {
            b.OpenElement(seq++, "div");
            b.AddAttribute(seq++, "class", "text-xs text-amber-600");
            b.AddContent(seq++, $"Cannot parse chord: {chordName}");
            b.CloseElement();
            return;
        }

        var dropType = type.Equals("drop3", StringComparison.OrdinalIgnoreCase)
            ? DropVoicings.DropType.Drop3
            : DropVoicings.DropType.Drop2;

        IReadOnlyList<Voicing> all;
        try
        {
            all = DropVoicings.Generate(chord, dropType, strings);
        }
        catch (ArgumentException)
        {
            all = Array.Empty<Voicing>();
        }

        var voicing = all.Count > 0 ? all[0] : null;

        b.OpenComponent<ChordDiagramComponent>(seq++);
        b.AddAttribute(seq++, "Chord", chord);
        b.AddAttribute(seq++, "Voicing", voicing);
        b.AddAttribute(seq++, "FretCount", 7);
        b.CloseComponent();
    }

    // :::progression chords="Dm7,G7,Cmaj7" bars="1,1,1" bpm="100" title="ii–V–I"
    private static void RenderProgression(RenderTreeBuilder b, ref int seq, LessonSegment seg)
    {
        var chordsStr = LessonParser.AsString(seg.Args.GetValueOrDefault("chords"), "Dm7,G7,Cmaj7");
        var title = LessonParser.AsString(seg.Args.GetValueOrDefault("title"), "和弦进行");
        var key = LessonParser.AsString(seg.Args.GetValueOrDefault("key"));
        var bpm = LessonParser.AsInt(seg.Args.GetValueOrDefault("bpm"), 100);

        var steps = Progressions.ParseSteps(chordsStr);
        var chords = new List<ProgressionChord>();
        var names = new List<string>();
        foreach (var step in steps)
        {
            var chord = ChordName.Parse(step.ChordSymbol);
            if (chord is null) continue;
            var notes = chord.Notes.Select(n => n.ToString()).ToList();
            names.Add(step.ChordSymbol);
            for (int x = 0; x < step.Bars; x++)
                chords.Add(new ProgressionChord(notes, "1n"));
        }

        if (chords.Count == 0)
        {
            b.OpenElement(seq++, "div");
            b.AddAttribute(seq++, "class", "text-xs text-amber-600");
            b.AddContent(seq++, $"Cannot parse progression: {chordsStr}");
            b.CloseElement();
            return;
        }

        b.OpenComponent<ProgressionPlayerComponent>(seq++);
        b.AddAttribute(seq++, "Title", title);
        b.AddAttribute(seq++, "Key", key);
        b.AddAttribute(seq++, "Chords", (IReadOnlyList<ProgressionChord>)chords);
        b.AddAttribute(seq++, "Names", (IReadOnlyList<string>)names);
        b.AddAttribute(seq++, "Bpm", bpm);
        b.CloseComponent();
    }

    private static void RenderCircle(RenderTreeBuilder b, ref int seq, LessonSegment seg)
    {
        b.OpenComponent<CircleComponent>(seq++);
        b.CloseComponent();
    }

    private static void RenderHarmony(RenderTreeBuilder b, ref int seq, LessonSegment seg)
    {
        var rootName = LessonParser.AsString(seg.Args.GetValueOrDefault("root"), "C");
        if (!Enum.TryParse<NoteName>(rootName, ignoreCase: true, out var root)) root = NoteName.C;
        b.OpenComponent<HarmonyComponent>(seq++);
        b.AddAttribute(seq++, "Root", root);
        b.CloseComponent();
    }

    // :::staff clef="treble" key="C" time="4/4" notes="c/4/q d/4/q ..."
    // Chords: "c/4+e/4+g/4/w" (keys joined with '+')
    // Optional inline TAB aligned to staff notes: tab="6:8 5:10 4:12"
    private static void RenderStaff(RenderTreeBuilder b, ref int seq, LessonSegment seg)
    {
        var clef = LessonParser.AsString(seg.Args.GetValueOrDefault("clef"), "treble");
        var key = LessonParser.AsString(seg.Args.GetValueOrDefault("key"));
        var time = LessonParser.AsString(seg.Args.GetValueOrDefault("time"));
        var notesStr = LessonParser.AsString(seg.Args.GetValueOrDefault("notes"));
        var tabStr = LessonParser.AsString(seg.Args.GetValueOrDefault("tab"));

        var notes = ParseStaffNotes(notesStr);
        var tab = ParseInlineTab(tabStr, notes);

        b.OpenComponent<NotationComponent>(seq++);
        b.AddAttribute(seq++, "Clef", clef);
        if (!string.IsNullOrEmpty(key)) b.AddAttribute(seq++, "KeySignature", key);
        if (!string.IsNullOrEmpty(time)) b.AddAttribute(seq++, "TimeSignature", time);
        b.AddAttribute(seq++, "Notes", notes);
        if (tab.Count > 0) b.AddAttribute(seq++, "TabNotes", tab);
        b.CloseComponent();
    }

    // Standalone TAB:
    // :::tab notes="6:0+5:2+4:2/q 3:1+2:0+1:0/h"
    // Each token is <string:fret>[+<string:fret>...]/<duration>
    private static void RenderTab(RenderTreeBuilder b, ref int seq, LessonSegment seg)
    {
        var notesStr = LessonParser.AsString(seg.Args.GetValueOrDefault("notes"));
        var tab = ParseTabNotes(notesStr);
        b.OpenComponent<NotationComponent>(seq++);
        b.AddAttribute(seq++, "Clef", "tab");
        b.AddAttribute(seq++, "TabNotes", tab);
        b.CloseComponent();
    }

    private static List<NotationNote> ParseStaffNotes(string notesStr)
    {
        var notes = new List<NotationNote>();
        if (string.IsNullOrEmpty(notesStr)) return notes;
        foreach (var token in notesStr.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            var slashParts = token.Split('/');
            if (slashParts.Length >= 3)
            {
                var duration = slashParts[^1].ToLowerInvariant();
                var pitchPart = string.Join('/', slashParts.Take(slashParts.Length - 1));
                var keys = pitchPart.Split('+')
                    .Select(k => k.Trim().ToLowerInvariant())
                    .Where(k => !string.IsNullOrEmpty(k))
                    .ToArray();
                if (keys.Length > 0)
                    notes.Add(new NotationNote(keys, duration));
            }
        }
        return notes;
    }

    private static List<TabNote> ParseInlineTab(string tabStr, List<NotationNote> staffNotes)
    {
        var result = new List<TabNote>();
        if (string.IsNullOrEmpty(tabStr)) return result;

        var tokens = tabStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < tokens.Length; i++)
        {
            if (!TryParseStringFret(tokens[i], out var str, out var fret)) continue;
            var dur = i < staffNotes.Count ? staffNotes[i].Duration : "q";
            result.Add(new TabNote(new[] { new TabPosition(str, fret) }, dur));
        }
        return result;
    }

    private static List<TabNote> ParseTabNotes(string notesStr)
    {
        var result = new List<TabNote>();
        if (string.IsNullOrEmpty(notesStr)) return result;

        foreach (var token in notesStr.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            var slashParts = token.Split('/');
            if (slashParts.Length < 2) continue;
            var duration = slashParts.Length >= 3
                ? string.Join('/', slashParts.Skip(1)).ToLowerInvariant()
                : "q";
            var positionsPart = slashParts[0];
            var positions = new List<TabPosition>();
            foreach (var sf in positionsPart.Split('+'))
            {
                if (TryParseStringFret(sf, out var s, out var f))
                    positions.Add(new TabPosition(s, f));
            }
            if (positions.Count > 0)
                result.Add(new TabNote(positions, duration));
        }
        return result;
    }

    private static bool TryParseStringFret(string token, out int str, out int fret)
    {
        str = 0; fret = 0;
        var parts = token.Split(':');
        return parts.Length == 2
            && int.TryParse(parts[0], out str)
            && int.TryParse(parts[1], out fret)
            && str is >= 1 and <= 6;
    }
}
