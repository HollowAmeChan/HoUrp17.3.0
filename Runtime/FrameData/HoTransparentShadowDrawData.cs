using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>Camera-local screen shadow reads for transparent surfaces in each draw phase.</summary>
    public sealed class HoTransparentShadowDrawData : ContextItem
    {
        private readonly List<TextureHandle> textures = new();
        private readonly List<TextureHandle> opaqueQueueTextures = new();

        /// <summary>Registers a texture published before the camera's transparent draw pass.</summary>
        public void RegisterTexture(TextureHandle texture) => Register(textures, texture);

        /// <summary>Registers a layered shadow texture used by alpha-blended materials in the opaque queue.</summary>
        public void RegisterOpaqueQueueTexture(TextureHandle texture) => Register(opaqueQueueTextures, texture);

        private static void Register(List<TextureHandle> target, TextureHandle texture)
        {
            if (texture.IsValid() && !target.Contains(texture))
                target.Add(texture);
        }

        internal void DeclareReads(IRasterRenderGraphBuilder builder, bool opaqueQueue)
        {
            foreach (TextureHandle texture in opaqueQueue ? opaqueQueueTextures : textures)
                builder.UseTexture(texture, AccessFlags.Read);
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            textures.Clear();
            opaqueQueueTextures.Clear();
        }
    }
}
