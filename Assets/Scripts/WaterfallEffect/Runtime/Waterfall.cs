using System;
using WaterfallEffect.Layers;
using UnityEngine;

namespace WaterfallEffect
{
    /// <summary>
    /// Root component of a waterfall. Owns the settings and pushes them into the child particle systems
    /// through layer configurators, so tweaking a value in the Inspector rebuilds the whole effect.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Waterfall : MonoBehaviour
    {
        [SerializeField] private WaterfallSettings _settings = new WaterfallSettings();
        [SerializeField] private ParticleSystem _stream;
        [SerializeField] private ParticleSystem _mist;
        [SerializeField] private ParticleSystem _splash;

        private readonly IWaterfallLayer _streamLayer = new StreamLayer();
        private readonly IWaterfallLayer _mistLayer = new MistLayer();
        private readonly IWaterfallLayer _splashLayer = new SplashLayer();

        public WaterfallSettings Settings => _settings;

        public void Initialize(ParticleSystem stream, ParticleSystem mist, ParticleSystem splash)
        {
            _stream = stream != null ? stream : throw new ArgumentNullException(nameof(stream));
            _mist = mist != null ? mist : throw new ArgumentNullException(nameof(mist));
            _splash = splash != null ? splash : throw new ArgumentNullException(nameof(splash));

            Apply();
        }

        /// <summary>Recalculates every layer from the current settings.</summary>
        public void Apply()
        {
            var geometry = new WaterfallGeometry(_settings);

            ApplyLayer(_stream, _streamLayer, geometry, true);
            ApplyLayer(_mist, _mistLayer, geometry, _settings.MistEnabled);
            ApplyLayer(_splash, _splashLayer, geometry, _settings.SplashEnabled);
        }

        private void ApplyLayer(ParticleSystem system, IWaterfallLayer layer, WaterfallGeometry geometry, bool isEnabled)
        {
            if (system == null)
            {
                return;
            }

            system.gameObject.SetActive(isEnabled);

            if (isEnabled)
            {
                layer.Configure(system, _settings, geometry);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Changing particle modules and activating objects is not allowed inside OnValidate,
            // so the rebuild is deferred to the next editor tick.
            UnityEditor.EditorApplication.delayCall -= ApplyIfAlive;
            UnityEditor.EditorApplication.delayCall += ApplyIfAlive;
        }

        private void ApplyIfAlive()
        {
            if (this != null)
            {
                Apply();
            }
        }
#endif
    }
}
