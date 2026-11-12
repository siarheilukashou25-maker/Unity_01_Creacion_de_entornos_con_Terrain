using UnityEngine;

namespace WaterfallEffect.Layers
{
    /// <summary>
    /// Small factory helpers for particle system curves and gradients.
    /// </summary>
    internal static class ParticleCurves
    {
        private const float MaxParticlesSafetyFactor = 1.2f;
        private const int MaxParticlesPadding = 16;

        public static ParticleSystem.MinMaxCurve Range(float min, float max)
        {
            return new ParticleSystem.MinMaxCurve(min, max);
        }

        public static ParticleSystem.MinMaxCurve Linear(float start, float end)
        {
            return new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, start, 1f, end));
        }

        /// <summary>White gradient with the given alpha keys; the tint comes from the start colour.</summary>
        public static ParticleSystem.MinMaxGradient AlphaFade(params GradientAlphaKey[] alphaKeys)
        {
            var gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                alphaKeys);

            return new ParticleSystem.MinMaxGradient(gradient);
        }

        /// <summary>Enough particles for the steady state of a looping emitter, with some headroom.</summary>
        public static int MaxParticles(float ratePerSecond, float maxLifetime)
        {
            return Mathf.CeilToInt(ratePerSecond * maxLifetime * MaxParticlesSafetyFactor) + MaxParticlesPadding;
        }
    }
}
