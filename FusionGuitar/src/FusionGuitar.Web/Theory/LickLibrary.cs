namespace FusionGuitar.Web.Theory;

/// <summary>Built-in library of style-classified improvisational licks.</summary>
public static class LickLibrary
{
    public static readonly Lick Dorian1 = new(
        "Dorian 上行动机", "Dorian", "D",
        Backing: "Dm7",
        Description: "从根音出发，经 9、b3、4、5 上行的 D Dorian 句子。",
        Notes: LickBuilder.FromFrets("4:0:0.5 4:2:0.5 4:3:0.5 3:0:0.5 3:2:0.5 3:4:1 3:0:0.5 3:2:0.5"));

    public static readonly Lick Dorian2 = new(
        "Dorian 六度跳进", "Dorian", "D",
        Backing: "Dm7",
        Description: "用大六度跳进制造 D Dorian 的明亮色彩（D–B 六度）。",
        Notes: LickBuilder.FromFrets("4:2:1 2:0:1 3:4:0.5 3:2:0.5 3:0:1 3:2:1 3:4:1"));

    public static readonly Lick Mixolydian1 = new(
        "Mixolydian 属琶音", "Mixolydian", "G",
        Backing: "G7",
        Description: "G7 琶音（G-B-D-F）+ 经过 9（A）的下行，标准属七句子。",
        Notes: LickBuilder.FromFrets("6:3:0.5 5:2:0.5 5:3:0.5 4:2:0.5 4:0:0.5 3:0:0.5 3:2:1 2:0:1"));

    public static readonly Lick Mixolydian2 = new(
        "Mixolydian 3-9 色彩", "Mixolydian", "G",
        Backing: "G13",
        Description: "强调 3（B）与 9（A）的 G13 色彩句子。",
        Notes: LickBuilder.FromFrets("5:2:1 4:2:0.5 4:0:0.5 3:2:0.5 3:4:0.5 3:2:1 2:0:1 2:3:1"));

    public static readonly Lick Blues1 = new(
        "蓝调 b3 回旋", "Blues", "A",
        Backing: "A7",
        Description: "A 蓝调音阶（A-C-D-Eb-E-G）的经典 b3 环绕句。",
        Notes: LickBuilder.FromFrets("5:0:0.5 4:2:0.5 4:1:0.5 4:2:0.5 3:2:1 3:1:0.5 3:0:0.5 3:2:1"));

    public static readonly Lick Bebop1 = new(
        "Bebop 八度环绕", "Bebop", "C",
        Backing: "Cmaj7",
        Description: "Bebop 大调音阶的下行包围音句（C B A G F E D C）。",
        Notes: LickBuilder.FromMidi("72:0.5 71:0.5 69:0.5 67:0.5 65:0.5 64:0.5 62:0.5 60:1"));

    public static readonly Lick Bebop2 = new(
        "Bebop 属七包围", "Bebop", "G",
        Backing: "G7",
        Description: "在 G7 的 3（B）周围做半音包围，Bebop 属七典型手法。",
        Notes: LickBuilder.FromMidi("67:0.5 66:0.5 68:0.5 71:0.5 70:0.5 71:0.5 74:1"));

    public static readonly Lick Pentatonic1 = new(
        "五声滑音句子", "Pentatonic", "A",
        Backing: "Am7",
        Description: "A 小调五声音阶（A-C-D-E-G）带滑音感的连续 3 音组。",
        Notes: LickBuilder.FromFrets("5:0:0.5 5:3:0.5 4:0:0.5 4:2:0.5 3:0:0.5 3:2:0.5 3:0:0.5 3:4:1"));

    public static readonly Lick Minor1 = new(
        "自然小调下行", "Natural Minor", "A",
        Backing: "Am7",
        Description: "A 自然小调（A-B-C-D-E-F-G）的八度下行。",
        Notes: LickBuilder.FromMidi("69:0.5 67:0.5 65:0.5 64:0.5 62:0.5 60:0.5 59:0.5 57:1"));

    public static readonly Lick Lydian1 = new(
        "Lydian #4 色彩", "Lydian", "F",
        Backing: "Fmaj7",
        Description: "强调 #4（B 自然音）的 F Lydian 句子，梦幻悬浮感。",
        Notes: LickBuilder.FromFrets("5:1:0.5 4:2:0.5 4:4:0.5 4:3:0.5 3:2:0.5 3:4:0.5 3:3:1 3:2:1"));

    public static readonly Lick Fusion1 = new(
        "Fusion 16 分连音", "Fusion", "D",
        Backing: "Dm9",
        Description: "D Dorian 的 16 分音符密集连奏，Fusion 常用语汇。",
        Notes: LickBuilder.FromFrets("4:2:0.25 4:0:0.25 3:4:0.25 3:2:0.25 3:0:0.25 3:2:0.25 3:4:0.25 3:2:0.25 3:0:0.5 2:0:0.5"));

    public static readonly Lick Altered1 = new(
        "Altered 属琶音", "Altered", "C",
        Backing: "C7alt",
        Description: "C7alt（C-Db-Eb-Gb-Ab-Bb）的减琶音式上行，紧张色彩。",
        Notes: LickBuilder.FromMidi("60:0.5 61:0.5 63:0.5 66:0.5 68:0.5 70:1"));

    public static readonly Lick Chromatic1 = new(
        "半音经过句", "Chromatic", "D",
        Backing: "Dm7",
        Description: "目标音 G 前的半音级进包围，最常用的色彩手法。",
        Notes: LickBuilder.FromFrets("4:0:0.5 4:1:0.5 4:2:0.5 4:3:0.5 3:2:1 3:3:0.5 3:5:1"));

    public static readonly IReadOnlyList<Lick> All = new[]
    {
        Dorian1, Dorian2, Mixolydian1, Mixolydian2, Blues1,
        Bebop1, Bebop2, Pentatonic1, Minor1, Lydian1,
        Fusion1, Altered1, Chromatic1
    };

    public static Lick? ByName(string name)
        => All.FirstOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public static IReadOnlyList<Lick> ByStyle(string style)
    {
        if (string.IsNullOrWhiteSpace(style) || style == "全部")
            return All;
        return All.Where(l => l.Style.Equals(style, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public static IReadOnlyList<string> Styles =>
        All.Select(l => l.Style).Distinct().OrderBy(s => s).ToList();
}
