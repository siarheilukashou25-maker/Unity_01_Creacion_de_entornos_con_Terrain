using UnityEngine;

namespace WaterfallEffect.Editor.Assets
{
    /// <summary>
    /// Supplies a transparent particle material compatible with the active render pipeline.
    /// </summary>
    public interface IParticleMaterialProvider
    {
        Material GetSoftParticleMaterial();
    }
}
