using UnityEngine;
using VContainer;

namespace ViewComponents.Audio
{
    [DisallowMultipleComponent]
    public sealed class AudioListenerCameraFollower : MonoBehaviour
    {
        private IStudioListenerAnchor _anchor;

        [Inject]
        private void Construct(IStudioListenerAnchor anchor)
        {
            _anchor = anchor;
        }

        private void LateUpdate()
        {
            Transform listener = _anchor.Transform;
            listener.SetPositionAndRotation(transform.position, transform.rotation);
        }
    }
}
