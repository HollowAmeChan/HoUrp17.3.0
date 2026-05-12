#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
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
    internal class SceneViewMSAAToolbar : ToolbarOverlay
    {
        const string k_Id = "Unity.Rendering.Universal.SceneViewMSAAToolbar";

        SceneViewMSAAToolbar()
            : base(SceneViewMSAAToggle.k_Id)
        {
        }
    }

    [EditorToolbarElement(k_Id, typeof(SceneView))]
    internal class SceneViewMSAAToggle : ToolbarToggle
    {
        internal const string k_Id = "Unity.Rendering.Universal.SceneViewMSAAToggle";
        static readonly HashSet<SceneViewMSAAToggle> s_Toggles = new();

        SceneViewMSAAToggle()
        {
            text = "MSAA";
            tooltip = "Use the Universal Render Pipeline Asset MSAA setting in the Scene View.";
            style.fontSize = 10;
            style.unityFontStyleAndWeight = FontStyle.Bold;

            SetValueWithoutNotify(SceneViewMSAASettings.enabled);
            RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                SceneViewMSAASettings.enabled = evt.newValue;
                foreach (var toggle in s_Toggles)
                    toggle.SetValueWithoutNotify(evt.newValue);
            });
            RegisterCallback<DetachFromPanelEvent>(_ => s_Toggles.Remove(this));

            s_Toggles.Add(this);
        }
    }
}
#endif
