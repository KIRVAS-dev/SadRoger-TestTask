using System;

namespace Core.Input
{
    public interface IWindInput
    {
        event Action DirectionChanged;
        event Action StrengthChanged;

        float Direction { get; }
        float Strength { get; }
    }
}
