# Fusion Guitar 项目迁移摘要

> 本文档记录项目当前状态、技术栈、约定与踩坑，便于在新机器/新会话中快速恢复上下文。
>
> 最后更新：阶段 4 完成（Drop2/Drop3 Voicing 引擎、和弦进行播放器、模块 4-5 课程）。

## 1. 项目基本信息

- **仓库**：`git@github.com:thirtykmpersecond/FUSION-GUITAR-PROJECT.git`（public，默认分支 `main`）
- **本地路径**：`/Users/xzw/UserDatas/FUSION-GUITAR-PROJECT-PLAN/`
- **规划文档**：`FUSION-GUITAR-PROJECT-PLAN.md`（中文，定义 6 大阶段、7 大教学模块、技术栈、目录规范）
- **Git 署名**：`thirtykmpersecond <thirtykmpersecond@users.noreply.github.com>`（GitHub 隐私邮箱，仅 local config）

## 2. 技术栈实际落地（与计划有偏差）

| 层面 | 计划 | 实际 |
|------|------|------|
| .NET | .NET 8 | **.NET 10**（本机 SDK 仅 `10.0.400`，csproj TargetFramework=`net10.0`；切 net8 只需改一行） |
| 框架 | Blazor WASM Standalone | 同（.NET 10 模板生成 `.slnx` 而非 `.sln`） |
| CSS | Tailwind | Tailwind 3.4.13，MSBuild target 自动调 `npm run tailwind:build` |
| 音频 | Tone.js | Tone@15.0.4（CDN），`wwwroot/js/audio.js` + `interop.js` + `AudioInterop.cs` |
| Markdown | Markdig | Markdig 1.3.2 |
| 乐谱 | VexFlow | **VexFlow 4.2.3（CDN UMD，全局 `Vex`）**，`wwwroot/js/notation.js` + `NotationInterop.cs` |
| 测试 | xUnit | xUnit，**73 个测试全通过** |
| PWA / MAUI | — | 未开始 |

## 3. 已完成阶段

### 阶段 1（commit `86fe6d4`）

- 乐理引擎 `Theory/`：
  - `Note.cs`：不可变 struct，MIDI/频率/Parse，A4=440
  - `Interval.cs`：所有常用音程常量 + Invert
  - `Scale.cs` + `ScaleFormulas`：19 种音阶
  - `Chord.cs` + `ChordFormulas`：18 种和弦（三和弦 + 七和弦 + 9 和弦）
  - `Fretboard.cs`：class 名 **`GuitarFretboard`**，避免与组件重名；支持 Standard/DropD/DADGAD
  - `Enums.cs`
- 组件：`Components/Fretboard/Fretboard.razor`、`Components/PianoKeyboard/PianoKeyboard.razor`
- 页面：Home、FretboardPage、PianoPage
- Apple 风格布局：MainLayout + NavMenu，毛玻璃卡片，Tailwind `brand` 色板

### 阶段 2（commits `42636e1`、`f287fdf`、`40e0bb0`、`cf354e5`）

- `Services/LessonService.cs`：从 `wwwroot/lessons/index.json` 加载 manifest
- `Services/ProgressService.cs`：localStorage 持久化完成状态
- `Components/Common/LessonParser.cs` + `LessonRenderer.cs`：Markdig + 自定义 `:::` 指令
  - `:::fretboard`、`:::piano`、`:::chord`、`:::callout`
- `Components/ChordDiagram/ChordDiagram.razor`：SVG 和弦框
- `Theory/Voicing.cs`：12 个开放把位和弦
- `Pages/LessonPage.razor`：路由 `/lessons/{ModuleId}/{Slug}`
- 模块 1 课程（`wwwroot/lessons/01-basics/`）6 节

### 阶段 3（commits `7a927b5`、`b5f1d4e`、`08dfe86`、`214d5e5`）

- **VexFlow 乐谱**：
  - `Components/Notation/Notation.razor` + `Interop/NotationInterop.cs` + `wwwroot/js/notation.js`
  - 支持标准五线谱（clef/key/time/notes）、吉他 TAB、以及双谱表（`StaveConnector` 连接）
  - `:::staff clef="treble" key="C" time="4/4" notes="c/4/8 d/4/8" tab="5:3 4:0"`
  - `:::tab notes="6:0+5:2+4:2/q 3:1+2:0+1:0/h"`
  - 音符格式：`pitch/octave/duration`（如 `c/4/q`），`+` 连接同时发声的和弦音
  - TAB 弦号：`1` = 高音 E 弦，`6` = 低音 E 弦（VexFlow 标准，与 `GuitarFretboard` 相反）
  - 参数变更时通过 `GetHashCode` 差异检测自动重绘
- **五度圈** `Components/CircleOfFifths/` + `/circle` 页面
- **和声地图** `Components/HarmonyMap/` + `/harmony` 页面
- **音频播放器升级** `Components/AudioPlayer/`：循环、变速（`Tone.Transport.bpm`）
- `/notation` 演示页：C 大调音阶双谱表、A 小调五声双谱表、C 大横按和弦 TAB、Cmaj7 琶音
- 模块 2 课程 8 节（`wwwroot/lessons/02-modes/`）：7 种教会调式 + 调式互换
- 模块 3 课程 7 节（`wwwroot/lessons/03-chord-scale/`）：大调 / 旋律小调 / 和声小调 Chord-Scale、全音 / 减音阶、ii-V-I 应用
- 模块 1 课文中已嵌入真实五线谱：
  - `02-major-scale.md`（C 大调音阶）
  - `03-triads-and-7ths.md`（C 大调三和弦 / 减 / 增 / 减三和弦）
  - `05-3nps.md`（3NPS Pattern 1 C4→C5 上行，五线谱 + TAB）
  - `06-pentatonic-blues.md`（A 小调五声音阶）

### 阶段 4（Drop2/Drop3 引擎、进行播放器、模块 4-5）

- **DropVoicings 引擎** `Theory/DropVoicings.cs`：
  - `Generate(chord, DropType.Drop2|Drop3, strings)`：4 音七和弦 × 3 个弦组 × 4 转位生成可弹指型
  - 弦组参数用 VexFlow 约定（`"1234"` = ①②③④ = 高音组），内部归一化为升序弦索引
  - 过滤：品数 ≤ 15、手指跨度 ≤ 5 品、必须含全部和弦音
  - 输出复用 `Voicing` / `Fingering` record（带 `BaseFret`、`Notes`）
- **VoiceLeading** `Theory/VoiceLeading.cs`：`SoundingMidi()` 取实响 MIDI；`BestNext()` 选移动最少的和弦
- **ChordName.Parse** `Theory/ChordName.cs`：紧凑符号解析（`Cmaj7`/`Gm7`/`Am7b5`/`Dm9`/`G7#5`…）
- **ChordDiagram 升级**：横按条（连续弦同指同品画粗横杠）、`FretCount` 参数（Drop 指型常用 6-7 品视窗）
- **/voicings 页** `Pages/VoicingsPage.razor`：根音/质量/类型/弦组 4 选 → 卡片流浏览所有指型，点击发声
- **:::voicing chord="Cmaj7" type="drop2" strings="1234"** 指令
- **Progression 库** `Theory/Progression.cs`：8 个经典进行（ii-V-I 大/小调、Autumn Leaves、Rhythm Changes、So What、Maiden Voyage、Cantaloupe Island、C Jam Blues），`ParseSteps("Dm7:2,G7")` 支持小节数
- **ProgressionPlayer** `Components/AudioPlayer/ProgressionPlayer.razor`：循环播放（复用 Tone.js `playProgression`）、BPM、单和弦试听 chip；`chip`/`chip-active` 样式加入 app.css
- **/progressions 页** + **:::progression chords="Dm7,G7,Cmaj7" bpm="80"** 指令
- 模块 4 `04-advanced-harmony/` 8 节：shell voicings、drop2、drop2 转位与声部连接、drop3、三全音替代、次属与连锁 ii-V、重配和声、上层结构 & So What
- 模块 5 `05-rhythm/` 6 节：节奏感觉、伴奏节奏型、Bossa、Funk、奇数拍、Rhythm Changes
- 每节课嵌入 `:::voicing` / `:::staff`(五线谱+TAB) / `:::progression` 作为 Lick 示范

## 4. 关键约定 & 踩过的坑

1. **类名与命名空间冲突**：乐理类已改名为 `GuitarFretboard`；Renderer 里用 `using XXComponent = ...` 别名。
2. **Blazor SVG `<text>` 标签冲突**：SVG 文本必须包在 `<g>...</g>` 里，或用 `@:` 显式标记。
3. **`@bind:after="Refresh"`**：.NET 7+，select 变化后刷新数据。
4. **Tailwind 构建**：csproj Target 监听 `Styles/app.css` 和 `tailwind.config.js`，输出 `wwwroot/css/app.css`（.gitignore 排除）。新机器首次 `cd src/FusionGuitar.Web && npm install`。
5. **`:::` 指令解析器**：自定义最简实现，非 Markdig 扩展。**支持引号内含空格的值**（`notes="a/4/8 c/5/8"`、`scale="Pentatonic Minor"`）。
6. **LessonParser.Tokenize 空格 bug（已修复）**：旧分词器只检查 token 起始字符是否为引号，在 `key="value with spaces"` 的第一个空格处断开，导致后续属性错位、课文文本串进 key 值，引发 VexFlow `Invalid key name` 错误。新实现先扫描到 `=`，若值以引号开头则读到匹配的闭合引号。回归测试见 `LessonParserTests.cs`。
7. **VexFlow 4 不接受空调号**：`stave.addKeySignature("C")` 或 `"Am"` 抛 `Invalid key name`。C 大调 / A 小调无升降号，应**跳过** `addKeySignature` 调用。
8. **TAB 弦号方向不一致**：
   - VexFlow `TabNote` positions：`str: 1` = 高音 E 弦（①弦），`str: 6` = 低音 E 弦（⑥弦）
   - `GuitarFretboard` / `Voicing`：`StringIndex = 0` = 低 E 弦，`5` = 高 E 弦
   - Markdown `tab="4:10 3:7"` 使用 VexFlow 约定（1=高 E）
9. **B 弦偏移**：3NPS/CAGED 在 ③ 弦到 ② 弦整体后移 1 品。
10. **`NavLink` href**：必须以 `/` 开头；空字符串会匹配所有路由。
11. **.gitignore 位置**：在 `FusionGuitar/` 子目录下，模式 `**/wwwroot/css/app.css`。
12. **.NET 10 模板**：生成 `.slnx`（新 XML 解决方案格式），命令用 `dotnet sln FusionGuitar.slnx add ...`。
13. **`Chord.Name` 截断**：用 `NoteLetters[Root.PitchClass]` 数组查表，不用 `TrimEnd('0'..'9')`。
14. **GitHub 推送网络问题**：国内网络下 `github.com` 可能被 DNS 污染到 `198.1.x.x`，SSH 22/443 超时。挂代理或换网络后 `git push origin main` 即可，不影响本地开发。
15. **VexFlow CDN**：`index.html` 中 `<script src="https://cdn.jsdelivr.net/npm/vexflow@4.2.3/releases/vexflow-min.js"></script>` 必须**非 module** 加载（UMD），`notation.js` 作为 ES module 读取全局 `Vex`。
16. **DropVoicings 弦组参数约定**：`strings` 用 VexFlow 约定（`"1234"` = ①②③④ = 高音组），内部归一化为升序 `StringIndex`（0=低E）。与 `GuitarFretboard` 相反。
17. **DropVoicings 只支持 4 音七和弦**：三和弦会抛 `ArgumentException`。生成时过滤：品数 ≤ 15、手指跨度 ≤ 5 品、必须含全部和弦音（`chord.PitchClasses.IsSubsetOf`）。
18. **Chord.Create 默认 octave=4**：`Chord.Notes` 落在 C4-B5 区间。课程 `:::progression` / `:::staff` 引用和弦音符时注意音域。
19. **`ChordName.Parse` 边界**：`"F#"` 提升号正确；`"Bbm7"` 的 b 会转成 A#（等音）。三全音替代课里 `Db7` 解析为 D#（等音）不改变音高。

## 5. 目录结构（当前实际）

```
FUSION-GUITAR-PROJECT-PLAN/
├── FUSION-GUITAR-PROJECT-PLAN.md
├── PROJECT-CONTEXT.md
├── README.md
└── FusionGuitar/
    ├── .gitignore
    ├── FusionGuitar.slnx
    ├── src/FusionGuitar.Web/
    │   ├── FusionGuitar.Web.csproj        # net10.0 + Markdig + Tailwind target
    │   ├── Program.cs
    │   ├── App.razor, Routes.razor, _Imports.razor
    │   ├── package.json, tailwind.config.js, Styles/app.css
    │   ├── Components/
    │   │   ├── Layout/{MainLayout,NavMenu}.razor
    │   │   ├── Fretboard/Fretboard.razor
    │   │   ├── PianoKeyboard/PianoKeyboard.razor
    │   │   ├── ChordDiagram/ChordDiagram.razor
    │   │   ├── Notation/Notation.razor
    │   │   ├── CircleOfFifths/
    │   │   ├── HarmonyMap/
    │   │   ├── AudioPlayer/{AudioPlayer,ProgressionPlayer}.razor
    │   │   └── Common/{LessonParser.cs,LessonRenderer.cs}
    │   ├── Theory/                        # Note, Interval, Scale, Chord, GuitarFretboard,
    │   │                                  # Voicing, ChordName, DropVoicings, VoiceLeading,
    │   │                                  # Progression, Enums
    │   ├── Interop/                       # AudioInterop, NotationInterop
    │   ├── Services/                      # LessonService, LessonModels, ProgressService
    │   ├── Pages/                         # Home, FretboardPage, PianoPage, NotationPage,
    │   │                                  # CirclePage, HarmonyPage, VoicingsPage,
    │   │                                  # ProgressionsPage, LessonPage, NotFound
    │   └── wwwroot/
    │       ├── index.html                 # Tone@15 + VexFlow@4 CDN
    │       ├── js/{audio.js,notation.js,interop.js}
    │       └── lessons/
    │           ├── index.json             # 模块 1-5 课程目录
    │           ├── 01-basics/*.md         # 6 节
    │           ├── 02-modes/*.md          # 8 节
    │           ├── 03-chord-scale/*.md    # 7 节
    │           ├── 04-advanced-harmony/*.md  # 8 节
    │           └── 05-rhythm/*.md         # 6 节
    └── tests/FusionGuitar.Tests/
        ├── FusionGuitar.Tests.csproj
        ├── Theory/{Note,Scale,Chord,Fretboard,DropVoicing,ChordName,Progression}Tests.cs
        └── LessonParserTests.cs           # 含引号空格回归
```

## 6. 环境要求（新机器）

- **.NET SDK 10.0+**（或改 `net8.0`）
- **Node.js**（仅 Tailwind 构建用，版本不敏感）
- 可选：`wasm-tools` workload（发布 AOT 优化）

### 首次拉取后

```bash
cd FusionGuitar/src/FusionGuitar.Web
npm install

cd ../../..
dotnet build FusionGuitar/FusionGuitar.slnx
dotnet test  FusionGuitar/FusionGuitar.slnx     # 应 73 passed
dotnet run   --project FusionGuitar/src/FusionGuitar.Web
# 访问 http://localhost:5294
```

### 部署

```bash
dotnet publish FusionGuitar/src/FusionGuitar.Web -c Release -o ./publish
```

## 7. 提交历史（按时间倒序，关键提交）

```
cbe9699  feat(lessons): module 4 advanced harmony + module 5 rhythm
546d7b8  feat(progressions): progression library, player component, /progressions page
065923f  feat(voicings): chord diagram barre support + /voicings browser page
c14e9de  feat(theory): Drop2/Drop3 voicing engine + voice leading
fe93afd  docs: refresh README and PROJECT-CONTEXT for stage 3
96c2a6d  docs(lessons): remove redundant ASCII TAB in pentatonic/blues lesson
214d5e5  fix(parser): preserve spaces inside quoted directive attributes
08dfe86  fix(notation): reject C/Am key signature, add TAB staff support
b5f1d4e  feat(notation): VexFlow staff rendering in lessons via :::staff directive
cf354e5  fix(pages): pass Markdown as expression not literal
7a927b5  feat(stage3): circle of fifths, harmony map, notation, audio player, modules 2-3
40e0bb0  docs(lessons): simplify 3NPS pattern-2..7 section
f287fdf  docs(lessons): revise 3NPS lesson with accurate tab and pattern table
42636e1  feat(stage2): lesson framework, ChordDiagram, module 1 content
86fe6d4  chore: bootstrap Fusion Guitar stage 1
```

## 8. 下一步：阶段 5（尚未开始）

按计划文档：

1. Lick 库系统（乐谱 + 音频 + 指板联动）
2. 模块 6 即兴演奏课程
3. 模块 7 风格与大师分析课程
4. 乐谱与指板 / 钢琴联动（点击乐谱高亮指板位置）
5. `<ChordDiagram>` 支持 Drop2+4 / Drop 扩展（目前 Drop3 已支持）

## 9. 风格 / 代码约定

- 不可变值类型 + 纯函数乐理，数据驱动（`IReadOnlyList<int>` 半音公式）
- Razor 组件用 SVG 手绘，不引入第三方 Blazor UI 库
- Apple 风格：大圆角 `rounded-2xl`、毛玻璃 `backdrop-blur-xl`、柔和阴影、`brand` 靛蓝色板
- 中文为主，术语首次出现附英文（如「音程 Intervals」）
- 提交信息用 Conventional Commits（`feat` / `fix` / `docs` / `chore`，scope 用 `notation` / `lessons` / `parser` 等）
- **不主动写注释**，靠命名表达意图
- 不创建 `.env` / 密钥文件

## 10. 已知遗留小问题

- `audio.js` 和 `interop.js` 有重复代码，未来只保留 `interop.js`
- `<ChordDiagram>` 只支持开放把位；横按 / Drop voicings 待阶段 4
- 深色模式 class 已留但还没有切换按钮（阶段 6）
- 移动端布局仅基础响应式（`md:` 断点），未深度适配
- 模块 4-7 课程待写
- `ProgressService` 用 `localStorage`，未做多设备同步
- VexFlow 走 CDN，离线场景待 PWA 阶段本地化
