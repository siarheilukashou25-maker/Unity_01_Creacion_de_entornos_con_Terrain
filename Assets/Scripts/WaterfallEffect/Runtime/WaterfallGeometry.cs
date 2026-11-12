using System;
using UnityEngine;

namespace WaterfallEffect
{
    /// <summary>
    /// Ballistic description of the falling stream, shared by all layers so they stay in sync:
    /// the stream dies exactly where the splash and mist are placed.
    /// </summary>
    public readonly struct WaterfallGeometry
    {
        /// <summary>Gravity modifier of the stream particles; the fall time is computed for it.</summary>
        public const float StreamGravityModifier = 1f;

        private const float MinGravity = 0.01f;

        public WaterfallGeometry(WaterfallSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            float gravity = Mathf.Max(Mathf.Abs(Physics.gravity.y) * StreamGravityModifier, MinGravity);

            // h = g * t^2 / 2  =>  t = sqrt(2h / g)
            FallTime = Mathf.Sqrt(2f * settings.Height / gravity);
            ImpactSpeed = gravity * FallTime;
            LandingPoint = new Vector3(0f, -settings.Height, settings.ForwardSpeed * FallTime);
        }

        /// <summary>Time for a particle to fall the full height, seconds.</summary>
        public float FallTime { get; }

        /// <summary>Vertical speed at the landing point, m/s.</summary>
        public float ImpactSpeed { get; }

        /// <summary>Landing point in waterfall local space.</summary>
        public Vector3 LandingPoint { get; }
    }
}
