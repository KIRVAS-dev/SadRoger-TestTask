using R3;

namespace Core.Gameplay.Wind
{
    public interface IReadOnlyWindModel
    {
        ReadOnlyReactiveProperty<float> Direction { get; }
        ReadOnlyReactiveProperty<float> BaseStrength { get; }
        ReadOnlyReactiveProperty<float> Strength { get; }
    }
}
