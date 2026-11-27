using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Describes the additional lights with realtime slices in this camera's Render Graph shadow atlas.
    /// The data is valid only for the current camera during graph recording.
    /// </summary>
    public sealed class AdditionalLightShadowAtlasData : ContextItem
    {
        /// <summary>A light with one or more accepted native shadow atlas slices.</summary>
        public readonly struct Entry
        {
            /// <summary>The light's zero-based position in native atlas publication order.</summary>
            public int Rank { get; }

            /// <summary>
            /// Index into the native additional shadow parameters. In Forward and Forward+, this
            /// is also the global additional light shader index.
            /// </summary>
            public int ShaderLightIndex { get; }

            /// <summary>Index into <see cref="UniversalLightData.visibleLights"/> for this camera.</summary>
            public int VisibleLightIndex { get; }

            /// <summary>The first index in the native world-to-shadow slice table for this light.</summary>
            public int FirstShadowSlice { get; }

            /// <summary>The number of accepted consecutive slices in that table.</summary>
            public int ShadowSliceCount { get; }

            /// <summary>
            /// Bit mask of accepted per-light slices. A spot light uses bit zero and a point light
            /// uses its cubemap face indices.
            /// </summary>
            public uint ShadowSliceMask { get; }

            internal Entry(int rank, int shaderLightIndex, int visibleLightIndex, int firstShadowSlice,
                int shadowSliceCount, uint shadowSliceMask)
            {
                Rank = rank;
                ShaderLightIndex = shaderLightIndex;
                VisibleLightIndex = visibleLightIndex;
                FirstShadowSlice = firstShadowSlice;
                ShadowSliceCount = shadowSliceCount;
                ShadowSliceMask = shadowSliceMask;
            }
        }

        readonly List<Entry> m_Entries = new();

        /// <summary>The number of lights with realtime shadow atlas slices.</summary>
        public int Count => m_Entries.Count;

        /// <summary>Whether native shadow indices also address the camera's Forward light data.</summary>
        public bool UsesForwardLightIndices { get; internal set; }

        /// <summary>Gets a light by its rank in native atlas publication order.</summary>
        public Entry this[int rank] => m_Entries[rank];

        /// <summary>Finds a light using its native additional shadow parameters index.</summary>
        /// <param name="shaderLightIndex">The native additional shadow parameters index.</param>
        /// <param name="entry">The light's atlas entry when present.</param>
        /// <returns>True when the light has a published entry for this camera.</returns>
        public bool TryGetByShaderLightIndex(int shaderLightIndex, out Entry entry)
        {
            for (int rank = 0; rank < m_Entries.Count; ++rank)
            {
                Entry candidate = m_Entries[rank];
                if (candidate.ShaderLightIndex == shaderLightIndex)
                {
                    entry = candidate;
                    return true;
                }
            }

            entry = default;
            return false;
        }

        internal void Add(int shaderLightIndex, int visibleLightIndex, int firstShadowSlice,
            int shadowSliceCount, uint shadowSliceMask)
        {
            m_Entries.Add(new Entry(m_Entries.Count, shaderLightIndex, visibleLightIndex, firstShadowSlice,
                shadowSliceCount, shadowSliceMask));
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            m_Entries.Clear();
            UsesForwardLightIndices = false;
        }
    }
}
