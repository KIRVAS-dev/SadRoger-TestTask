using System.Collections.Generic;
using UnityEngine;

namespace BuildPresetTool
{
    public class PresetVerificationReport
    {
        public readonly List<string> MatchedSettings = new List<string>();
        readonly List<string> mismatchedSettings = new List<string>();
        readonly List<string> unreadableSettings = new List<string>();
        public bool HasIssues => mismatchedSettings.Count > 0 || unreadableSettings.Count > 0;

        public void AddMatched(string setting)
        {
            MatchedSettings.Add(setting);
        }

        public void AddMismatched(string setting, string expected, string actual)
        {
            mismatchedSettings.Add($"{setting}: expected '{expected}', got '{actual}'");
        }

        public void AddUnreadable(string setting, string reason = "")
        {
            unreadableSettings.Add(
                string.IsNullOrEmpty(reason)
                    ? setting
                    : $"{setting} ({reason})"
            );
        }

        public void LogSummary()
        {
            if (MatchedSettings.Count > 0)
            {
                Debug.Log(
                    $"[BuildPresetApplier] ✅ Verified {MatchedSettings.Count} settings match: {string.Join(", ", MatchedSettings)}"
                );
            }

            if (mismatchedSettings.Count > 0)
            {
                Debug.LogWarning($"[BuildPresetApplier] ⚠️ Mismatched {mismatchedSettings.Count} settings:");

                foreach (string mismatch in mismatchedSettings)
                {
                    Debug.LogWarning($"  • {mismatch}");
                }
            }

            if (unreadableSettings.Count > 0)
            {
                Debug.LogWarning(
                    $"[BuildPresetApplier] ⚠️ Could not verify {unreadableSettings.Count} settings: {string.Join(", ", unreadableSettings)}"
                );
            }

            if (!HasIssues
             && MatchedSettings.Count > 0)
            {
                Debug.Log(
                    $"[BuildPresetApplier] ✓ All verified settings match successfully. Total: {MatchedSettings.Count} settings."
                );
            }
        }
    }
}
