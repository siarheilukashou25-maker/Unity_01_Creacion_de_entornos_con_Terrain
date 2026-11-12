using UnityEngine;

namespace WaterfallEffect.Layers
{
    /// <summary>
    /// Configures one particle system of the waterfall from the shared settings and geometry.
    /// </summary>
    public interface IWaterfallLayer
    {
        void Configure(ParticleSystem system, WaterfallSettings settings, WaterfallGeometry geometry);
    }
}
