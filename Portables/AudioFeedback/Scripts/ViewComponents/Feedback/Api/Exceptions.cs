using Core.Gameplay.Feedback;
using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Feedback
{
    internal sealed class DuplicateAudioFeedbackEntryException : ExtendedException
    {
        internal DuplicateAudioFeedbackEntryException(AudioFeedbackType type, string objectName)
            : base("feedback-1", $"Feedback entry for '{type}' is duplicated on '{objectName}'") { }
    }

    internal sealed class MissingAudioFeedbackEntryException : ExtendedException
    {
        internal MissingAudioFeedbackEntryException(AudioFeedbackType type, string objectName)
            : base("feedback-2", $"Feedback entry for '{type}' is missing on '{objectName}'") { }
    }

    internal sealed class MissingAudioFeedbackSoundException : ExtendedException
    {
        internal MissingAudioFeedbackSoundException(AudioFeedbackType type, string objectName)
            : base("feedback-3", $"Feedback entry for '{type}' has no sound on '{objectName}'") { }
    }

    internal sealed class MissingAudioFeedbackFieldException : ExtendedException
    {
        internal MissingAudioFeedbackFieldException(string fieldName, string objectName)
            : base("feedback-4", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
