using UnityEngine;

namespace WaterfallEffect.Layers
{
    /// <summary>
    /// Slow, large, translucent puffs rising from the landing point.
    /// </summary>
    public sealed class MistLayer : IWaterfallLayer
    {
        private const float RatePerMetre = 6f;
        private const float MinLifetime = 2.5f;
        private const float MaxLifetime = 4.5f;
        private const float MinSpeed = 0.3f;
        private const float MaxSpeed = 1.2f;
        private const float MinSize = 1.5f;
        private const float SizePerMetreMin = 0.6f;
        private const float SizePerMetreMax = 1.2f;
        private const float GravityModifier = -0.02f;
        private const float Opacity = 0.35f;
        private const float EmitterWidthFactor = 1.2f;
        private const float EmitterDepth = 1f;
        private const float EmitterThickness = 0.5f;
        private const float RandomDirection = 0.3f;
        private const float MaxSpinDegreesPerSecond = 20f;
        private const float FullTurnRadians = Mathf.PI * 2f;

        // Rotating the box shape by -90° around X turns its emission direction (+Z) upwards.
        private static readonly Vector3 UpwardShapeRotation = new Vector3(-90f, 0f, 0f);

        public void Configure(ParticleSystem system, WaterfallSettings settings, WaterfallGeometry geometry)
        {
            system.transform.localPosition = geometry.LandingPoint;
            system.transform.localRotation = Quaternion.identity;

            float rate = RatePerMetre * settings.Width * settings.FlowIntensity;
            Color tint = settings.WaterColor;
            tint.a = Opacity;

            ParticleSystem.MainModule main = system.main;
            main.loop = true;
            main.prewarm = true;
            main.playOnAwake = true;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.startLifetime = ParticleCurves.Range(MinLifetime, MaxLifetime);
            main.startSpeed = ParticleCurves.Range(MinSpeed, MaxSpeed);
            main.startSize = ParticleCurves.Range(
                Mathf.Max(MinSize, settings.Width * SizePerMetreMin),
                Mathf.Max(MinSize, settings.Width * SizePerMetreMax));
            main.startRotation = ParticleCurves.Range(0f, FullTurnRadians);
            main.startColor = tint;
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
            shape.scale = new Vector3(settings.Width * EmitterWidthFactor, EmitterDepth, EmitterThickness);
            shape.randomDirectionAmount = RandomDirection;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = system.colorOverLifetime;
            colorOverLifetime.enabled = true;
            colorOverLifetime.color = ParticleCurves.AlphaFade(
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.2f),
                new GradientAlphaKey(0f, 1f));

            ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = system.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = ParticleCurves.Linear(0.6f, 1.4f);

            ParticleSystem.RotationOverLifetimeModule rotationOverLifetime = system.rotationOverLifetime;
            rotationOverLifetime.enabled = true;
            rotationOverLifetime.z = ParticleCurves.Range(
                -MaxSpinDegreesPerSecond * Mathf.Deg2Rad,
                MaxSpinDegreesPerSecond * Mathf.Deg2Rad);

            ParticleSystem.NoiseModule noise = system.noise;
            noise.enabled = false;

            var renderer = system.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }
    }
}
