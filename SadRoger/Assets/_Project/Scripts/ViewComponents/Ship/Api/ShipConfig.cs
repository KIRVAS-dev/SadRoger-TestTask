using ContentValidation;
using Core.Gameplay.Ship;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Ship
{
    [CreateAssetMenu(menuName = "Configs/Ship Config")]
    public sealed class ShipConfig
        : ScriptableObject,
          IShipSettings,
          IValidatable
    {
        [Tooltip("Units per second at full throttle")]
        [SerializeField] private float _moveSpeed = 8f;

        [Tooltip("Degrees per second at full steering")]
        [SerializeField] private float _turnSpeed = 90f;

        float IShipSettings.MoveSpeed => _moveSpeed;
        float IShipSettings.TurnSpeed => _turnSpeed;

        public void Validate()
        {
            Guard.AgainstNonPositive(_moveSpeed, () => Invalid(nameof(_moveSpeed), _moveSpeed));
            Guard.AgainstNonPositive(_turnSpeed, () => Invalid(nameof(_turnSpeed), _turnSpeed));

            return;

            ExtendedException Invalid(string fieldName, float value) => new InvalidShipValueException(fieldName, value);
        }
    }
}
