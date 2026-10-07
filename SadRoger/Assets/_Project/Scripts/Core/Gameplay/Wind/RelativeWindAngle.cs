using System;
using Core.Gameplay.Navigation;
using Core.Gameplay.Ship;
using R3;

namespace Core.Gameplay.Wind
{
    public sealed class RelativeWindAngle
        : IRelativeWindAngle,
          IDisposable
    {
        private readonly ReadOnlyReactiveProperty<float> _angle;

        public RelativeWindAngle(IReadOnlyWindModel windModel, IReadOnlyShipModel shipModel)
        {
            _angle = windModel.Direction.CombineLatest(shipModel.Heading, RelativeAngle).ToReadOnlyReactiveProperty();
        }

        ReadOnlyReactiveProperty<float> IRelativeWindAngle.Angle => _angle;

        void IDisposable.Dispose()
        {
            _angle.Dispose();
        }

        private static float RelativeAngle(float windDirection, float shipHeading)
        {
            return AngleHelper.SignedDeltaDegrees(shipHeading, windDirection);
        }
    }
}
