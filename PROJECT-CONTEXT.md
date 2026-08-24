# Fusion Guitar 项目迁移摘要

> 本文档记录项目当前状态、技术栈、约定与踩坑，便于在新机器/新会话中快速恢复上下文。

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
| 音频 | Tone.js | Tone@15.0.4（CDN），`wwwroot/js/interop.js` ES module + `AudioInterop.cs` |
| Markdown | Markdig | Markdig 1.3.2 |
| 测试 | xUnit | xUnit，35 个测试全通过 |
| 乐谱 VexFlow / PWA / MAUI | — | **未开始** |

## 3. 已完成阶段

### 阶段 1（commit `86fe6d4`）

- 乐理引擎 `Theory/`：
  - `Note.cs`：不可变 struct，MIDI/频率/Parse，A4=440
  - `Interval.cs`：所有常用音程常量 + Invert
  - `Scale.cs` + `ScaleFormulas`：19 种音阶（Major/Minor/HarmonicMinor/MelodicMinor、7 教会调式、大小五声、Blues、WholeTone、减音阶、BebopDominant、Altered）
  - `Chord.cs` + `ChordFormulas`：18 种和弦（三和弦 + 七和弦 + 9 和弦）
  - `Fretboard.cs`：class 名 **`GuitarFretboard`**，非 `Fretboard`，避免与组件重名；支持 Standard/DropD/DADGAD
  - `Enums.cs`
- 组件：
  - `Components/Fretboard/Fretboard.razor`：SVG，品记、弦粗细、根音红圆点，点击发声
  - `Components/PianoKeyboard/PianoKeyboard.razor`
- 页面：`Home`、`FretboardPage`（根音/音阶/品数切换）、`PianoPage`（根音/和弦切换）
- Apple 风格布局：`MainLayout.razor` + `NavMenu.razor`，毛玻璃卡片，Tailwind `brand` 色板

### 阶段 2（commit `42636e1`，后两次微调 `f287fdf`、`40e0bb0`）

- `Services/LessonService.cs`：从 `wwwroot/lessons/index.json` 加载 manifest，按 `{moduleId}/{slug}.md` 拉取 Markdown
- `Services/ProgressService.cs`：localStorage 持久化完成状态
- `Components/Common/LessonParser.cs` + `LessonRenderer.cs`：
  - Markdig 渲染普通 Markdown
  - 自定义指令：
    - `:::fretboard root="C" scale="Major" frets=12`
    - `:::piano root="A" type="chord" name="Minor7"`
    - `:::chord root="C" quality="Major"`
    - `:::callout title="..."`
  - **关键坑**：组件类名与命名空间同名（`Fretboard` 组件在 `...Fretboard` 命名空间里），必须用 `using FretboardComponent = ...` 类型别名
- `Components/ChordDiagram/ChordDiagram.razor`：SVG 和弦框，×/○、手指号、点击发声
- `Theory/Voicing.cs`：12 个开放把位和弦（C D E G A / Am Em Dm / A7 D7 E7 C7）
- `Pages/LessonPage.razor`：路由 `/lessons/{ModuleId}/{Slug}`，含上/下一课、完成按钮
- `NavMenu` 改造：动态加载课程树，每模块显示 `done/total` 进度
- 模块 1 课程（`wwwroot/lessons/01-basics/`）6 节：
  1. `01-notes-and-intervals.md`
  2. `02-major-scale.md`
  3. `03-triads-and-7ths.md`
  4. `04-caged.md`
  5. `05-3nps.md`（**已修订两次**，Pattern 1 TAB 准确，2–7 号把位刻意留到模块 2 讲）
  6. `06-pentatonic-blues.md`
- 课程样式：`Styles/app.css` 里 `.prose-lesson` 排版（h2 左竖线、表格、code 块、callout）
- 测试新增 `LessonParserTests.cs`（5 个），总计 **35 个测试**

## 4. 关键约定 & 踩过的坑

1. **类名与命名空间冲突**：Razor 组件 `Fretboard.razor` 在命名空间 `FusionGuitar.Web.Components.Fretboard` 中，乐理类原名也是 `Fretboard`。已把乐理类改名为 `GuitarFretboard`，PianoKeyboard/ChordDiagram 在 Renderer 里用 `using XXComponent = ...` 别名。
2. **Blazor SVG `<text>` 标签冲突**：Blazor 把裸 `<text>` 当成自己的 `<text>` 容器（不能带属性），SVG 文本必须：
   - 包在 `<g>...</g>` 里，或
   - 用 `@:<text ...>...</text>` 显式标记，或
   - 不要嵌在 `@if`/块里
3. **`@bind:after="Refresh"`** 在 .NET 7+ 可用，用于 select 变化后刷新数据。
4. **Tailwind 构建**：csproj Target 监听 `Styles/app.css` 和 `tailwind.config.js`，输出到 `wwwroot/css/app.css`（**该文件 .gitignore 排除，构建时生成**）。新机器首次需要 `cd src/FusionGuitar.Web && npm install`。
5. **`:::` 指令解析器**：自定义最简实现，非 Markdig 扩展。支持 `key="value"`、`key='value'`、数字、布尔。不要加引号字符串里带空格的复杂语法。
6. **字符串 0 弦约定**：`GuitarFretboard` 和 `Voicing` 中 `StringIndex = 0` 是**低 E 弦（6 弦）**，5 是高 E 弦（1 弦）。
7. **B 弦偏移**：3NPS/CAGED 在 ③ 弦到 ② 弦整体后移 1 品，文档和组件都按此规则。
8. **`NavLink` href**：必须以 `/` 开头（`href="/"`、`href="fretboard"`），空字符串 `href=""` 会匹配所有。
9. **.gitignore 位置**：在 `FusionGuitar/` 子目录下（非仓库根），模式 `**/wwwroot/css/app.css` 才能匹配。
10. **.NET 模板版本差异**：.NET 10 `blazorwasm` 模板生成的是 `.slnx`（新 XML 解决方案格式），不是 `.sln`；命令用 `dotnet sln FusionGuitar.slnx add ...`。
11. **`Chord.Name` 截断**：最初用 `Root.ToString().TrimEnd('0'..'9')` 去 octave，对 `C#4` 等会误截，改用 `NoteLetters[Root.PitchClass]` 数组查表。

## 5. 目录结构（当前实际）

```
FUSION-GUITAR-PROJECT-PLAN/
├── FUSION-GUITAR-PROJECT-PLAN.md          # 原始规划
├── README.md                              # 阶段 1 后补的项目说明
└── FusionGuitar/
    ├── .gitignore
    ├── FusionGuitar.slnx
    ├── src/FusionGuitar.Web/
    │   ├── FusionGuitar.Web.csproj        # net10.0 + Markdig + Tailwind target
    │   ├── Program.cs                     # 注册 AudioInterop/LessonService/ProgressService
    │   ├── App.razor, Routes.razor, _Imports.razor
    │   ├── package.json, tailwind.config.js, Styles/app.css
    │   ├── Components/
    │   │   ├── Layout/{MainLayout,NavMenu}.razor
    │   │   ├── Fretboard/Fretboard.razor
    │   │   ├── PianoKeyboard/PianoKeyboard.razor
    │   │   ├── ChordDiagram/ChordDiagram.razor
    │   │   └── Common/{LessonParser.cs,LessonRenderer.cs}
    │   ├── Theory/                        # Note, Interval, Scale, Chord, GuitarFretboard, Voicing, Enums
    │   ├── Interop/AudioInterop.cs
    │   ├── Services/                      # LessonService, LessonModels, ProgressService
    │   ├── Pages/                         # Home, FretboardPage, PianoPage, LessonPage, NotFound
    │   └── wwwroot/
    │       ├── index.html                 # 引入 Tone@15 CDN
    │       ├── js/{audio.js,interop.js}
    │       └── lessons/
    │           ├── index.json             # 模块 1 课程目录
    │           └── 01-basics/*.md         # 6 节课
    └── tests/FusionGuitar.Tests/
        ├── FusionGuitar.Tests.csproj      # 引用 Web 项目 + Markdig
        ├── Theory/{Note,Scale,Chord,Fretboard}Tests.cs
        └── LessonParserTests.cs
```

## 6. 环境要求（新机器）

- **.NET SDK 10.0+**（或把 csproj 改 `net8.0`）
- **Node.js**（仅 Tailwind 构建用，版本不敏感，当前 v26.7.0）
- 可选：`wasm-tools` workload（发布时 AOT 优化，不装也能跑）

### 首次拉取后

```bash
cd FusionGuitar/src/FusionGuitar.Web
npm install

cd ../../..
dotnet build FusionGuitar/FusionGuitar.slnx
dotnet test FusionGuitar/FusionGuitar.slnx     # 应 35 passed
dotnet run --project FusionGuitar/src/FusionGuitar.Web
# 访问 http://localhost:5294
# 课程入口：/lessons/01-basics/01-notes-and-intervals
```

### 部署

```bash
dotnet publish FusionGuitar/src/FusionGuitar.Web -c Release -o ./publish
```

输出纯静态 WASM，可放 GitHub Pages / Cloudflare Pages / 任意静态托管。

## 7. 提交历史（按时间倒序）

```
40e0bb0  docs(lessons): simplify 3NPS pattern-2..7 section
f287fdf  docs(lessons): revise 3NPS lesson with accurate tab and pattern table
42636e1  feat(stage2): lesson framework, ChordDiagram, module 1 content
86fe6d4  chore: bootstrap Fusion Guitar stage 1
```

## 8. 下一步：阶段 3（尚未开始）

按计划文档：

1. `CircleOfFifths` 组件（SVG 五度圈，调关系 / 调式互换）
2. `HarmonyMap` 组件（和弦替代网络）
3. VexFlow 集成 → `Notation` 组件（五线谱 + TAB）
4. `AudioPlayer` 升级：Loop、变速（`Tone.Transport.bpm`）、节拍器、移调
5. 模块 2 课程（7 种教会调式 + 调式互换，7 节）
6. 模块 3 课程（大调 / 旋律小调 / 和声小调 Chord-Scale、全音/减音阶，7 节）

## 9. 风格 / 代码约定

- 不可变值类型 + 纯函数乐理，数据驱动（`IReadOnlyList<int>` 半音公式）
- Razor 组件用 SVG 手绘，不引入第三方 Blazor UI 库
- Apple 风格：大圆角 `rounded-2xl`、毛玻璃 `backdrop-blur-xl`、柔和阴影 `shadow-soft/pop`、`brand` 靛蓝色板
- 中文为主，术语首次出现附英文（如「音程 Intervals」）
- 提交信息用 Conventional Commits（`feat` / `docs` / `chore`，scope 用 `stage1` / `lessons` 等）
- **不主动写注释**，靠命名表达意图
- 不创建 `.env` / 密钥文件，所有配置硬编码或在 `appsettings`（暂无）

## 10. 已知遗留小问题

- 阶段 3 的 `<ChordDiagram>` 在课程里只显示开放把位；横按和弦 / Drop voicings 待阶段 4
- `audio.js` 和 `interop.js` 有重复代码，未来只保留 `interop.js` 即可
- `Home.razor` 的"基础夯实"卡片指向 `lessons/01-basics`，会自动跳到该模块第一课
- 深色模式 class 已留但还没有切换按钮（阶段 6）
- 移动端布局仅做了基础响应式（`md:` 断点），未深度适配
- 阶段 2 课程只覆盖模块 1，模块 2-7 内容待写
- `ProgressService` 用 `localStorage`，未做多设备同步（阶段 6 再考虑）
