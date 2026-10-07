using ContentValidation;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace UI.LocationExitNotice
{
    public sealed class LocationExitNoticeView
        : MonoBehaviour,
          ILocationExitNoticeView,
          IValidatable
    {
        [SerializeField] private GameObject _root;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_root, () => new MissingLocationExitNoticeFieldException(nameof(_root), gameObject.name));
        }

        void ILocationExitNoticeView.Show()
        {
            _root.SetActive(true);
        }

        void ILocationExitNoticeView.Hide()
        {
            _root.SetActive(false);
        }
    }
}
