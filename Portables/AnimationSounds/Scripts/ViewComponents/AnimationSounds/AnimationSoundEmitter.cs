using ContentValidation;
using FMODUnity;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.AnimationSounds
{
    [DisallowMultipleComponent]
    public sealed class AnimationSoundEmitter
        : MonoBehaviour,
          IValidatable
    {
        private const float MinClipWeight = 0.5f;

        [SerializeField] private Animator _animator;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(
                _animator,
                () => new MissingAnimationSoundEmitterFieldException(nameof(_animator), gameObject.name)
            );

            Guard.AgainstNull(
                _animator.runtimeAnimatorController,
                () => new MissingAnimationSoundControllerException(_animator.gameObject.name)
            );

            ValidateClipEvents();
        }

        private void ValidateClipEvents()
        {
            foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
            {
                foreach (AnimationEvent clipEvent in clip.events)
                {
                    if (clipEvent.functionName != nameof(PlaySound))
                    {
                        continue;
                    }

                    AnimationSoundConfig soundConfig = clipEvent.objectReferenceParameter as AnimationSoundConfig;

                    Guard.AgainstNull(soundConfig, () => new MissingAnimationSoundConfigException(clip.name, gameObject.name));

                    soundConfig.Validate();
                }
            }
        }

        private void PlaySound(AnimationEvent animationEvent)
        {
            bool isDominantClip = animationEvent.animatorClipInfo.weight >= MinClipWeight;

            if (!isDominantClip)
            {
                return;
            }

            AnimationSoundConfig soundConfig = (AnimationSoundConfig)animationEvent.objectReferenceParameter;

            RuntimeManager.PlayOneShot(soundConfig.Sound, transform.position);
        }
    }
}
