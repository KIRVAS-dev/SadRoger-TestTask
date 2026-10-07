using System.Collections.Generic;
using UnityEngine;

namespace BuildPresetTool
{
    public class PresetApplicationReport
    {
        readonly List<string> appliedSettings = new List<string>();
        readonly List<string> failedSettings = new List<string>();
        readonly List<string> notFoundSettings = new List<string>();
        bool hasErrors => failedSettings.Count > 0 || notFoundSettings.Count > 0;

        public void AddApplied(string setting)
        {
            appliedSettings.Add(setting);
        }

        public void AddFailed(string setting, string reason = "")
        {
            failedSettings.Add(
                string.IsNullOrEmpty(reason)
                    ? setting
                    : $"{setting} ({reason})"
            );
        }

        public void AddNotFound(string setting)
        {
            notFoundSettings.Add(setting);
        }

        public void LogSummary()
        {
            if (appliedSettings.Count > 0)
            {
                Debug.Log(
                    $"[BuildPresetApplier] ✅ Applied {appliedSettings.Count} settings: {string.Join(", ", appliedSettings)}"
                );
            }

            if (notFoundSettings.Count > 0)
            {
                Debug.LogWarning(
                    $"[BuildPresetApplier] ⚠️ Not found {notFoundSettings.Count} settings: {string.Join(", ", notFoundSettings)}"
                );
            }

            if (failedSettings.Count > 0)
            {
                Debug.LogError(
                    $"[BuildPresetApplier] ❌ Failed to apply {failedSettings.Count} settings: {string.Join(", ", failedSettings)}"
                );
            }

            if (!hasErrors
             && appliedSettings.Count > 0)
            {
                Debug.Log($"[BuildPresetApplier] ✓ Preset applied successfully. Total: {appliedSettings.Count} settings.");
            }
        }
    }
}
