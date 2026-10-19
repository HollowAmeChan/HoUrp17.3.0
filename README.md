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
| [HoTools](https://github.com/HollowAmeChan/HoTools) | Blender 角色、模型、贴图、动画与自动化工具 |
| [HoGLTF](https://github.com/HollowAmeChan/HoGLTF) | Blender glTF 导出与 Ho 材质契约桥接 |
| [HoUrpConfig17.0.3](https://github.com/HollowAmeChan/HoUrpConfig17.0.3) | URP Shader 配置 |
| [lilToon](https://github.com/HollowAmeChan/lilToon) | 角色 / NPR 着色器 |
| [lilToon-URP-Extensions](https://github.com/HollowAmeChan/lilToon-URP-Extensions) | 光照、阴影、反射与后处理扩展 |
| [lilToon-UnityGLTF-Extensions](https://github.com/HollowAmeChan/lilToon-UnityGLTF-Extensions) | glTF 材质契约导入 |
| [HoUnityTools](https://github.com/HollowAmeChan/HoUnityTools) | 角色、动画与创作工具 |

## 可选材质合集

- [HoShader](https://github.com/HollowAmeChan/HoShader)：宝石、皮肤、绒毛等独立材质与 Shader 实验合集，按需选用。
