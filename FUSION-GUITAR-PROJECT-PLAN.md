# Fusion 电吉他乐理教材 + 可视化 Web 应用

## 一、项目概述

面向电吉他手的系统 Fusion 乐理教材，以 Web 应用形式呈现，支持交互式指板图、和弦图、音频播放、乐谱渲染等可视化功能。中文为主，术语附英文对照。

## 二、技术栈

| 层面 | 选型 | 说明 |
|------|------|------|
| 框架 | .NET 8 Blazor WebAssembly | C# 全栈前端，编译为 WebAssembly |
| 语言 | C# / Razor | 强类型，乐理引擎用 C# 建模 |
| 样式 | Tailwind CSS | 手写 Apple 风格 UI |
| 字体 | -apple-system / SF Pro 系统字体栈 | Apple 设计语言 |
| 图标 | Lucide（线性风格） | 接近 SF Symbols 风格 |
| 音频 | Tone.js（通过 JS Interop） | 合成器、Loop、变速、节拍器 |
| 乐谱 | VexFlow（通过 JS Interop） | 五线谱 + TAB |
| Markdown | Markdig | 课程内容解析，嵌入 Blazor 组件 |
| 状态管理 | Blazor 内置服务 / Fluxor | 全局状态、学习进度 |
| 路由 | Blazor Router | 页面导航 |
| 未来扩展 | PWA / .NET MAUI Blazor Hybrid | 离线使用、桌面/移动端复用 |

**视觉风格**：大圆角、柔和阴影、毛玻璃（backdrop-blur）、浅色/深色模式、充足留白、精致排版。

## 三、教材内容体系（7 大模块）

### 模块 1：基础夯实
- 音名与音程（Intervals）
- 大调音阶与自然音程
- 三和弦（Triads）与七和弦（7th Chords）
- CAGED 系统
- 3-notes-per-string（3NPS）指型
- 五声音阶与蓝调音阶（Pentatonic & Blues）

### 模块 2：调式理论
- 7 种教会调式（Ionian ~ Locrian）
- 各调式特征音与听觉色彩
- 调式指板全把位
- 调式互换（Modal Interchange）

### 模块 3：和弦-音阶理论（核心）
- 大调体系 Chord-Scale
- 旋律小调体系（Lydian Dominant、Altered Scale 等）
- 和声小调体系
- 全音音阶（Whole Tone）与减音阶（Diminished）

### 模块 4：高级和声
- 延伸音（9/11/13）与 Tension
- 变化和弦（Altered Chords）
- Drop2 / Drop3 Voicing
- Upper Structure Triads
- 三全音替代（Tritone Substitution）
- 次属和弦链（Secondary Dominants）
- Coltrane Changes

### 模块 5：Fusion 节奏与伴奏
- Comping 节奏型
- Slash Chord
- 线性演奏（Linear Playing）
- Funk / Latin / Jazz 律动

### 模块 6：即兴演奏
- Bebop 音阶
- 包围音（Enclosure）
- Chord Tone Targeting
- 模进（Sequences）
- Outside 演奏与 Side-slipping
- Lick 库（带乐谱 + 音频 + 指板）

### 模块 7：风格与大师分析
- Jazz Fusion（Chick Corea / Herbie Hancock）
- Allan Holdsworth
- Scott Henderson
- Guthrie Govan
- 其他代表风格与乐句分析

每个知识点结构：概念讲解 → 指板可视化 → 音频示范 → 练习乐句 → 即兴应用。

## 四、核心可视化组件

| 组件 | 功能 |
|------|------|
| `<Fretboard>` | SVG 指板图，高亮音阶/和弦/琶音，CAGED/3NPS 把位框，点击发声，多把位并排 |
| `<ChordDiagram>` | 标准和弦框，Voicing 展示，交互式构建 |
| `<PianoKeyboard>` | 钢琴键盘，辅助理解音程/和弦/音阶构造 |
| `<CircleOfFifths>` | 交互式五度圈，调关系、调式互换 |
| `<HarmonyMap>` | 和弦替代网络、调性中心图 |
| `<Notation>` | VexFlow 渲染五线谱 + TAB |
| `<AudioPlayer>` | 和弦进行 Loop、音阶/琶音示范、节拍器、变速、移调 |

## 五、乐理引擎设计（C# 数据层）

```
Theory/
  Note.cs           # 音名、MIDI 编号、频率换算，不可变结构体
  Interval.cs       # 音程质量与数量，模式匹配
  Scale.cs          # 音阶公式定义 + 音高生成
  Chord.cs          # 和弦公式（根音+音程集合）+ 生成
  Fretboard.cs      # 弦/品 → 音高映射，指板坐标
  ChordScale.cs     # 和弦-音阶匹配规则
  Voicing.cs        # 吉他和弦指型数据
  Progression.cs    # 和弦进行数据
  Enums.cs          # NoteName、IntervalQuality、ChordQuality 等枚举
```

**设计原则**：
- 全部公式数据驱动（如大调音阶 `[0,2,4,5,7,9,11]`）
- 支持自动 12 调移调
- 不可变值类型，纯函数，易单元测试
- 强类型枚举，避免魔法数字

## 六、JS Interop 层

C# 无法直接调用 Web Audio API 和 VexFlow，需要封装：

```
wwwroot/
  js/
    audio.js       # Tone.js 封装：播放音符、Loop、节拍器
    notation.js    # VexFlow 封装：渲染五线谱+TAB
    interop.js     # 统一入口
Interop/
  AudioInterop.cs  # C# 侧调用封装，IJSRuntime
  NotationInterop.cs
```

封装好之后，业务代码全部用 C# 调用，不直接写 JS。

## 七、课程内容方案

- 课程文件为 **Markdown**，存放在 `Content/Lessons/` 目录
- 使用 **Markdig** 解析 Markdown
- 通过 Markdig 扩展语法在 Markdown 中嵌入 Blazor 组件：

  ```markdown
  下面是 C 大调音阶在第五把位的指板图：

  :::fretboard scale="C major" position="5":::

  你可以点击任意音符试听。
  ```
- 编译器将自定义块转换为 Blazor 组件渲染

## 八、目录结构

```
FusionGuitar/
  FusionGuitar.sln
  src/
    FusionGuitar.Web/
      FusionGuitar.Web.csproj
      Program.cs
      App.razor
      Routes.razor
      _Imports.razor
      wwwroot/
        js/
          audio.js
          notation.js
          interop.js
        css/
          app.css
      Components/
        Layout/
          MainLayout.razor
          NavMenu.razor
        Fretboard/
          Fretboard.razor
          Fretboard.razor.cs
        ChordDiagram/
        PianoKeyboard/
        CircleOfFifths/
        HarmonyMap/
        Notation/
        AudioPlayer/
        Common/
      Theory/
        Note.cs
        Interval.cs
        Scale.cs
        Chord.cs
        Fretboard.cs
        ChordScale.cs
        Voicing.cs
        Progression.cs
        Enums.cs
      Interop/
        AudioInterop.cs
        NotationInterop.cs
      Content/
        Lessons/
          01-Basics/
          02-Modes/
          03-ChordScale/
          04-AdvancedHarmony/
          05-Rhythm/
          06-Improvisation/
          07-Masters/
        Data/
          scales.json
          chords.json
          voicings.json
          progressions.json
          licks.json
      Services/
        LessonService.cs
        ProgressService.cs
        SettingsService.cs
      Pages/
        Index.razor
        Lesson.razor
        Practice.razor
        Settings.razor
  tests/
    FusionGuitar.Tests/
      Theory/
        NoteTests.cs
        ScaleTests.cs
        ChordTests.cs
        FretboardTests.cs
```

## 九、实施阶段

### 阶段 1：基础框架与乐理引擎
- 安装 .NET 8 SDK
- 创建 Blazor WebAssembly 项目
- 集成 Tailwind CSS
- 实现乐理引擎（Note/Interval/Scale/Chord/Fretboard）
- 编写乐理引擎单元测试
- 实现 `<PianoKeyboard>` 组件（较简单，先练手）
- 实现 `<Fretboard>` 指板图核心组件
- 接入 Tone.js，点击音符发声

### 阶段 2：课程框架与基础内容
- 配置 Markdig + Blazor 组件嵌入
- 实现布局：侧边导航栏、课程目录、学习进度
- 实现 `<ChordDiagram>` 和弦图组件
- 完成模块 1（基础夯实）全部课程内容

### 阶段 3：高级可视化与音频
- 实现 `<CircleOfFifths>` 五度圈
- 实现 `<HarmonyMap>` 和声关系图
- 集成 VexFlow，实现 `<Notation>` 乐谱组件
- 升级 `<AudioPlayer>`：Loop、变速、节拍器、移调
- 完成模块 2（调式理论）
- 完成模块 3（和弦-音阶理论）

### 阶段 4：和声与节奏
- 实现 Drop2/Drop3 Voicing 引擎
- 完成模块 4（高级和声）
- 完成模块 5（Fusion 节奏与伴奏）
- 建立和弦进行库、Voicing 库

### 阶段 5：即兴与风格
- 完成模块 6（即兴演奏）
- Lick 库系统（乐谱 + 音频 + 指板联动）
- 完成模块 7（风格与大师分析）

### 阶段 6：打磨与发布
- 深色/浅色模式
- 移动端适配（手机/平板练琴场景）
- PWA 离线支持
- 性能优化
- 部署（GitHub Pages / 自有服务器）

## 十、开发环境准备

需要安装：

1. **.NET 8 SDK** — https://dotnet.microsoft.com/download
2. **Node.js**（Tailwind CSS 编译需要）— https://nodejs.org
3. 推荐 IDE：**Visual Studio 2022** 或 **JetBrains Rider** 或 **VS Code + C# 插件**

项目初始化命令（将在执行阶段运行）：
```bash
dotnet new blazorwasm -n FusionGuitar.Web -o src/FusionGuitar.Web
dotnet new xunit -n FusionGuitar.Tests -o tests/FusionGuitar.Tests
dotnet new sln -n FusionGuitar
```

## 十一、学习资源

- [Blazor 官方文档](https://learn.microsoft.com/zh-cn/aspnet/core/blazor/)
- [Blazor WebAssembly 教程](https://dotnet.microsoft.com/learn/aspnet/blazor-tutorial)
- [Tailwind CSS 文档](https://tailwindcss.com/)
- [Tone.js 文档](https://tonejs.github.io/)
- [VexFlow 教程](https://github.com/0xfe/vexflow)
- [Markdig 文档](https://github.com/xoofx/markdig)
