using UnityEngine;

namespace WaterfallEffect.Layers
{
    /// <summary>
    /// Droplets bouncing up from the landing point. Their speed scales with the impact speed,
    /// so higher waterfalls splash harder.
    /// </summary>
    public sealed class SplashLayer : IWaterfallLayer
    {
        private const float RatePerMetre = 40f;
        private const float MinLifetime = 0.4f;
        private const float MaxLifetime = 0.9f;
        private const float MinSpeedFactor = 0.1f;
        private const float MaxSpeedFactor = 0.25f;
        private const float MinSize = 0.2f;
        private const float MaxSize = 0.6f;
        private const float GravityModifier = 1f;
        private const float EmitterDepth = 0.5f;
        private const float EmitterThickness = 0.1f;
        private const float RandomDirection = 0.4f;
        private const float EndSizeMultiplier = 0.5f;

        // Rotating the box shape by -90° around X turns its emission direction (+Z) upwards.
        private static readonly Vector3 UpwardShapeRotation = new Vector3(-90f, 0f, 0f);

        public void Configure(ParticleSystem system, WaterfallSettings settings, WaterfallGeometry geometry)
        {
            system.transform.localPosition = geometry.LandingPoint;
            system.transform.localRotation = Quaternion.identity;

            float rate = RatePerMetre * settings.Width * settings.FlowIntensity;

            ParticleSystem.MainModule main = system.main;
            main.loop = true;
            main.prewarm = true;
            main.playOnAwake = true;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.startLifetime = ParticleCurves.Range(MinLifetime, MaxLifetime);
            main.startSpeed = ParticleCurves.Range(
                geometry.ImpactSpeed * MinSpeedFactor,
                geometry.ImpactSpeed * MaxSpeedFactor);
            main.startSize = ParticleCurves.Range(MinSize, MaxSize);
            main.startColor = settings.WaterColor;
            main.gravityModifier = GravityModifier;
            main.maxParticles = ParticleCurves.MaxParticles(rate, MaxLifetime);

            ParticleSystem.EmissionModule emission = system.emission;
            emission.enabled = true;
            emission.rateOverTime = rate;

            ParticleSystem.ShapeModule shape = system.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.position = Vector3.zero;
            shape.rotation = UpwardShapeRotation;
            shape.scale = new Vector3(settings.Width, EmitterDepth, EmitterThickness);
            shape.randomDirectionAmount = RandomDirection;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = system.colorOverLifetime;
            colorOverLifetime.enabled = true;
            colorOverLifetime.color = ParticleCurves.AlphaFade(
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f));

            ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = system.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = ParticleCurves.Linear(1f, EndSizeMultiplier);

            ParticleSystem.NoiseModule noise = system.noise;
            noise.enabled = false;

            var renderer = system.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }
    }
}
