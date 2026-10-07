using System;
using System.Collections.Generic;
using ContentValidation;
using Core.Gameplay.Feedback;
using Core.Lifecycle;
using FMODUnity;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Feedback
{
    public sealed class AudioFeedbackPerformer
        : MonoBehaviour,
          IAudioFeedbackPerformer,
          IValidatable,
          IWarmupLifecycle
    {
        [SerializeField] private List<AudioFeedbackEntry> _entries;

        private readonly Dictionary<AudioFeedbackType, EventReference> _soundsByType =
            new Dictionary<AudioFeedbackType, EventReference>();

        void IValidatable.Validate()
        {
            Guard.AgainstNullOrEmpty(_entries, () => new MissingAudioFeedbackFieldException(nameof(_entries), gameObject.name));

            HashSet<AudioFeedbackType> seenTypes = new HashSet<AudioFeedbackType>();

            foreach (AudioFeedbackEntry entry in _entries)
            {
                Guard.AgainstTrue(
                    !seenTypes.Add(entry.Type),
                    () => new DuplicateAudioFeedbackEntryException(entry.Type, gameObject.name)
                );

                Guard.AgainstTrue(entry.Sound.IsNull, () => new MissingAudioFeedbackSoundException(entry.Type, gameObject.name));
            }

            foreach (AudioFeedbackType type in Enum.GetValues(typeof(AudioFeedbackType)))
            {
                Guard.AgainstTrue(!seenTypes.Contains(type), () => new MissingAudioFeedbackEntryException(type, gameObject.name));
            }
        }

        void IWarmupLifecycle.Warmup()
        {
            foreach (AudioFeedbackEntry entry in _entries)
            {
                _soundsByType.Add(entry.Type, entry.Sound);
            }
        }

        void IAudioFeedbackPerformer.Play(AudioFeedbackType type)
        {
            RuntimeManager.PlayOneShot(_soundsByType[type]);
        }
    }
}
