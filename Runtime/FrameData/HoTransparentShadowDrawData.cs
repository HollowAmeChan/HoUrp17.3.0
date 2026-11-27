using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>Camera-local screen shadow reads required by the regular transparent draw pass.</summary>
    public sealed class HoTransparentShadowDrawData : ContextItem
    {
        private readonly List<TextureHandle> textures = new();

        /// <summary>Registers a texture published before the camera's transparent draw pass.</summary>
        public void RegisterTexture(TextureHandle texture)
        {
            if (texture.IsValid() && !textures.Contains(texture))
                textures.Add(texture);
        }

        internal void DeclareReads(IRasterRenderGraphBuilder builder)
        {
            foreach (TextureHandle texture in textures)
                builder.UseTexture(texture, AccessFlags.Read);
        }

        /// <inheritdoc/>
        public override void Reset() => textures.Clear();
    }
}
