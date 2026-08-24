# Fusion Guitar

面向电吉他手的系统 Fusion / Jazz 乐理教材，以交互式 Web 应用形式呈现：可点击发声的指板图、钢琴键盘、和弦图，配合 VexFlow 渲染的五线谱与吉他六线谱（TAB），以及可循环变速的音频播放器。中文为主，术语附英文对照。

> 当前处于 **阶段 5 完成**：乐理引擎、课程框架、交互式组件、VexFlow 乐谱（含 TAB）、五度圈、和声地图、Drop2/Drop3 引擎、和弦进行播放器、模块 1-7 课程、以及乐谱+指板+音频三联动 Lick 库均已落地。详细路线图见 [FUSION-GUITAR-PROJECT-PLAN.md](./FUSION-GUITAR-PROJECT-PLAN.md)。

## 技术栈

- [.NET 10 Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)（C# / Razor，编译为 WebAssembly，`.slnx` 解决方案）
- [Tailwind CSS](https://tailwindcss.com/)（Apple 风格 UI，毛玻璃 / 大圆角 / 深浅色模式预留）
- [Tone.js](https://tonejs.github.io/)（经 JS Interop 调用，音频合成与回放）
- [VexFlow 4.2.3](https://www.vexflow.com/)（CDN UMD，`Vex.Flow` 全局变量，渲染五线谱与 TAB）
- [Markdig](https://github.com/xoofx/markdig)（Markdown 解析 + 自定义 `:::` 组件指令）
- xUnit（乐理引擎、解析器单元测试，共 82 个）

## 目录结构

```
FusionGuitar/
├── src/FusionGuitar.Web/
│   ├── Theory/            # 乐理引擎：Note / Interval / Scale / Chord / GuitarFretboard
│   │                      #            / Voicing / ChordName / DropVoicings / Progression
│   │                      #            / Lick / LickLibrary
│   ├── Components/
│   │   ├── Fretboard/     # 指板 SVG（支持逐音高亮）
│   │   ├── PianoKeyboard/ # 钢琴键盘
│   │   ├── ChordDiagram/  # 和弦框（支持横按）
│   │   ├── Notation/      # VexFlow 五线谱 + TAB（支持点击/高亮）
│   │   ├── CircleOfFifths/# 五度圈
│   │   ├── HarmonyMap/    # 和声替代网络
│   │   ├── AudioPlayer/   # 音频回放（循环 / 变速 / 节拍器 / 进行播放）
│   │   ├── Lick/          # LickPlayer：乐谱 + 指板 + 音频三联动
│   │   ├── Layout/        # MainLayout / NavMenu
│   │   └── Common/        # LessonParser / LessonRenderer
│   ├── Interop/           # C# → JS：AudioInterop / NotationInterop
│   ├── Services/          # LessonService / ProgressService
│   ├── Pages/             # Home / Fretboard / Piano / Notation / Circle / Harmony
│   │                      # / Voicings / Progressions / Licks / Lesson
│   ├── wwwroot/
│   │   ├── js/            # audio.js / notation.js / interop.js
│   │   ├── lessons/       # 01-basics / 02-modes / 03-chord-scale
│   │   │                  # 04-advanced-harmony / 05-rhythm / 06-improvisation
│   │   │                  # 07-masters + index.json
│   │   └── index.html     # 引入 Tone.js + VexFlow CDN
│   └── Styles/app.css     # Tailwind 入口
└── tests/FusionGuitar.Tests/
    ├── Theory/            # 乐理引擎单元测试
    └── LessonParserTests.cs
```

## 本地开发

### 环境要求

- [.NET SDK 10.0+](https://dotnet.microsoft.com/download)（项目文件目标 `net10.0`，可按需改为 `net8.0`）
- [Node.js](https://nodejs.org/)（仅用于编译 Tailwind CSS）

### 首次安装与运行

```bash
# 安装 Tailwind 依赖（仅首次）
cd FusionGuitar/src/FusionGuitar.Web
npm install

cd ../../..
dotnet build FusionGuitar/FusionGuitar.slnx
dotnet test  FusionGuitar/FusionGuitar.slnx     # 应 82 passed
dotnet run   --project FusionGuitar/src/FusionGuitar.Web
# 访问 http://localhost:5294
# 课程入口：/lessons/01-basics/01-notes-and-intervals
```

构建时 MSBuild target 会自动执行 `npm run tailwind:build`。若需实时编辑样式，另开终端：

```bash
cd FusionGuitar/src/FusionGuitar.Web
npm run tailwind:watch
```

### 部署

```bash
dotnet publish FusionGuitar/src/FusionGuitar.Web -c Release -o ./publish
```

输出纯静态 WASM，可放 GitHub Pages / Cloudflare Pages / 任意静态托管。

## 课程 Markdown 指令

课程文件位于 `wwwroot/lessons/{moduleId}/{slug}.md`，由 Markdig 渲染，支持以下自定义 `:::` 指令：

```markdown
:::fretboard root="C" scale="Major" frets=12
:::piano root="A" type="chord" name="Minor7"
:::chord root="C" quality="Major"
:::callout title="提示"

:::staff clef="treble" key="C" time="4/4"
         notes="c/4/8 d/4/8 e/4/8 f/4/8"
         tab="5:3 4:0 4:2 4:3"
:::tab notes="6:0+5:2+4:2/q 3:1+2:0+1:0/h"
:::voicing chord="Cmaj7" type="drop2" strings="1234"
:::progression chords="Dm7,G7,Cmaj7" bpm="80" title="ii–V–I"
:::lick name="Dorian 上行动机" bpm="90"
```

- `:::staff`：VexFlow 五线谱；`notes` 格式为 `pitch/octave/duration`（`+` 连接同时发声的和弦音），`tab` 可选，提供时自动在下方渲染对齐的六线谱。
- `:::tab`：纯六线谱；`notes` 格式为 `string:fret/duration`，多弦用 `+` 连接。TAB 弦号遵循 VexFlow 约定：`1` = 高音 E 弦，`6` = 低音 E 弦。
- `:::voicing`：渲染一个 Drop2/Drop3 和弦指型图（`type` = `drop2` / `drop3`，`strings` 为弦组，`1` 表示高音 E 弦）。
- `:::progression`：循环播放和弦进行（`chords` 用逗号分隔，`Dm7:2` 表示两小节）。
- `:::lick`：嵌入一个三联动 Lick（乐谱 + TAB + 指板 + 音频），`name` 对应 Lick 库中的句子，`bpm` 可调。
- `key="C"` / `key="Am"` 等无升降号调会被自动跳过（VexFlow 4 不接受空调号字符串）。
- 所有带引号的属性值**支持内含空格**（解析器已修复）。

## 已完成阶段

### 阶段 1：乐理引擎 + 基础可视化

- `Theory/`：Note / Interval / Scale（19 种公式）/ Chord（18 种公式）/ GuitarFretboard（Standard / Drop D / DADGAD）
- 组件：`<Fretboard>`、`<PianoKeyboard>`
- 页面：Home / FretboardPage / PianoPage
- Apple 风格布局、毛玻璃导航、Tailwind `brand` 色板

### 阶段 2：课程框架 + ChordDiagram + 模块 1

- `LessonService`（从 `wwwroot/lessons/index.json` 加载）、`ProgressService`（localStorage）
- `LessonParser` + `LessonRenderer`：Markdig 渲染 + `:::` 指令
- `<ChordDiagram>`：SVG 和弦框，×/○、手指号、点击发声
- `Voicing`：12 个开放把位和弦
- 模块 1 课程 6 节：音名与音程 / 大调音阶 / 三和弦与七和弦 / CAGED / 3NPS / 五声与蓝调
- `/lessons/{moduleId}/{slug}` 路由，含上下课导航、完成按钮、侧边进度计数

### 阶段 3：五度圈 + 和声地图 + VexFlow 乐谱 + 模块 2-3

- `<CircleOfFifths>`：SVG 五度圈，展示调关系与调式互换
- `<HarmonyMap>`：和弦替代网络
- `<Notation>`：VexFlow 4 五线谱组件，支持 clef / key / time / 和弦 / 双谱表（五线谱 + TAB，`StaveConnector` 连接）
- `Interop/NotationInterop.cs` + `wwwroot/js/notation.js`
- `:::staff` / `:::tab` Markdown 指令，引号值支持空格
- `<AudioPlayer>` 升级：循环、变速（`Tone.Transport.bpm`）
- 模块 2 课程 8 节：7 种教会调式 + 调式互换
- 模块 3 课程 7 节：大调 / 旋律小调 / 和声小调 Chord-Scale、全音 / 减音阶、ii-V-I 应用
- `/notation` 演示页，`/circle`、`/harmony` 交互页

### 阶段 4：和声与节奏

- **Drop2/Drop3 引擎**（`Theory/DropVoicings.cs`）：任意 4 音七和弦 × 三个弦组 × 四转位生成可弹指型，自动过滤超出指板 / 手指跨度的结果
- `VoiceLeading`：声部连接评分，选取移动最少的和弦进行衔接
- `ChordName.Parse`：紧凑和弦符号解析（`Cmaj7` / `Gm7` / `Am7b5` / `Dm9`…）
- `<ChordDiagram>` 升级：横按条渲染 + 可调品数视窗
- `/voicings` 页：浏览任意 Drop2/Drop3 和弦的所有指型并试听
- `Theory/Progression.cs` + `<ProgressionPlayer>` + `/progressions` 页：8 个经典进行的循环播放（ii–V–I 大/小调、Autumn Leaves、Rhythm Changes、So What、Maiden Voyage、Cantaloupe Island、C Jam Blues）
- `:::voicing` / `:::progression` 指令
- 模块 4 课程 8 节：Shell 和弦、Drop2/Drop3、三全音替代、次属和弦、重配和声、上层结构
- 模块 5 课程 6 节：节奏感觉、伴奏节奏型、Bossa、Funk、奇数拍、Rhythm Changes
- 每节课程配 Lick 示范（五线谱 + TAB）与可循环播放的进行

### 阶段 5：即兴与风格

- **Lick 模型**（`Theory/Lick.cs`）：`Lick` / `LickNote`（MIDI + 时值 + 指板位置），`LickBuilder.FromMidi` / `FromFrets` 紧凑语法
- **LickPlayer**（`Components/Lick/`）：**乐谱 + 指板 + 音频三联动**
  - VexFlow 五线谱 + TAB（Notation 增加音符点击 + 高亮环）
  - 指板（Fretboard 增加 `Highlight` 脉冲高亮）
  - Tone.js 逐音播放（播放/停止/0.5x-1x 变速/单音点击跳转），播放时同步高亮当前音
- **LickLibrary**（`Theory/LickLibrary.cs`）：17 个分类句子（Dorian、Mixolydian、Blues、Bebop、Pentatonic、Lydian、Fusion、Altered、Chromatic + 4 个大师风格）
- `/licks` 页：按风格筛选 + 堆叠播放器；`:::lick name="..." bpm="..."` 指令
- 模块 6 课程 8 节：目标音导向、节奏控制、动机句法、空间感、和弦音即兴、转调与指板导航、色彩音与张力、练习方法论
- 模块 7 课程 7 节：Holdsworth、Metheny、Martino、Scofield、McLaughlin、Fusion 综合、大师研习法（大师 Lick 加厚版）
- Notation/Fretboard/AudioInterop 均支持逐音回调与高亮（`NotationEvents` / `SequenceEvents`）

## 后续路线

- 阶段 6：深色模式切换、移动端深度适配、PWA、部署与多设备同步

## License

暂未指定开源协议（All Rights Reserved）。如需协作或再使用，请联系作者。
