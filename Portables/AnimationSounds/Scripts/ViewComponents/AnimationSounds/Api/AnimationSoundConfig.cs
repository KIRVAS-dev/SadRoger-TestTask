using ContentValidation;
using FMODUnity;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.AnimationSounds
{
    [CreateAssetMenu(menuName = "Configs/Animation Sound Config")]
    internal sealed class AnimationSoundConfig
        : ScriptableObject,
          IValidatable
    {
        [SerializeField] private EventReference _sound;

        internal EventReference Sound => _sound;

        public void Validate()
        {
            Guard.AgainstTrue(_sound.IsNull, () => new MissingAnimationSoundException(name));
        }
    }
}
