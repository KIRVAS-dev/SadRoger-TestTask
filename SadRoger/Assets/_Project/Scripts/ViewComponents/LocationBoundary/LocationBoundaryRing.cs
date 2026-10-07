using ContentValidation;
using Core.Gameplay.LocationBoundary;
using Core.Lifecycle;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using VContainer;

namespace ViewComponents.LocationBoundary
{
    [DisallowMultipleComponent]
    public sealed class LocationBoundaryRing
        : MonoBehaviour,
          IValidatable,
          IWarmupLifecycle
    {
        private const int MinSegmentCount = 3;
        private const float FullTurnRadians = 2f * Mathf.PI;

        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private int _segmentCount = 128;

        private ILocationBoundarySettings _settings;

        [Inject]
        private void Construct(ILocationBoundarySettings settings)
        {
            _settings = settings;
        }

        void IValidatable.Validate()
        {
            Guard.AgainstNull(
                _lineRenderer,
                () => new MissingLocationBoundaryFieldException(nameof(_lineRenderer), gameObject.name)
            );

            Guard.AgainstLessThan(
                _segmentCount,
                MinSegmentCount,
                () => new InvalidLocationBoundaryRingValueException(nameof(_segmentCount), gameObject.name, _segmentCount)
            );
        }

        void IWarmupLifecycle.Warmup()
        {
            DrawRing();
        }

        private void DrawRing()
        {
            float height = transform.position.y;

            _lineRenderer.useWorldSpace = true;
            _lineRenderer.loop = true;
            _lineRenderer.positionCount = _segmentCount;

            for (int i = 0; i < _segmentCount; i++)
            {
                float angle = FullTurnRadians * i / _segmentCount;

                Vector3 point = new Vector3(
                    _settings.CenterX + Mathf.Sin(angle) * _settings.Radius,
                    height,
                    _settings.CenterZ + Mathf.Cos(angle) * _settings.Radius
                );

                _lineRenderer.SetPosition(i, point);
            }
        }
    }
}
