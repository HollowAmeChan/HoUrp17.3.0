using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// A renderer feature may own the image effects while retaining the pipeline's
    /// anti-aliasing, upscaling and final display encoding.
    /// Evaluated separately for each camera after its Volume stack is updated.
    /// </summary>
    public interface IHoPostProcessingProvider
    {
        bool ReplacesBuiltinPostProcessing(Camera camera);
    }

    /// <summary>Last recorded post pass, keyed by camera rather than editor focus.</summary>
    [System.Serializable]
    public struct HoPostProcessingExecution
    {
        public int revision;
        public string backend, antialiasing;
        public bool hoOnly, nativeColorLut, temporalAA;
    }

    public static class HoPostProcessingDiagnostics
    {
        static readonly Dictionary<int, HoPostProcessingExecution> executions = new();
        static int revision;

        public static bool TryGet(Camera camera, out HoPostProcessingExecution execution)
        {
            execution = default;
            return camera != null && executions.TryGetValue(camera.GetInstanceID(), out execution);
        }

        internal static void Record(UniversalCameraData cameraData, string backend, bool nativeColorLut)
        {
            if (executions.Count > 48) executions.Clear();
            executions[cameraData.camera.GetInstanceID()] = new HoPostProcessingExecution {
                revision = ++revision, backend = backend,
                antialiasing = cameraData.antialiasing.ToString(),
                hoOnly = cameraData.useHoPostProcessing,
                nativeColorLut = nativeColorLut,
                temporalAA = cameraData.IsTemporalAAEnabled()
            };
        }
    }
}
