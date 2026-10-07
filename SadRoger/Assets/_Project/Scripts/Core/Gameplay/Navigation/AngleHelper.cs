using System;

namespace Core.Gameplay.Navigation
{
    public static class AngleHelper
    {
        private const float FullTurn = 360f;
        private const float HalfTurn = 180f;
        private const float DegreesToRadians = MathF.PI / HalfTurn;

        public static float NormalizeDegrees(float degrees)
        {
            float normalized = degrees % FullTurn;

            return normalized < 0f
                ? normalized + FullTurn
                : normalized;
        }

        public static float SignedDeltaDegrees(float fromDegrees, float toDegrees)
        {
            float delta = NormalizeDegrees(toDegrees - fromDegrees);

            return delta > HalfTurn
                ? delta - FullTurn
                : delta;
        }

        public static float ForwardX(float headingDegrees)
        {
            return MathF.Sin(headingDegrees * DegreesToRadians);
        }

        public static float ForwardZ(float headingDegrees)
        {
            return MathF.Cos(headingDegrees * DegreesToRadians);
        }
    }
}
