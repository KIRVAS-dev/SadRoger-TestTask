using System.Collections.Generic;

namespace Core.Gameplay.TransformRotator
{
    public interface ITransformRotatorsRegistry
    {
        IReadOnlyCollection<ITransformRotatorView> Rotators { get; }
    }
}
