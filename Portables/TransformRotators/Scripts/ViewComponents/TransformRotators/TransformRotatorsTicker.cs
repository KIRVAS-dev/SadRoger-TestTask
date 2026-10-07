using System.Collections.Generic;
using Core.Gameplay.TransformRotator;
using Core.Loop;

namespace ViewComponents.TransformRotators
{
    public sealed class TransformRotatorsTicker : IPresentationTickable
    {
        private readonly ITransformRotatorsRegistry _registry;

        public TransformRotatorsTicker(ITransformRotatorsRegistry registry)
        {
            _registry = registry;
        }

        void IPresentationTickable.Tick()
        {
            IReadOnlyCollection<ITransformRotatorView> rotators = _registry.Rotators;

            foreach (ITransformRotatorView rotator in rotators)
            {
                rotator.Rotate();
            }
        }
    }
}
