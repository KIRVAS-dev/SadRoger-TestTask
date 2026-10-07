using Core.Gameplay.Weather;
using Core.Gameplay.Wind;
using Core.Lifecycle;
using NUnit.Framework;

namespace Tests.EditMode
{
    internal sealed class WeatherServiceTests
    {
        private FakeSettings _settings;
        private FakeWindStrengthMultiplier _windStrengthMultiplier;
        private WeatherModel _model;
        private WeatherService _service;

        [SetUp]
        public void SetUp()
        {
            _settings = new FakeSettings();
            _windStrengthMultiplier = new FakeWindStrengthMultiplier();
            _model = new WeatherModel();
            _service = new WeatherService(_settings, _windStrengthMultiplier, _model);
        }

        [Test]
        public void Prepare_AppliesInitialStatePreset()
        {
            ((IPreparationLifecycle)_service).Prepare();

            Assert.That(_model.ActivePreset.Value, Is.SameAs(_settings.Clear));
            Assert.That(_windStrengthMultiplier.Multiplier, Is.EqualTo(_settings.Clear.WindMultiplier));
        }

        [TestCase(WeatherState.Clear)]
        [TestCase(WeatherState.Rain)]
        [TestCase(WeatherState.Storm)]
        [TestCase(WeatherState.Fog)]
        public void SetState_AppliesMatchingPreset(WeatherState state)
        {
            ((IWeatherService)_service).SetState(state);

            IWeatherPreset expected = ((IWeatherSettings)_settings).GetPreset(state);

            Assert.That(_model.State.Value, Is.EqualTo(state));
            Assert.That(_model.ActivePreset.Value, Is.SameAs(expected));
            Assert.That(_windStrengthMultiplier.Multiplier, Is.EqualTo(expected.WindMultiplier));
        }

        private sealed class FakePreset : IWeatherPreset
        {
            public FakePreset(float windMultiplier)
            {
                WindMultiplier = windMultiplier;
            }

            public float Intensity => 0f;
            public float FogDensity => 0f;
            public float WindMultiplier { get; }
        }

        private sealed class FakeSettings : IWeatherSettings
        {
            public FakePreset Clear { get; } = new FakePreset(1f);
            public FakePreset Rain { get; } = new FakePreset(1.2f);
            public FakePreset Storm { get; } = new FakePreset(2f);
            public FakePreset Fog { get; } = new FakePreset(0.3f);

            IWeatherPreset IWeatherSettings.GetPreset(WeatherState state)
            {
                return state switch
                {
                    WeatherState.Clear => Clear,
                    WeatherState.Rain => Rain,
                    WeatherState.Storm => Storm,
                    WeatherState.Fog => Fog,
                    _ => null
                };
            }
        }

        private sealed class FakeWindStrengthMultiplier : IWindStrengthMultiplier
        {
            public float Multiplier { get; private set; }

            void IWindStrengthMultiplier.SetMultiplier(float multiplier)
            {
                Multiplier = multiplier;
            }
        }
    }
}
