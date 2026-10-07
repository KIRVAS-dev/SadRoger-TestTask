using R3;

namespace Core.Gameplay.Wind
{
    public sealed class WindModel : IReadOnlyWindModel
    {
        public ReactiveProperty<float> Direction { get; } = new ReactiveProperty<float>();
        public ReactiveProperty<float> BaseStrength { get; } = new ReactiveProperty<float>();
        public ReactiveProperty<float> Strength { get; } = new ReactiveProperty<float>();

        ReadOnlyReactiveProperty<float> IReadOnlyWindModel.Direction => Direction;
        ReadOnlyReactiveProperty<float> IReadOnlyWindModel.BaseStrength => BaseStrength;
        ReadOnlyReactiveProperty<float> IReadOnlyWindModel.Strength => Strength;
    }
}
