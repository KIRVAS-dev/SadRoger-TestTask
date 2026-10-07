using ContentValidation;
using DG.Tweening;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace UI.LoadingScreen
{
    [CreateAssetMenu(menuName = "Configs/Loading Screen Config")]
    internal sealed class LoadingScreenConfig
        : ScriptableObject,
          IValidatable
    {
        [Header("Fade Out")]
        [SerializeField] private float _fadeOutDuration = 0.35f;
        [SerializeField] private Ease _fadeOutEase = Ease.OutQuad;

        public float FadeOutDuration => _fadeOutDuration;
        public Ease FadeOutEase => _fadeOutEase;

        public void Validate()
        {
            Guard.AgainstNonPositive(
                _fadeOutDuration,
                () => new InvalidLoadingScreenConfigValueException(nameof(_fadeOutDuration), _fadeOutDuration)
            );
        }
    }
}
