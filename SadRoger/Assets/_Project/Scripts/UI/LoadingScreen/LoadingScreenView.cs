using ContentValidation;
using DG.Tweening;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.LoadingScreen
{
    public sealed class LoadingScreenView
        : MonoBehaviour,
          ILoadingScreenView,
          IValidatable
    {
        private const float VisibleAlpha = 1f;
        private const float HiddenAlpha = 0f;
        private const float PercentFactor = 100f;
        private const string PercentFormat = "{0}%";

        [SerializeField] private LoadingScreenConfig _config;
        [SerializeField] private CanvasGroup _root;
        [SerializeField] private Image _progressFill;
        [SerializeField] private TextMeshProUGUI _progressText;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_config, () => Missing(nameof(_config)));
            Guard.AgainstNull(_root, () => Missing(nameof(_root)));
            Guard.AgainstNull(_progressFill, () => Missing(nameof(_progressFill)));
            Guard.AgainstNull(_progressText, () => Missing(nameof(_progressText)));

            Guard.AgainstTrue(
                _progressFill.type != Image.Type.Filled,
                () => new InvalidLoadingScreenProgressFillException(nameof(_progressFill), gameObject.name)
            );

            _config.Validate();

            return;

            ExtendedException Missing(string fieldName) => new MissingLoadingScreenFieldException(fieldName, gameObject.name);
        }

        void ILoadingScreenView.Show()
        {
            _root.DOKill();
            _root.alpha = VisibleAlpha;
            _root.gameObject.SetActive(true);
        }

        void ILoadingScreenView.Hide()
        {
            _root.DOKill();

            _root
               .DOFade(HiddenAlpha, _config.FadeOutDuration)
               .SetEase(_config.FadeOutEase)
               .SetLink(gameObject)
               .OnComplete(OnFadedOut);
        }

        void ILoadingScreenView.SetProgress(float progress)
        {
            _progressFill.fillAmount = progress;
            _progressText.SetText(PercentFormat, Mathf.Round(progress * PercentFactor));
        }

        private void OnFadedOut()
        {
            _root.gameObject.SetActive(false);
        }
    }
}
