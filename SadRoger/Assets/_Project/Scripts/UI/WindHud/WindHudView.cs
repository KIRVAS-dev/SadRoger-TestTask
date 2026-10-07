using System;
using ContentValidation;
using Core.Lifecycle;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindHud
{
    public sealed class WindHudView
        : MonoBehaviour,
          IWindHudView,
          IValidatable,
          IWarmupLifecycle
    {
        private const string DegreesFormat = "{0}°";
        private const string PercentFormat = "{0}%";
        private const float PercentFactor = 100f;
        private const float HalfTurn = 180f;
        private const float FullTurn = 360f;

        [SerializeField] private TextMeshProUGUI _directionText;
        [SerializeField] private TextMeshProUGUI _strengthText;
        [SerializeField] private TextMeshProUGUI _relativeAngleText;
        [Tooltip("Arrow pointing up at zero rotation; up on screen is the ship's bow")]
        [SerializeField] private RectTransform _windArrow;
        [SerializeField] private Slider _directionSlider;
        [SerializeField] private Slider _baseStrengthSlider;

        public event Action<float> DirectionChanged;
        public event Action<float> BaseStrengthChanged;

        private void OnDestroy()
        {
            _directionSlider.onValueChanged.RemoveListener(OnDirectionSliderChanged);
            _baseStrengthSlider.onValueChanged.RemoveListener(OnBaseStrengthSliderChanged);
        }

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_directionText, () => Missing(nameof(_directionText)));
            Guard.AgainstNull(_strengthText, () => Missing(nameof(_strengthText)));
            Guard.AgainstNull(_relativeAngleText, () => Missing(nameof(_relativeAngleText)));
            Guard.AgainstNull(_windArrow, () => Missing(nameof(_windArrow)));
            Guard.AgainstNull(_directionSlider, () => Missing(nameof(_directionSlider)));
            Guard.AgainstNull(_baseStrengthSlider, () => Missing(nameof(_baseStrengthSlider)));

            return;

            ExtendedException Missing(string fieldName) => new MissingWindHudFieldException(fieldName, gameObject.name);
        }

        void IWarmupLifecycle.Warmup()
        {
            _directionSlider.onValueChanged.AddListener(OnDirectionSliderChanged);
            _baseStrengthSlider.onValueChanged.AddListener(OnBaseStrengthSliderChanged);
        }

        void IWindHudView.SetDirection(float direction)
        {
            _directionText.SetText(DegreesFormat, Mathf.Round(direction) % FullTurn);
            _directionSlider.SetValueWithoutNotify(direction);
        }

        void IWindHudView.SetBaseStrength(float baseStrength)
        {
            _baseStrengthSlider.SetValueWithoutNotify(baseStrength);
        }

        void IWindHudView.SetStrength(float strength)
        {
            _strengthText.SetText(PercentFormat, Mathf.Round(strength * PercentFactor));
        }

        void IWindHudView.SetRelativeAngle(float relativeAngle)
        {
            _relativeAngleText.SetText(DegreesFormat, Mathf.Round(relativeAngle));
            _windArrow.localRotation = Quaternion.Euler(0f, 0f, -(relativeAngle + HalfTurn));
        }

        private void OnDirectionSliderChanged(float direction)
        {
            DirectionChanged?.Invoke(direction);
        }

        private void OnBaseStrengthSliderChanged(float baseStrength)
        {
            BaseStrengthChanged?.Invoke(baseStrength);
        }
    }
}
