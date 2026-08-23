# Fusion Guitar

面向电吉他手的系统 Fusion / Jazz 乐理教材，以交互式 Web 应用形式呈现：可点击发声的指板图、钢琴键盘、和弦图，配合音频示范与乐谱渲染。中文为主，术语附英文对照。

> 当前处于 **阶段 1**：乐理引擎 + 基础可视化组件已完成。详细路线图见 [FUSION-GUITAR-PROJECT-PLAN.md](./FUSION-GUITAR-PROJECT-PLAN.md)。

## 技术栈

- [.NET 10 Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)（C# / Razor，编译为 WebAssembly）
- [Tailwind CSS](https://tailwindcss.com/)（Apple 风格 UI，毛玻璃 / 大圆角 / 深浅色模式预留）
- [Tone.js](https://tonejs.github.io/)（经 JS Interop 调用，音频合成）
- xUnit（乐理引擎单元测试）

## 目录结构

```
FusionGuitar/
├── src/FusionGuitar.Web/
│   ├── Theory/            # 乐理引擎：Note / Interval / Scale / Chord / GuitarFretboard
│   ├── Components/        # Razor 组件：Fretboard / PianoKeyboard / Layout
│   ├── Interop/           # C# → JS 封装（AudioInterop）
│   ├── Pages/             # 路由页面
│   ├── wwwroot/js/        # Tone.js 封装
│   └── Styles/app.css     # Tailwind 入口
└── tests/FusionGuitar.Tests/
    └── Theory/            # 乐理引擎单元测试
```

## 本地开发

### 环境要求

- [.NET SDK](https://dotnet.microsoft.com/download)（项目文件目标 `net10.0`，可按需改为 `net8.0`）
- [Node.js](https://nodejs.org/)（仅用于编译 Tailwind CSS）

### 首次安装与运行

```bash
# 安装 Tailwind 依赖（仅首次）
cd src/FusionGuitar.Web
npm install

# 运行开发服务器（首次构建会自动编译 Tailwind）
dotnet run

# 或在仓库根目录运行测试
dotnet test FusionGuitar/FusionGuitar.slnx
```

构建时 MSBuild target 会自动执行 `npm run tailwind:build`。若需实时编辑样式，另开终端：

```bash
cd src/FusionGuitar.Web
npm run tailwind:watch
```

## 阶段 1 已完成内容

- 乐理引擎：音名 / MIDI / 频率、音程、19 种音阶与教会调式、18 种和弦公式、指板映射（支持 Standard / Drop D / DADGAD）
- 单元测试：30 个测试全部通过
- 组件：`<Fretboard>`（SVG 指板图，品记 / 弦粗细 / 根音高亮 / 点击发声）、`<PianoKeyboard>`（钢琴键盘，和弦 / 音阶高亮）
- 页面：首页、指板工作台（根音 / 音阶 / 品数切换）、钢琴工作台（根音 / 和弦切换）
- Apple 风格布局、毛玻璃侧边导航、深浅色样式基础

## 后续路线

- 阶段 2：Markdig + Blazor 组件嵌入、课程框架、`<ChordDiagram>`、模块 1 课程内容
- 阶段 3：`<CircleOfFifths>`、`<HarmonyMap>`、VexFlow 乐谱、升级版 `<AudioPlayer>`
- 阶段 4：Drop2/Drop3 Voicing 引擎、和弦进行 / Voicing 库
- 阶段 5：Lick 库（乐谱 + 音频 + 指板联动）、大师风格分析
- 阶段 6：深色模式完善、移动端适配、PWA、部署

## License

暂未指定开源协议（All Rights Reserved）。如需协作或再使用，请联系作者。
