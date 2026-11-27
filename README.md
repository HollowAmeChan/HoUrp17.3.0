# Ho Universal Render Pipeline 17.3.0

Ho 渲染体系的 URP 基座，基于 Unity URP 17.3.0 维护，供材质与渲染扩展使用。

## 功能

- **URP 渲染基础**：前向 / Forward+、延迟渲染、灯光、阴影、探针、后处理与 Shader Library。
- **扩展接口**：RendererFeature、自定义渲染 pass、RenderGraph 与兼容模式接口。
- **Scene View 抗锯齿**：工具栏可切换 Off、MSAA、FXAA、SMAA、TAA；TAA 可配合 Always Refresh 持续收敛。
- **明确的 pass 排序**：同一渲染事件按入队顺序执行，便于组织 Ho 各模块的先后依赖。
- **反射探针更新**：更换 Cubemap 或分辨率时刷新反射图集缓存。

面向 Unity 6000.3，包名保留 `com.unity.render-pipelines.universal`。包文档见 [Documentation~](Documentation~/)。

## Ho 渲染体系相关仓库

| 仓库 | 用途 |
| --- | --- |
| [HoUrpConfig17.0.3](https://github.com/HollowAmeChan/HoUrpConfig17.0.3) | URP Shader 配置 |
| [lilToon-URP-Extensions](https://github.com/HollowAmeChan/lilToon-URP-Extensions) | 光照、阴影、反射与后处理扩展 |
| HoShader | 下一代材质底座，项目正在迁移，仓库目前为私有 |
| [HoUnityTools](https://github.com/HollowAmeChan/HoUnityTools) | 角色、动画与创作工具 |

## 日期版本与安装包

推送默认分支后自动发布日期版本（`YYYYMMDD-HHMMSS`）与 Unity 安装包；在 GitHub Releases 下载 `.tgz`，通过 Package Manager 从 tarball 安装。版本对齐、手动发布与安装说明见 [发布说明](.github/RELEASING.md)。
