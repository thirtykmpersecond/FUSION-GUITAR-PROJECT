using System.Text.Json;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FusionGuitar.Web.Components.Common;

public sealed class LessonSegment
{
    public bool IsHtml { get; init; }
    public string Content { get; init; } = "";
    public string Component { get; init; } = "";
    public Dictionary<string, JsonElement> Args { get; init; } = new();
}

public static class LessonParser
{
    private static readonly MarkdownPipeline Pipeline =
        new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

    public static List<LessonSegment> Parse(string markdown)
    {
        var segments = new List<LessonSegment>();
        var lines = markdown.Replace("\r\n", "\n").Split('\n');
        var mdBuffer = new List<string>();

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd();
            if (line.StartsWith(":::") && line.Length > 3)
            {
                FlushMarkdown(segments, mdBuffer);
                segments.Add(ParseDirective(line));
            }
            else
            {
                mdBuffer.Add(line);
            }
        }
        FlushMarkdown(segments, mdBuffer);
        return segments;
    }

    private static void FlushMarkdown(List<LessonSegment> segments, List<string> buffer)
    {
        if (buffer.Count == 0) return;
        var md = string.Join('\n', buffer);
        if (string.IsNullOrWhiteSpace(md)) { buffer.Clear(); return; }
        var html = Markdig.Markdown.ToHtml(md, Pipeline);
        segments.Add(new LessonSegment { IsHtml = true, Content = html });
        buffer.Clear();
    }

    private static LessonSegment ParseDirective(string line)
    {
        var body = line[3..].Trim();
        var sp = body.IndexOfAny(new[] { ' ', '\t' });
        string name;
        string argsJson = "{}";
        if (sp < 0)
        {
            name = body;
        }
        else
        {
            name = body[..sp].Trim();
            var rest = body[sp..].Trim();
            if (rest.Length > 0)
            {
                try
                {
                    argsJson = ConvertKeyValueToJson(rest);
                }
                catch
                {
                    argsJson = "{}";
                }
            }
        }
        var args = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(argsJson) ?? new();
        return new LessonSegment { IsHtml = false, Component = name, Args = args };
    }

    private static string ConvertKeyValueToJson(string rest)
    {
        if (rest.StartsWith('{')) return rest;
        // key="value" key2='value2' key3=number
        var dict = new Dictionary<string, object?>();
        foreach (var pair in Tokenize(rest))
        {
            var eq = pair.IndexOf('=');
            if (eq < 0) continue;
            var k = pair[..eq].Trim();
            var v = pair[(eq + 1)..].Trim();
            if ((v.StartsWith('"') && v.EndsWith('"')) || (v.StartsWith('\'') && v.EndsWith('\'')))
            {
                dict[k] = v[1..^1];
            }
            else if (int.TryParse(v, out var i))
            {
                dict[k] = i;
            }
            else if (double.TryParse(v, System.Globalization.NumberStyles.Float,
                         System.Globalization.CultureInfo.InvariantCulture, out var d))
            {
                dict[k] = d;
            }
            else if (bool.TryParse(v, out var b))
            {
                dict[k] = b;
            }
            else
            {
                dict[k] = v;
            }
        }
        return JsonSerializer.Serialize(dict);
    }

    private static List<string> Tokenize(string s)
    {
        var result = new List<string>();
        int i = 0;
        while (i < s.Length)
        {
            while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
            if (i >= s.Length) break;
            int start = i;

            // Form: key="quoted value with spaces"
            // Read up to '=' then, if next char is a quote, read until closing quote.
            int eq = -1;
            int j = i;
            while (j < s.Length && !char.IsWhiteSpace(s[j]) && s[j] != '=') j++;
            if (j < s.Length && s[j] == '=')
            {
                eq = j;
                j++; // past '='
                if (j < s.Length && (s[j] == '"' || s[j] == '\''))
                {
                    char q = s[j];
                    j++;
                    while (j < s.Length && s[j] != q) j++;
                    if (j < s.Length) j++; // include closing quote
                    result.Add(s[start..j]);
                    i = j;
                    continue;
                }
            }

            // Unquoted token: read until whitespace
            while (i < s.Length && !char.IsWhiteSpace(s[i])) i++;
            result.Add(s[start..i]);
        }
        return result;
    }

    public static string AsString(JsonElement el, string fallback = "")
        => el.ValueKind switch
        {
            JsonValueKind.String => el.GetString() ?? fallback,
            JsonValueKind.Number => el.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => fallback
        };

    public static int AsInt(JsonElement el, int fallback)
        => el.ValueKind == JsonValueKind.Number ? el.GetInt32() : fallback;

    public static bool AsBool(JsonElement el, bool fallback)
        => el.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => fallback
        };
}
