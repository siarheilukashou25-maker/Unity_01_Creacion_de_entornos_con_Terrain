using UnityEngine;

namespace WaterfallEffect.Layers
{
    /// <summary>
    /// The falling water curtain: stretched particles thrown forward off the edge and pulled down by gravity.
    /// Lifetime equals the fall time, so particles disappear exactly at the landing point.
    /// </summary>
    public sealed class StreamLayer : IWaterfallLayer
    {
        private const float RatePerMetre = 60f;
        private const float SpeedSpread = 0.2f;
        private const float SizePerMetre = 0.2f;
        private const float MinSize = 0.4f;
        private const float MaxSize = 2f;
        private const float SizeSpread = 0.3f;
        private const float EndSizeMultiplier = 1.8f;
        private const float EmitterThickness = 0.2f;
        private const float NoiseFrequency = 0.5f;
        private const float NoiseScrollSpeed = 0.5f;
        private const float StretchLengthScale = 1f;
        private const float StretchVelocityScale = 0.05f;

        public void Configure(ParticleSystem system, WaterfallSettings settings, WaterfallGeometry geometry)
        {
            system.transform.localPosition = Vector3.zero;
            system.transform.localRotation = Quaternion.identity;

            float rate = RatePerMetre * settings.Width * settings.FlowIntensity;
            float size = Mathf.Clamp(settings.Width * SizePerMetre, MinSize, MaxSize);

            ParticleSystem.MainModule main = system.main;
            main.loop = true;
            main.prewarm = true;
            main.playOnAwake = true;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.startLifetime = geometry.FallTime;
            main.startSpeed = ParticleCurves.Range(
                settings.ForwardSpeed * (1f - SpeedSpread),
                settings.ForwardSpeed * (1f + SpeedSpread));
            main.startSize = ParticleCurves.Range(size * (1f - SizeSpread), size * (1f + SizeSpread));
            main.startColor = settings.WaterColor;
            main.gravityModifier = WaterfallGeometry.StreamGravityModifier;
            main.maxParticles = ParticleCurves.MaxParticles(rate, geometry.FallTime);

            ParticleSystem.EmissionModule emission = system.emission;
            emission.enabled = true;
            emission.rateOverTime = rate;

            // A box emits along its local +Z, i.e. forward off the edge.
            ParticleSystem.ShapeModule shape = system.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.position = Vector3.zero;
            shape.rotation = Vector3.zero;
            shape.scale = new Vector3(settings.Width, EmitterThickness, EmitterThickness);
            shape.randomDirectionAmount = 0f;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = system.colorOverLifetime;
            colorOverLifetime.enabled = true;
            colorOverLifetime.color = ParticleCurves.AlphaFade(
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.8f, 0.9f),
                new GradientAlphaKey(0f, 1f));

            ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = system.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = ParticleCurves.Linear(1f, EndSizeMultiplier);

            ParticleSystem.NoiseModule noise = system.noise;
            noise.enabled = settings.Turbulence > 0f;
            noise.strength = settings.Turbulence;
            noise.frequency = NoiseFrequency;
            noise.scrollSpeed = NoiseScrollSpeed;
            noise.quality = ParticleSystemNoiseQuality.Medium;

            var renderer = system.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Stretch;
            renderer.lengthScale = StretchLengthScale;
            renderer.velocityScale = StretchVelocityScale;
        }
    }
}
