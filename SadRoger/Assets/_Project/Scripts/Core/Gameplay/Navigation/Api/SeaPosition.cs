using System;

namespace Core.Gameplay.Navigation
{
    public readonly struct SeaPosition : IEquatable<SeaPosition>
    {
        public SeaPosition(float x, float z)
        {
            X = x;
            Z = z;
        }

        public float X { get; }
        public float Z { get; }

        public float SquaredDistanceTo(float x, float z)
        {
            float deltaX = X - x;
            float deltaZ = Z - z;

            return deltaX * deltaX + deltaZ * deltaZ;
        }

        public bool Equals(SeaPosition other)
        {
            return X.Equals(other.X) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is SeaPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Z);
        }
    }
}
