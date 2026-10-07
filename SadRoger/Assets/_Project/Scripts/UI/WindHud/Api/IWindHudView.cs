using System;

namespace UI.WindHud
{
    public interface IWindHudView
    {
        event Action<float> DirectionChanged;
        event Action<float> BaseStrengthChanged;

        void SetDirection(float direction);
        void SetBaseStrength(float baseStrength);
        void SetStrength(float strength);
        void SetRelativeAngle(float relativeAngle);
    }
}
