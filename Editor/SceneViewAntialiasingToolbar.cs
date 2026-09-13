#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace UnityEditor.Rendering.Universal
{
    [Overlay(
        typeof(SceneView),
        k_Id,
        "URP",
        true,
        defaultDisplay = true,
        defaultDockZone = DockZone.TopToolbar,
        defaultLayout = Layout.HorizontalToolbar
    )]
    internal class SceneViewAntialiasingToolbar : ToolbarOverlay
    {
        // The overlay keeps the id it had as the "MSAA" toggle, so that existing toolbar layouts
        // still find the entry now that the toggle became a mode menu.
        const string k_Id = "Unity.Rendering.Universal.SceneViewMSAAToolbar";

        SceneViewAntialiasingToolbar()
            : base(SceneViewAntialiasingMenu.k_Id)
        {
        }
    }

    /// <summary>
    /// Scene View toolbar entry that selects the anti-aliasing mode of the Scene View camera.
    /// </summary>
    /// <remarks>
    /// The mode is exclusive, like the anti-aliasing setting of a game camera and like the Scene View
    /// camera settings of HDRP: MSAA comes from the URP asset while FXAA, SMAA and TAA are applied by the
    /// post processing pass. The post processing modes need "Post Processing" enabled in the Scene View
    /// "Effects" menu, which is not exposed to script, so the menu reports that state instead of changing
    /// it. "Always Refresh" is exposed, so it is offered for TAA, which converges while the view keeps
    /// rendering.
    /// </remarks>
    [EditorToolbarElement(k_Id, typeof(SceneView))]
    internal class SceneViewAntialiasingMenu : EditorToolbarDropdown
    {
        internal const string k_Id = "Unity.Rendering.Universal.SceneViewAntialiasingMenu";

        static readonly HashSet<SceneViewAntialiasingMenu> s_Menus = new();

        SceneViewAntialiasingMenu()
        {
            style.fontSize = 10;
            style.unityFontStyleAndWeight = FontStyle.Bold;

            UpdateLabel();
            clicked += ShowMenu;
            RegisterCallback<AttachToPanelEvent>(_ => UpdateLabel());
            RegisterCallback<DetachFromPanelEvent>(_ => s_Menus.Remove(this));

            // The label follows the stored mode. Menu entries refresh every open toolbar directly, the
            // schedule also covers the migration of the old toggle and any other Scene View toolbar.
            schedule.Execute(UpdateLabel).Every(500);

            s_Menus.Add(this);
        }

        static SceneView activeSceneView => SceneView.lastActiveSceneView;

        static bool isPostProcessingEnabled
        {
            get
            {
                SceneView sceneView = activeSceneView;
                return sceneView != null && sceneView.camera != null &&
                       UnityEngine.Rendering.CoreUtils.ArePostProcessesEnabled(sceneView.camera);
            }
        }

        static void RefreshMenus()
        {
            foreach (var menu in s_Menus)
                menu.UpdateLabel();
        }

        void UpdateLabel()
        {
            text = $"抗锯齿: {ShortName(SceneViewAntialiasingSettings.mode)}";
            tooltip = SceneViewAntialiasingSettings.requiresPostProcessing && !isPostProcessingEnabled
                ? "场景视图相机的抗锯齿模式。当前模式由后处理实现，需要在场景视图的 Effects 菜单里开启 Post Processing 才会生效。"
                : "场景视图相机的抗锯齿模式。";
        }

        void ShowMenu()
        {
            var menu = new GenericMenu();

            SceneView sceneView = activeSceneView;
            bool postProcessing = isPostProcessingEnabled;

            menu.AddDisabledItem(new GUIContent("场景视图抗锯齿"));
            menu.AddSeparator(string.Empty);

            AddMode(menu, SceneViewAntialiasing.None, "关闭");
            AddMode(menu, SceneViewAntialiasing.MSAA, $"MSAA（{MSAASourceLabel()}）");
            AddMode(menu, SceneViewAntialiasing.FastApproximateAntialiasing, "快速近似（FXAA）");
            AddMode(menu, SceneViewAntialiasing.SubpixelMorphologicalAntiAliasing, "亚像素形态（SMAA）");
            AddMode(menu, SceneViewAntialiasing.TemporalAntiAliasing, "时域抗锯齿（TAA）");

            menu.AddSeparator(string.Empty);

            AddQuality(menu);

            menu.AddSeparator(string.Empty);

            AddSceneViewState(menu, sceneView, postProcessing);

            menu.DropDown(worldBound);
        }

        static void AddMode(GenericMenu menu, SceneViewAntialiasing value, string label)
        {
            menu.AddItem(new GUIContent(label), SceneViewAntialiasingSettings.mode == value, () =>
            {
                SceneViewAntialiasingSettings.mode = value;
                RefreshMenus();
            });
        }

        static void AddQuality(GenericMenu menu)
        {
            // SMAA quality, the same setting the camera inspector shows for subpixel morphological AA.
            AddRadio(menu, "SMAA 质量/低", SceneViewAntialiasingSettings.quality == AntialiasingQuality.Low,
                () => SceneViewAntialiasingSettings.quality = AntialiasingQuality.Low);
            AddRadio(menu, "SMAA 质量/中", SceneViewAntialiasingSettings.quality == AntialiasingQuality.Medium,
                () => SceneViewAntialiasingSettings.quality = AntialiasingQuality.Medium);
            AddRadio(menu, "SMAA 质量/高", SceneViewAntialiasingSettings.quality == AntialiasingQuality.High,
                () => SceneViewAntialiasingSettings.quality = AntialiasingQuality.High);

            // TAA quality, the same setting the camera inspector shows for temporal AA.
            AddRadio(menu, "TAA 质量/极低", SceneViewAntialiasingSettings.taaQuality == TemporalAAQuality.VeryLow,
                () => SceneViewAntialiasingSettings.taaQuality = TemporalAAQuality.VeryLow);
            AddRadio(menu, "TAA 质量/低", SceneViewAntialiasingSettings.taaQuality == TemporalAAQuality.Low,
                () => SceneViewAntialiasingSettings.taaQuality = TemporalAAQuality.Low);
            AddRadio(menu, "TAA 质量/中", SceneViewAntialiasingSettings.taaQuality == TemporalAAQuality.Medium,
                () => SceneViewAntialiasingSettings.taaQuality = TemporalAAQuality.Medium);
            AddRadio(menu, "TAA 质量/高", SceneViewAntialiasingSettings.taaQuality == TemporalAAQuality.High,
                () => SceneViewAntialiasingSettings.taaQuality = TemporalAAQuality.High);
            AddRadio(menu, "TAA 质量/极高", SceneViewAntialiasingSettings.taaQuality == TemporalAAQuality.VeryHigh,
                () => SceneViewAntialiasingSettings.taaQuality = TemporalAAQuality.VeryHigh);
        }

        static void AddRadio(GenericMenu menu, string path, bool selected, Action setter)
        {
            menu.AddItem(new GUIContent(path), selected, () =>
            {
                setter();
                RefreshMenus();
            });
        }

        static void AddSceneViewState(GenericMenu menu, SceneView sceneView, bool postProcessing)
        {
            // "Post Processing" in the Scene View "Effects" menu cannot be set from script, so this only
            // reports it. FXAA, SMAA and TAA have no effect while it is off.
            menu.AddDisabledItem(new GUIContent($"场景视图后处理（Effects 菜单）：{(postProcessing ? "开" : "关")}"));

            if (SceneViewAntialiasingSettings.requiresPostProcessing && !postProcessing)
                menu.AddDisabledItem(new GUIContent("FXAA、SMAA、TAA 需要开启场景视图后处理"));

            if (sceneView == null || !SceneViewAntialiasingSettings.usesTemporalAA)
                return;

            // TAA converges while the Scene View keeps rendering, which is what "Always Refresh" does.
            bool alwaysRefresh = sceneView.sceneViewState.alwaysRefreshEnabled;
            menu.AddItem(new GUIContent("始终刷新（TAA 才能收敛）"), alwaysRefresh, () =>
            {
                sceneView.sceneViewState.alwaysRefresh = !alwaysRefresh;
                sceneView.Repaint();
            });
        }

        static string MSAASourceLabel()
        {
            int samples = SceneViewAntialiasingSettings.msaaSamples;
            return samples > 1 ? $"URP Asset {samples}x" : "URP Asset 未开启";
        }

        static string ShortName(SceneViewAntialiasing value)
        {
            switch (value)
            {
                case SceneViewAntialiasing.None:
                    return "关闭";
                case SceneViewAntialiasing.MSAA:
                    return "MSAA";
                case SceneViewAntialiasing.FastApproximateAntialiasing:
                    return "FXAA";
                case SceneViewAntialiasing.SubpixelMorphologicalAntiAliasing:
                    return "SMAA";
                case SceneViewAntialiasing.TemporalAntiAliasing:
                    return "TAA";
                default:
                    return value.ToString();
            }
        }
    }
}
#endif
