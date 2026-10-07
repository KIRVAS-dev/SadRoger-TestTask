using ContentValidation;
using Core.Gameplay.LocationBoundary;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.LocationBoundary
{
    [CreateAssetMenu(menuName = "Configs/Location Boundary Config")]
    public sealed class LocationBoundaryConfig
        : ScriptableObject,
          ILocationBoundarySettings,
          IValidatable
    {
        [SerializeField] private float _radius = 50f;

        [Tooltip("Seconds outside the boundary before the warning appears")]
        [SerializeField] private float _warningDelay = 1f;

        [Tooltip("Countdown seconds after the warning before the location is exited")]
        [SerializeField] private float _exitCountdown = 5f;

        float ILocationBoundarySettings.Radius => _radius;
        float ILocationBoundarySettings.WarningDelay => _warningDelay;
        float ILocationBoundarySettings.ExitCountdown => _exitCountdown;

        public void Validate()
        {
            Guard.AgainstNonPositive(_radius, () => Invalid(nameof(_radius), _radius));
            Guard.AgainstNegative(_warningDelay, () => Invalid(nameof(_warningDelay), _warningDelay));
            Guard.AgainstNonPositive(_exitCountdown, () => Invalid(nameof(_exitCountdown), _exitCountdown));

            return;

            ExtendedException Invalid(string fieldName, float value) =>
                new InvalidLocationBoundaryValueException(fieldName, value);
        }
    }
}
