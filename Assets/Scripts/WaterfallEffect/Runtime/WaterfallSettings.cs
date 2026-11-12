using System;
using UnityEngine;

namespace WaterfallEffect
{
    /// <summary>
    /// Designer-facing parameters of a waterfall. All particle values are derived from these.
    /// </summary>
    [Serializable]
    public sealed class WaterfallSettings
    {
        [SerializeField, Min(0.5f), Tooltip("Width of the water curtain, metres.")]
        private float _width = 4f;

        [SerializeField, Min(1f), Tooltip("Fall height from the emitter to the landing point, metres.")]
        private float _height = 20f;

        [SerializeField, Range(0.1f, 3f), Tooltip("Multiplier for the amount of water.")]
        private float _flowIntensity = 1f;

        [SerializeField, Range(0f, 10f), Tooltip("Horizontal speed at which water leaves the edge, m/s.")]
        private float _forwardSpeed = 1.5f;

        [SerializeField, Range(0f, 2f), Tooltip("Strength of the noise that makes the stream wobble.")]
        private float _turbulence = 0.4f;

        [SerializeField, Tooltip("Tint of the falling water. Alpha controls its opacity.")]
        private Color _waterColor = new Color(0.78f, 0.9f, 1f, 0.85f);

        [SerializeField, Tooltip("Mist cloud at the bottom of the waterfall.")]
        private bool _mistEnabled = true;

        [SerializeField, Tooltip("Droplets bouncing off the landing point.")]
        private bool _splashEnabled = true;

        public float Width => _width;

        public float Height => _height;

        public float FlowIntensity => _flowIntensity;

        public float ForwardSpeed => _forwardSpeed;

        public float Turbulence => _turbulence;

        public Color WaterColor => _waterColor;

        public bool MistEnabled => _mistEnabled;

        public bool SplashEnabled => _splashEnabled;
    }
}
