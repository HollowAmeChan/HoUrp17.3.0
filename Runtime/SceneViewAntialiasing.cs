#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    /// <summary>
    /// Anti-aliasing mode used by the Scene View camera.
    /// </summary>
    /// <remarks>
    /// URP offers two kinds of anti-aliasing. MSAA is a pipeline asset setting that changes the sample
    /// count of the camera color target, while FXAA, SMAA and TAA are per camera modes that are applied
    /// by the post processing pass. The Scene View camera is created and owned by the editor: it has no
    /// UniversalAdditionalCameraData component and no page in the camera inspector, so it needs its own
    /// mode. MSAA is listed as a mode as well, to keep the Scene View choice exclusive in the same spirit
    /// as the HDRP Scene View camera settings.
    /// </remarks>
    internal enum SceneViewAntialiasing
    {
        /// <summary>No anti-aliasing is requested for the Scene View camera.</summary>
        None,

        /// <summary>Multisample anti-aliasing using the sample count of the currently used URP asset.</summary>
        MSAA,

        /// <summary>Fast approximate anti-aliasing, applied by the post processing pass.</summary>
        FastApproximateAntialiasing,

        /// <summary>Subpixel morphological anti-aliasing, applied by the post processing pass.</summary>
        SubpixelMorphologicalAntiAliasing,

        /// <summary>Temporal anti-aliasing, applied by the post processing pass.</summary>
        TemporalAntiAliasing,
    }

    /// <summary>
    /// Persistent (per user) Scene View anti-aliasing mode, edited from the Scene View toolbar overlay.
    /// </summary>
    internal static class SceneViewAntialiasingSettings
    {
        const string k_ModeKey = "Unity.Rendering.Universal.SceneViewAntialiasing";
        const string k_QualityKey = "Unity.Rendering.Universal.SceneViewAntialiasingQuality";
        const string k_TaaQualityKey = "Unity.Rendering.Universal.SceneViewAntialiasingTaaQuality";

        // Before this menu existed the Scene View had a single "MSAA" toggle. It is read once and
        // removed, so that an existing setup is migrated instead of silently reset to a default.
        const string k_LegacyMSAAKey = "Unity.Rendering.Universal.SceneViewMSAA";

        // Fast approximate anti-aliasing is the default: it is the cheapest post processing mode and it
        // does not need "Always Refresh", so the Scene View keeps its edge quality without holding a
        // history. MSAA is not the default because this pipeline prefers the post processing modes over
        // multisampling (see the GeometryBuffer coverage resolve). Both modes are still one click away in
        // the toolbar menu.
        const SceneViewAntialiasing k_DefaultMode = SceneViewAntialiasing.FastApproximateAntialiasing;
        const AntialiasingQuality k_DefaultQuality = AntialiasingQuality.High;

        internal static SceneViewAntialiasing mode
        {
            get
            {
                if (!EditorPrefs.HasKey(k_ModeKey))
                    EditorPrefs.SetInt(k_ModeKey, (int)MigrateLegacyMode());

                return (SceneViewAntialiasing)EditorPrefs.GetInt(k_ModeKey);
            }
            set
            {
                if (mode == value)
                    return;

                EditorPrefs.SetInt(k_ModeKey, (int)value);
                SceneView.RepaintAll();
            }
        }

        /// <summary>SMAA quality. TAA has its own quality, see <see cref="taaQuality"/>.</summary>
        internal static AntialiasingQuality quality
        {
            get => (AntialiasingQuality)EditorPrefs.GetInt(k_QualityKey, (int)k_DefaultQuality);
            set
            {
                if (quality == value)
                    return;

                EditorPrefs.SetInt(k_QualityKey, (int)value);
                SceneView.RepaintAll();
            }
        }

        /// <summary>
        /// TAA quality, the equivalent of the "Quality" setting of a camera with temporal anti-aliasing.
        /// </summary>
        internal static TemporalAAQuality taaQuality
        {
            get => (TemporalAAQuality)EditorPrefs.GetInt(k_TaaQualityKey, (int)TemporalAAQuality.High);
            set
            {
                if (taaQuality == value)
                    return;

                EditorPrefs.SetInt(k_TaaQualityKey, (int)value);
                SceneViewCameraStates.SetTaaQuality(value);
                SceneView.RepaintAll();
            }
        }

        /// <summary>
        /// True when the mode is applied by the post processing pass, which means the Scene View needs
        /// "Post Processing" enabled in its "Effects" menu to have any effect.
        /// </summary>
        internal static bool requiresPostProcessing => mode == SceneViewAntialiasing.FastApproximateAntialiasing
                                                       || mode == SceneViewAntialiasing.SubpixelMorphologicalAntiAliasing
                                                       || mode == SceneViewAntialiasing.TemporalAntiAliasing;

        /// <summary>True when the mode accumulates a history and therefore needs the camera history container.</summary>
        internal static bool usesTemporalAA => mode == SceneViewAntialiasing.TemporalAntiAliasing;

        /// <summary>
        /// The sample count the Scene View renders with when <see cref="SceneViewAntialiasing.MSAA"/> is
        /// selected. There is nothing to render with MSAA when the asset disables it.
        /// </summary>
        internal static int msaaSamples
        {
            get
            {
                UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
                return asset != null ? asset.msaaSampleCount : 1;
            }
        }

        /// <summary>Maps the Scene View mode onto the per camera post processing mode.</summary>
        internal static AntialiasingMode cameraAntialiasingMode
        {
            get
            {
                switch (mode)
                {
                    case SceneViewAntialiasing.FastApproximateAntialiasing:
                        return AntialiasingMode.FastApproximateAntialiasing;
                    case SceneViewAntialiasing.SubpixelMorphologicalAntiAliasing:
                        return AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    case SceneViewAntialiasing.TemporalAntiAliasing:
                        return AntialiasingMode.TemporalAntiAliasing;
                    default:
                        // None and MSAA are not post processing modes.
                        return AntialiasingMode.None;
                }
            }
        }

        static SceneViewAntialiasing MigrateLegacyMode()
        {
            SceneViewAntialiasing migrated = k_DefaultMode;

            // The old toggle could only express "use MSAA in the Scene View" or "do not": there was no
            // other mode to pick, so "do not" was not a request for no anti-aliasing at all. It therefore
            // maps onto the current default, while the enabled state keeps MSAA.
            if (EditorPrefs.HasKey(k_LegacyMSAAKey))
            {
                migrated = EditorPrefs.GetBool(k_LegacyMSAAKey) ? SceneViewAntialiasing.MSAA : k_DefaultMode;
                EditorPrefs.DeleteKey(k_LegacyMSAAKey);
            }

            return migrated;
        }
    }

    /// <summary>
    /// Per camera state that UniversalAdditionalCameraData owns for game cameras, for one Scene View camera.
    /// </summary>
    /// <remarks>
    /// TAA needs a camera history container, persistent motion data and TAA settings. Game cameras get
    /// those from the UniversalAdditionalCameraData component, which URP adds to every camera it renders.
    /// The Scene View camera is created by the editor and never gets that component, and attaching one to
    /// an editor owned GameObject would also turn on everything else that component does (gizmo drawing,
    /// volume stack, camera stacking). Keeping the state next to the Scene View settings leaves that
    /// GameObject untouched. The history owns render textures, so it is released with the pipeline.
    /// </remarks>
    internal sealed class SceneViewCameraState
    {
        UniversalCameraHistory m_History;
        MotionVectorsPersistentData m_MotionVectorsPersistentData;
        TemporalAA.Settings m_TaaSettings;

        internal SceneViewCameraState()
        {
            m_TaaSettings = TemporalAA.Settings.Create();
            m_TaaSettings.quality = SceneViewAntialiasingSettings.taaQuality;
        }

        internal UniversalCameraHistory history => m_History ??= new UniversalCameraHistory();

        internal MotionVectorsPersistentData motionVectorsPersistentData =>
            m_MotionVectorsPersistentData ??= new MotionVectorsPersistentData();

        internal ref TemporalAA.Settings taaSettings => ref m_TaaSettings;

        internal void SetTaaQuality(TemporalAAQuality value) => m_TaaSettings.quality = value;

        internal void Dispose()
        {
            m_History?.Dispose();
            m_History = null;
            m_MotionVectorsPersistentData = null;
        }
    }

    /// <summary>
    /// The <see cref="SceneViewCameraState"/> of every Scene View that used temporal anti-aliasing.
    /// </summary>
    /// <remarks>
    /// The state is per camera: a Scene View camera has its own history and its own motion data, exactly
    /// like a game camera has its own UniversalAdditionalCameraData. Sharing one state between the cameras
    /// of a split Scene View layout would make them blend into each other's history.
    /// </remarks>
    internal static class SceneViewCameraStates
    {
        static readonly Dictionary<int, SceneViewCameraState> s_States = new Dictionary<int, SceneViewCameraState>();

        internal static SceneViewCameraState Get(Camera camera)
        {
            int cameraId = camera.GetInstanceID();

            if (!s_States.TryGetValue(cameraId, out SceneViewCameraState state))
            {
                state = new SceneViewCameraState();
                s_States.Add(cameraId, state);
            }

            return state;
        }

        internal static void SetTaaQuality(TemporalAAQuality value)
        {
            foreach (var state in s_States.Values)
                state.SetTaaQuality(value);
        }

        /// <summary>
        /// Releases the state of a Scene View camera that no longer uses temporal anti-aliasing, which
        /// frees its history textures. This is the Scene View equivalent of the history reset URP performs
        /// for a game camera that turns temporal anti-aliasing off.
        /// </summary>
        internal static void Release(Camera camera)
        {
            int cameraId = camera.GetInstanceID();

            if (s_States.TryGetValue(cameraId, out SceneViewCameraState state))
            {
                state.Dispose();
                s_States.Remove(cameraId);
            }
        }

        internal static void Dispose()
        {
            foreach (var state in s_States.Values)
                state.Dispose();

            s_States.Clear();
        }
    }
}
#endif
