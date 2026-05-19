# Ho Universal Render Pipeline 17.3.0

这个仓库是 Hollow 本地使用的 Unity Universal Render Pipeline 17.3.0 包副本。包名仍然是 `com.unity.render-pipelines.universal`，因此 Unity 项目指向这个本地目录后，会把它当作当前 URP 包解析。

## 在整套系统里的定位

`HoUrp17.3.0` 是 `D:/Unity_Fork` 这套渲染系统的 URP 基座。

它提供：

- URP runtime、editor、shader library、renderer feature、2D renderer、samples 和 tests。
- `lilToon-URP-Extensions` 依赖的 RenderGraph 与兼容模式 API。
- `lilToon`、`lilPBR`、`HoToon` 使用的 URP shader include 路径。
- 与 `HoUrpConfig17.0.3` 配套的本地 URP 运行时。

高层的 lilToon/lilPBR 实验功能应优先放在 `lilToon-URP-Extensions`，只有必须改 URP 内部时才改这个仓库。

## 重要目录

- `Runtime/`：URP renderer、frame data、passes、materials、settings、2D、XR、VFX 和 RenderGraph。
- `ShaderLibrary/`：URP shader 和下游 shader 包共享的 HLSL include。
- `Shaders/`：URP 内置 shader。
- `Editor/`：URP 编辑器 UI、转换器、shader stripping、构建检查等。
- `Samples~/`：URP 官方样例。
- `Tests/`：runtime/editor 测试。
- `Documentation~/`：包文档入口和 API 索引。

## 包身份

```json
{
  "name": "com.unity.render-pipelines.universal",
  "displayName": "Ho Universal Render Pipeline",
  "version": "17.3.0",
  "unity": "6000.3"
}
```

因为包名就是官方 URP 包名，同一个项目里不要再同时安装另一个 `com.unity.render-pipelines.universal`。

## 安装

在 Unity 项目的 `Packages/manifest.json` 中使用本地路径：

```json
{
  "dependencies": {
    "com.unity.render-pipelines.universal": "file:D:/Unity_Fork/HoUrp17.3.0",
    "com.unity.render-pipelines.universal-config": "file:D:/Unity_Fork/HoUrpConfig17.0.3"
  }
}
```

这个包仍需要兼容版本的：

- `com.unity.render-pipelines.core`
- `com.unity.shadergraph`

## 开发注意

- 改 URP 内部时，需要同时考虑 RenderGraph 和兼容模式路径。
- 下游包能解决的问题，不要直接塞进 URP fork。
- 当前 `Runtime/Materials/Lit.mat` 在 README 修改前已经有本地改动，请和文档改动分开处理。
