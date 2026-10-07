using ContentValidation;
using Core.Gameplay.TransformRotator;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.TransformRotators
{
    internal sealed class TransformRotatorWhileRendererVisible
        : MonoBehaviour,
          ITransformRotatorView,
          IValidatable
    {
        [SerializeField] private Renderer _visibilityRenderer;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Vector3 _speed;
        [SerializeField] private RotationDirection _direction = RotationDirection.Clockwise;

        private float DirectionSign => _direction == RotationDirection.Clockwise
            ? -1f
            : 1f;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_targetTransform, () => Missing(nameof(_targetTransform)));
            Guard.AgainstNull(_visibilityRenderer, () => Missing(nameof(_visibilityRenderer)));

            return;

            ExtendedException Missing(string fieldName) => new MissingTransformRotatorFieldException(fieldName, gameObject.name);
        }

        void ITransformRotatorView.Rotate()
        {
            bool canRotate = isActiveAndEnabled && _visibilityRenderer.isVisible;

            if (!canRotate)
            {
                return;
            }

            _targetTransform.Rotate(_speed * DirectionSign * Time.deltaTime);
        }
    }
}
