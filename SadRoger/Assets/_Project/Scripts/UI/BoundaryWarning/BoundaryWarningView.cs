using ContentValidation;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;

namespace UI.BoundaryWarning
{
    public sealed class BoundaryWarningView
        : MonoBehaviour,
          IBoundaryWarningView,
          IValidatable
    {
        private const string SecondsFormat = "{0}";

        [SerializeField] private GameObject _root;
        [SerializeField] private TextMeshProUGUI _countdownText;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_root, () => Missing(nameof(_root)));
            Guard.AgainstNull(_countdownText, () => Missing(nameof(_countdownText)));

            return;

            ExtendedException Missing(string fieldName) => new MissingBoundaryWarningFieldException(fieldName, gameObject.name);
        }

        void IBoundaryWarningView.Show()
        {
            _root.SetActive(true);
        }

        void IBoundaryWarningView.Hide()
        {
            _root.SetActive(false);
        }

        void IBoundaryWarningView.SetSecondsLeft(int secondsLeft)
        {
            _countdownText.SetText(SecondsFormat, secondsLeft);
        }
    }
}
