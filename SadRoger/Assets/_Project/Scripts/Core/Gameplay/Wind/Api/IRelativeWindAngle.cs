using R3;

namespace Core.Gameplay.Wind
{
    public interface IRelativeWindAngle
    {
        ReadOnlyReactiveProperty<float> Angle { get; }
    }
}
