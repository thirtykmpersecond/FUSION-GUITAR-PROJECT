using FusionGuitar.Web.Components.Common;
using FusionGuitar.Web.Theory;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using FretboardComponent = FusionGuitar.Web.Components.Fretboard.Fretboard;
using PianoComponent = FusionGuitar.Web.Components.PianoKeyboard.PianoKeyboard;
using ChordDiagramComponent = FusionGuitar.Web.Components.ChordDiagram.ChordDiagram;

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
}
