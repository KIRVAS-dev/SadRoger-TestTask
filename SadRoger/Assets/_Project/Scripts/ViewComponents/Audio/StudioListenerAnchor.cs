using UnityEngine;

namespace ViewComponents.Audio
{
    public sealed class StudioListenerAnchor : IStudioListenerAnchor
    {
        private readonly Transform _transform;

        public StudioListenerAnchor(Transform transform)
        {
            _transform = transform;
        }

        Transform IStudioListenerAnchor.Transform => _transform;
    }
}
