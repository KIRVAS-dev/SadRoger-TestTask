using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BuildPresetTool
{
    public class PresetValidationResult
    {
        public readonly List<string> Warnings = new List<string>();
        public readonly List<string> Errors = new List<string>();
        public bool IsValid => Errors.Count == 0;

        public void AddWarning(string message)
        {
            Warnings.Add(message);
        }

        public void AddError(string message)
        {
            Errors.Add(message);
        }
    }

    public static class PresetValidator
    {
        public static PresetValidationResult ValidatePreset(BuildPreset preset, SupportedPlatform platform)
        {
            var result = new PresetValidationResult();

            if (preset == null)
            {
                result.AddError("Preset is null");
                return result;
            }

            if (platform == SupportedPlatform.WebGL)
            {
                if (preset.maximum_memory_size < preset.initial_memory_size)
                {
                    result.AddError(
                        $"maximum_memory_size ({preset.maximum_memory_size} MB) cannot be less than initial_memory_size ({preset.initial_memory_size} MB)"
                    );
                }

                if (preset.maximum_memory_size > 2048)
                {
                    result.AddWarning(
                        $"maximum_memory_size ({preset.maximum_memory_size} MB) exceeds 2048 MB, which may cause performance issues"
                    );
                }

                if (preset.initial_memory_size < 16)
                {
                    result.AddWarning($"initial_memory_size ({preset.initial_memory_size} MB) is very low, may cause issues");
                }

#if UNITY_6000_0_OR_NEWER
                if (preset.threads_support
                 && !preset.webassembly_2023_feature_set)
                {
                    result.AddError("threads_support requires webassembly_2023_feature_set to be enabled");
                }
#else
                if (preset.threads_support)
                {
                    result.AddWarning("threads_support requires Unity 6 or newer and webassembly_2023_feature_set to be enabled");
                }
#endif

                if (string.IsNullOrEmpty(preset.memory_growth_mode))
                {
                    result.AddWarning("memory_growth_mode is not set");
                }
            }

            if (platform == SupportedPlatform.iOS)
            {
                if (preset.texture_compression == "DXT"
                 || preset.texture_compression == "DXTC")
                {
                    result.AddError("iOS does not support DXT texture compression. Use ASTC, PVRTC, or ETC2 instead");
                }
            }

            if (platform == SupportedPlatform.Android)
            {
                if (preset.texture_compression == "DXT"
                 || preset.texture_compression == "DXTC")
                {
                    result.AddWarning("DXT texture compression is not optimal for Android. Consider using ASTC or ETC2");
                }
            }

            if (string.IsNullOrEmpty(preset.gpu_skinning))
            {
                result.AddWarning("gpu_skinning is not set");
            }

            if (platform != SupportedPlatform.Windows
             && platform != SupportedPlatform.macOS
             && string.IsNullOrEmpty(preset.texture_compression))
            {
                result.AddWarning("texture_compression is not set");
            }

            if (preset.burst_enable_compilation
             && !preset.burst_enable_optimizations)
            {
                result.AddWarning("Burst compilation is enabled but optimizations are disabled");
            }

            if (string.IsNullOrEmpty(preset.il2cpp_code_generation))
            {
                result.AddWarning("il2cpp_code_generation is not set");
            }

            if (string.IsNullOrEmpty(preset.managed_stripping_level))
            {
                result.AddWarning("managed_stripping_level is not set");
            }

            return result;
        }

        public static PresetVerificationReport ValidateAfterApplication(BuildPreset expectedPreset, SupportedPlatform platform,
            PresetType type = PresetType.Development)
        {
            var result = new PresetVerificationReport();

            if (expectedPreset == null)
            {
                result.AddUnreadable("Preset", "Preset is null");
                return result;
            }

            try
            {
                BuildPreset actualPreset = SettingsReader.ReadCurrentSettings(platform);

                if (actualPreset == null)
                {
                    result.AddUnreadable("Settings", "Could not read current settings");
                    return result;
                }

                ValidateBool(result, "Strip Engine Code", expectedPreset.strip_engine_code, actualPreset.strip_engine_code);
                ValidateBool(result, "Graphics Jobs", expectedPreset.graphics_jobs, actualPreset.graphics_jobs);
                ValidateBool(result, "Incremental GC", expectedPreset.incremental_gc, actualPreset.incremental_gc);

                ValidateString(result, "GPU Skinning", expectedPreset.gpu_skinning, actualPreset.gpu_skinning);
                ValidateBool(result, "Static Batching", expectedPreset.static_batching, actualPreset.static_batching);

                if (platform != SupportedPlatform.Windows
                 && platform != SupportedPlatform.macOS)
                {
                    ValidateString(
                        result,
                        "Texture Compression",
                        expectedPreset.texture_compression,
                        actualPreset.texture_compression
                    );
                }

                ValidateString(
                    result,
                    "Managed Stripping Level",
                    expectedPreset.managed_stripping_level,
                    actualPreset.managed_stripping_level
                );

                ValidateString(
                    result,
                    "IL2CPP Code Generation",
                    expectedPreset.il2cpp_code_generation,
                    actualPreset.il2cpp_code_generation
                );

                ValidateBool(result, "Development Build", type == PresetType.Development, EditorUserBuildSettings.development);
                ValidateBool(result, "Script Debugging", expectedPreset.script_debugging, actualPreset.script_debugging);

                ValidateBool(
                    result,
                    "Deep Profiling Support",
                    expectedPreset.deep_profiling_support,
                    actualPreset.deep_profiling_support
                );

                ValidateBool(result, "Scripts Only Build", expectedPreset.scripts_only_build, actualPreset.scripts_only_build);

                ValidateBool(
                    result,
                    "Burst Compilation",
                    expectedPreset.burst_enable_compilation,
                    actualPreset.burst_enable_compilation
                );

                ValidateBool(
                    result,
                    "Burst Optimizations",
                    expectedPreset.burst_enable_optimizations,
                    actualPreset.burst_enable_optimizations
                );

                ValidateBool(
                    result,
                    "Burst Debug Info",
                    expectedPreset.burst_force_debug_info,
                    actualPreset.burst_force_debug_info
                );

                ValidateString(
                    result,
                    "Burst Debug Data Kind",
                    expectedPreset.burst_debug_data_kind,
                    actualPreset.burst_debug_data_kind
                );

                ValidateString(result, "Burst Optimize For", expectedPreset.burst_optimize_for, actualPreset.burst_optimize_for);
                ValidateString(result, "Burst Float Mode", expectedPreset.burst_float_mode, actualPreset.burst_float_mode);

                if (platform == SupportedPlatform.WebGL)
                {
                    ValidateString(result, "WebGL Compression", expectedPreset.compression, actualPreset.compression);

                    ValidateBool(
                        result,
                        "Decompression Fallback",
                        expectedPreset.decompression_fallback,
                        actualPreset.decompression_fallback
                    );

                    ValidateString(result, "Enable Exceptions", expectedPreset.enable_exceptions, actualPreset.enable_exceptions);

                    ValidateInt(
                        result,
                        "Initial Memory Size",
                        expectedPreset.initial_memory_size,
                        actualPreset.initial_memory_size
                    );

                    ValidateInt(
                        result,
                        "Maximum Memory Size",
                        expectedPreset.maximum_memory_size,
                        actualPreset.maximum_memory_size
                    );

                    ValidateString(
                        result,
                        "Memory Growth Mode",
                        expectedPreset.memory_growth_mode,
                        actualPreset.memory_growth_mode
                    );

                    ValidateBool(result, "Data Caching", expectedPreset.data_caching, actualPreset.data_caching);

                    ValidateBool(
                        result,
                        "Name Files As Hashes",
                        expectedPreset.name_files_as_hashes,
                        actualPreset.name_files_as_hashes
                    );

                    ValidateBool(result, "Show Diagnostics", expectedPreset.show_diagnostics, actualPreset.show_diagnostics);
                    ValidateBool(result, "Debug Symbols", expectedPreset.debug_symbols, actualPreset.debug_symbols);
                    ValidateBool(result, "Threads Support", expectedPreset.threads_support, actualPreset.threads_support);
                    ValidateString(result, "Power Preference", expectedPreset.power_preference, actualPreset.power_preference);

                    ValidateBool(
                        result,
                        "WebAssembly 2023",
                        expectedPreset.webassembly_2023_feature_set,
                        actualPreset.webassembly_2023_feature_set
                    );

                    ValidateString(result, "Code Optimization", expectedPreset.code_optimization, actualPreset.code_optimization);
                }

                if (platform == SupportedPlatform.Android
                 || platform == SupportedPlatform.iOS)
                {
                    ValidateString(
                        result,
                        "Accelerometer Frequency",
                        expectedPreset.accelerometer_frequency,
                        actualPreset.accelerometer_frequency
                    );
                }

                if (platform != SupportedPlatform.WebGL)
                {
                    ValidateString(result, "Code Optimization", expectedPreset.code_optimization, actualPreset.code_optimization);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PresetValidator] Error during post-application validation: {ex.Message}");
                result.AddUnreadable("Validation", ex.Message);
            }

            return result;
        }

        static void ValidateBool(PresetVerificationReport result, string name, bool expected, bool actual)
        {
            if (expected == actual)
            {
                result.AddMatched(name);
            }
            else
            {
                result.AddMismatched(name, expected.ToString(), actual.ToString());
            }
        }

        static void ValidateString(PresetVerificationReport result, string name, string expected, string actual)
        {
            if (string.IsNullOrEmpty(expected)
             && string.IsNullOrEmpty(actual))
            {
                result.AddMatched(name);
                return;
            }

            if (string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase))
            {
                result.AddMatched(name);
            }
            else
            {
                result.AddMismatched(name, expected ?? "null", actual ?? "null");
            }
        }

        static void ValidateInt(PresetVerificationReport result, string name, int expected, int actual)
        {
            if (expected == actual)
            {
                result.AddMatched(name);
            }
            else
            {
                result.AddMismatched(name, expected.ToString(), actual.ToString());
            }
        }
    }
}
