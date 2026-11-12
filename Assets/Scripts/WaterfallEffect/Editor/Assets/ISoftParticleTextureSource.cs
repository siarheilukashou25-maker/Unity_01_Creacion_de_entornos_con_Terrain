using UnityEngine;

namespace WaterfallEffect.Editor.Assets
{
    /// <summary>
    /// Supplies the round soft-edged sprite used by all waterfall particles.
    /// </summary>
    public interface ISoftParticleTextureSource
    {
        Texture2D GetTexture();
    }
}
