using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BuildPresetTool
{
    public enum SupportedPlatform
    {
        WebGL,
        Windows,
        macOS,
        Android,
        iOS
    }

    public enum PresetType
    {
        Development,
        Production,
        Custom
    }

    [Serializable]
    public class BuildPreset
    {
        public string compression = "Gzip";
        public bool decompression_fallback;
        public string enable_exceptions = "FullWithStacktrace";
        public string code_optimization = "DiskSize";
        public bool webassembly_2023_feature_set;
        public bool data_caching = true;
        public bool name_files_as_hashes;
        public bool show_diagnostics;
        public bool debug_symbols;
        public bool threads_support;
        public int initial_memory_size = 32;
        public int maximum_memory_size = 2048;
        public string memory_growth_mode = "Geometric";
        public string power_preference = "Default";

        public bool strip_engine_code;
        public string texture_compression = "ASTC";
        public bool static_batching = true;
        public string gpu_skinning = "GPU";
        public bool graphics_jobs;
        public bool incremental_gc = true;
        public string managed_stripping_level = "Minimal";
        public string il2cpp_code_generation = "OptimizeSpeed";

        public string accelerometer_frequency = "Disabled";

        public bool burst_enable_compilation = true;
        public bool burst_enable_optimizations = true;
        public bool burst_force_debug_info;
        public string burst_debug_data_kind = "None";
        public string burst_optimize_for = "Default";
        public string burst_float_mode = "Default";

        public bool script_debugging;
        public bool deep_profiling_support;
        public bool scripts_only_build;
    }

    [Serializable]
    public class NamedPreset
    {
        public string name;
        public BuildPreset preset;
    }

    [Serializable]
    public class PlatformPresets
    {
        public BuildPreset development,
            production;
        public NamedPreset[] customPresets;
    }

    [Serializable]
    public class PlatformsContainer
    {
        public PlatformPresets WebGL,
            Windows,
            macOS,
            Android,
            iOS;
    }

    [Serializable]
    public class BuildPresetsRoot
    {
        public PlatformsContainer platforms;
    }

    public static class BuildPresetApplier
    {
        static string _buildPresetsJsonAssetPath;

        static string GetBuildPresetsJsonAssetPath()
        {
            if (_buildPresetsJsonAssetPath != null)
            {
                return _buildPresetsJsonAssetPath;
            }

            string[] guids = AssetDatabase.FindAssets("BuildPresets");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path != null
                 && path.EndsWith("BuildPresets.json", StringComparison.OrdinalIgnoreCase))
                {
                    _buildPresetsJsonAssetPath = path;
                    return path;
                }
            }

            return null;
        }

        static string JsonPath
        {
            get
            {
                string assetPath = GetBuildPresetsJsonAssetPath();

                if (string.IsNullOrEmpty(assetPath))
                {
                    return Path.Combine(Application.dataPath, "BuildPresets.json");
                }

                string relative = assetPath.Length > 7
                 && assetPath.Substring(startIndex: 0, length: 7).Equals("Assets/", StringComparison.OrdinalIgnoreCase)
                        ? assetPath.Substring(7)
                        : assetPath;

                return Path.Combine(Application.dataPath, relative.Replace(oldChar: '/', Path.DirectorySeparatorChar));
            }
        }

        internal static string GetBuildPresetsJsonAssetPathForEditor()
        {
            return GetBuildPresetsJsonAssetPath();
        }

        const string ProjectSettingsPath = "ProjectSettings/ProjectSettings.asset";

        static readonly BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
        static readonly BindingFlags AnyInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

#region Code Optimization Mapping
        static string MapCodeOptimizationToUnity(string presetValue)
        {
            return presetValue switch
            {
                "DiskSizeWithLTO" => "DiskSizeLTO",
                "DiskSize" => "size",
                _ => presetValue
            };
        }

        internal static string MapCodeOptimizationFromUnity(string unityValue)
        {
            return unityValue switch
            {
                "DiskSizeLTO" => "DiskSizeWithLTO",
                "size" => "DiskSize",
                _ => unityValue
            };
        }
#endregion

#region Load/Save Presets
        static BuildPresetsRoot LoadPresets()
        {
            if (!File.Exists(JsonPath))
            {
                _buildPresetsJsonAssetPath = null;
                return null;
            }

            try
            {
                return JsonUtility.FromJson<BuildPresetsRoot>(File.ReadAllText(JsonPath));
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildPresetApplier] Failed to load presets from {JsonPath}: {ex.Message}");
                return null;
            }
        }

        public static BuildPreset GetPreset(SupportedPlatform platform, PresetType type)
        {
            BuildPresetsRoot root = LoadPresets();

            if (root?.platforms == null)
            {
                return null;
            }

            PlatformPresets platformPresets = GetPlatformPresets(root, platform);

            return type switch
            {
                PresetType.Development => platformPresets?.development,
                PresetType.Production => platformPresets?.production,
                PresetType.Custom => null,
                _ => null
            };
        }

        public static BuildPreset GetCustomPreset(SupportedPlatform platform, string presetName)
        {
            if (string.IsNullOrEmpty(presetName))
            {
                return null;
            }

            BuildPresetsRoot root = LoadPresets();

            if (root?.platforms == null)
            {
                return null;
            }

            PlatformPresets pp = GetPlatformPresets(root, platform);

            if (pp?.customPresets == null)
            {
                return null;
            }

            foreach (NamedPreset np in pp.customPresets)
            {
                if (string.Equals(np.name, presetName, StringComparison.OrdinalIgnoreCase))
                {
                    return np.preset;
                }
            }

            return null;
        }

        public static string[] GetCustomPresetNames(SupportedPlatform platform)
        {
            BuildPresetsRoot root = LoadPresets();

            if (root?.platforms == null)
            {
                return Array.Empty<string>();
            }

            PlatformPresets pp = GetPlatformPresets(root, platform);

            if (pp?.customPresets == null
             || pp.customPresets.Length == 0)
            {
                return Array.Empty<string>();
            }

            return pp.customPresets.Select(np => np.name ?? "").ToArray();
        }

        public static void SaveCurrentAsCustom(SupportedPlatform platform, string presetName)
        {
            BuildPresetsRoot root = LoadPresets() ?? new BuildPresetsRoot { platforms = new PlatformsContainer() };
            PlatformPresets platformPresets = GetOrCreatePlatformPresets(root, platform);

            if (platformPresets.customPresets == null)
            {
                platformPresets.customPresets = Array.Empty<NamedPreset>();
            }

            var list = new List<NamedPreset>(platformPresets.customPresets);
            int existing = list.FindIndex(np => string.Equals(np.name, presetName, StringComparison.OrdinalIgnoreCase));

            var newPreset = new NamedPreset
            {
                name = presetName,
                preset = SettingsReader.ReadCurrentSettings(platform)
            };

            if (existing >= 0)
            {
                list[existing] = newPreset;
            }
            else
            {
                list.Add(newPreset);
            }

            platformPresets.customPresets = list.ToArray();
            SavePresets(root);
            Debug.Log($"[BuildPresetApplier] Saved current settings as Custom preset \"{presetName}\" for {platform}.");
        }

        public static void DeleteCustomPreset(SupportedPlatform platform, string presetName)
        {
            BuildPresetsRoot root = LoadPresets();

            if (root?.platforms == null)
            {
                return;
            }

            PlatformPresets pp = GetPlatformPresets(root, platform);

            if (pp?.customPresets == null)
            {
                return;
            }

            List<NamedPreset> list =
                pp.customPresets.Where(np => !string.Equals(np.name, presetName, StringComparison.OrdinalIgnoreCase)).ToList();

            pp.customPresets = list.ToArray();
            SavePresets(root);
            Debug.Log($"[BuildPresetApplier] Deleted custom preset \"{presetName}\" for {platform}.");
        }

        public static void DeleteAllCustomPresets(SupportedPlatform platform)
        {
            BuildPresetsRoot root = LoadPresets();

            if (root?.platforms == null)
            {
                return;
            }

            PlatformPresets pp = GetPlatformPresets(root, platform);

            if (pp == null)
            {
                return;
            }

            pp.customPresets = Array.Empty<NamedPreset>();
            SavePresets(root);
            Debug.Log($"[BuildPresetApplier] Deleted all custom presets for {platform}.");
        }

        public static bool IsCustomPresetEmpty(SupportedPlatform platform)
        {
            string[] names = GetCustomPresetNames(platform);
            return names == null || names.Length == 0;
        }

        const string EditorPrefsKeyPlatform = "BuildPresetTool.LastApplied.Platform";
        const string EditorPrefsKeyPresetType = "BuildPresetTool.LastApplied.PresetType";
        const string EditorPrefsKeyCustomName = "BuildPresetTool.LastApplied.CustomPresetName";

        public static void SaveLastAppliedPreset(SupportedPlatform platform, PresetType presetType,
            string customPresetName = null)
        {
            EditorPrefs.SetInt(EditorPrefsKeyPlatform, (int)platform);
            EditorPrefs.SetInt(EditorPrefsKeyPresetType, (int)presetType);
            EditorPrefs.SetString(EditorPrefsKeyCustomName, customPresetName ?? "");
        }

        public static bool TryLoadLastAppliedPreset(out SupportedPlatform platform, out PresetType presetType,
            out string customPresetName)
        {
            platform = SupportedPlatform.WebGL;
            presetType = PresetType.Development;
            customPresetName = "";

            if (!EditorPrefs.HasKey(EditorPrefsKeyPlatform)
             || !EditorPrefs.HasKey(EditorPrefsKeyPresetType))
            {
                return false;
            }

            int platformInt = EditorPrefs.GetInt(EditorPrefsKeyPlatform);
            int typeInt = EditorPrefs.GetInt(EditorPrefsKeyPresetType);

            if (!Enum.IsDefined(typeof(SupportedPlatform), platformInt)
             || !Enum.IsDefined(typeof(PresetType), typeInt))
            {
                return false;
            }

            platform = (SupportedPlatform)platformInt;
            presetType = (PresetType)typeInt;
            customPresetName = EditorPrefs.GetString(EditorPrefsKeyCustomName, "");
            return true;
        }

        static void SavePresets(BuildPresetsRoot root)
        {
            File.WriteAllText(JsonPath, JsonUtility.ToJson(root, prettyPrint: true));
            AssetDatabase.Refresh();
        }
#endregion

#region Apply Preset
        public static void ApplyPreset(SupportedPlatform platform, string customPresetName,
            Action<string, float> onProgress = null)
        {
            BuildPreset preset = GetCustomPreset(platform, customPresetName);

            if (preset == null)
            {
                Debug.LogWarning($"[BuildPresetApplier] Custom preset not found: {platform}/\"{customPresetName}\"");
                return;
            }

            ApplyPresetInternal(
                platform,
                PresetType.Custom,
                preset,
                onProgress,
                customPresetName
            );
        }

        public static void ApplyPreset(SupportedPlatform platform, PresetType type, Action<string, float> onProgress = null)
        {
            BuildPreset preset = GetPreset(platform, type);

            if (preset == null)
            {
                Debug.LogWarning($"[BuildPresetApplier] Preset not found: {platform}/{type}");
                return;
            }

            ApplyPresetInternal(
                platform,
                type,
                preset,
                onProgress,
                customPresetName: null
            );
        }

        static void ApplyPresetInternal(SupportedPlatform platform, PresetType type, BuildPreset preset,
            Action<string, float> onProgress = null, string customPresetName = null)
        {
            PresetValidationResult validation = PresetValidator.ValidatePreset(preset, platform);

            if (!validation.IsValid)
            {
                Debug.LogError($"[BuildPresetApplier] Preset validation failed for {platform}/{type}:");

                foreach (string error in validation.Errors)
                {
                    Debug.LogError($"  ❌ {error}");
                }

                foreach (string warning in validation.Warnings)
                {
                    Debug.LogWarning($"  ⚠️ {warning}");
                }

                Debug.LogError("[BuildPresetApplier] Preset application aborted due to validation errors.");
                return;
            }

            if (validation.Warnings.Count > 0)
            {
                Debug.LogWarning($"[BuildPresetApplier] Preset validation warnings for {platform}/{type}:");

                foreach (string warning in validation.Warnings)
                {
                    Debug.LogWarning($"  ⚠️ {warning}");
                }
            }

            var result = new PresetApplicationReport();
            BuildTarget target = SettingsReader.GetBuildTarget(platform);
            BuildTargetGroup group = SettingsReader.GetBuildTargetGroup(platform);

            try
            {
                onProgress?.Invoke($"Applying {platform}/{type} preset...", arg2: 0.1f);

                onProgress?.Invoke("Build Options...", arg2: 0.2f);
                ApplyBuildSettingsOptions(preset, result);

                onProgress?.Invoke("Player Settings...", arg2: 0.4f);
                ApplyDirectPlayerSettings(preset, result);

                onProgress?.Invoke("Reflection Settings...", arg2: 0.5f);

                ApplyReflectionBasedSettings(
                    platform,
                    target,
                    group,
                    preset,
                    result
                );

                if (platform == SupportedPlatform.WebGL)
                {
                    onProgress?.Invoke("WebGL Settings...", arg2: 0.6f);
                    ApplyWebGLSettings(preset, result);
                }

                if (platform == SupportedPlatform.Android
                 || platform == SupportedPlatform.iOS)
                {
                    onProgress?.Invoke("Mobile Settings...", arg2: 0.65f);
                    ApplyMobileSettings(preset, result);
                }

                if (platform != SupportedPlatform.WebGL)
                {
                    onProgress?.Invoke("Code Optimization...", arg2: 0.7f);
                    ApplyCodeOptimization(platform, preset.code_optimization, result);
                }

                onProgress?.Invoke("Saving assets...", arg2: 0.85f);
                AssetDatabase.SaveAssets();

                onProgress?.Invoke("Burst Settings...", arg2: 0.9f);
                ApplyBurstDebugDataKind(target, preset.burst_debug_data_kind, result);

                onProgress?.Invoke("Build Settings...", arg2: 0.95f);
                ApplyBuildSettings(type, result);

                onProgress?.Invoke("Finalizing...", arg2: 0.96f);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                onProgress?.Invoke("Validating...", arg2: 0.98f);

                PresetVerificationReport postValidation = PresetValidator.ValidateAfterApplication(preset, platform, type);

                if (postValidation.HasIssues)
                {
                    Debug.LogWarning($"[BuildPresetApplier] Post-application validation found issues for {platform}/{type}:");
                    postValidation.LogSummary();
                }
                else if (postValidation.MatchedSettings.Count > 0)
                {
                    Debug.Log($"[BuildPresetApplier] Post-application validation passed for {platform}/{type}");
                    postValidation.LogSummary();
                }

                onProgress?.Invoke("Complete!", arg2: 1.0f);

                SaveLastAppliedPreset(
                    platform,
                    type,
                    type == PresetType.Custom
                        ? customPresetName
                        : null
                );

                result.LogSummary();
            }
            finally
            {
                onProgress?.Invoke(arg1: null, arg2: -1f);
            }
        }

        static void ApplyDirectPlayerSettings(BuildPreset preset, PresetApplicationReport result = null)
        {
            try
            {
                PlayerSettings.stripEngineCode = preset.strip_engine_code;
                result?.AddApplied("Strip Engine Code");

                PlayerSettings.graphicsJobs = preset.graphics_jobs;
                result?.AddApplied("Graphics Jobs");

                PlayerSettings.gcIncremental = preset.incremental_gc;
                result?.AddApplied("Incremental GC");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildPresetApplier] Error applying direct PlayerSettings: {ex.Message}");
                result?.AddFailed("Direct Player Settings", ex.Message);
            }
        }

        static void ApplyReflectionBasedSettings(SupportedPlatform platform, BuildTarget target, BuildTargetGroup group,
            BuildPreset preset, PresetApplicationReport result = null)
        {
            ApplyGPUSkinning(preset.gpu_skinning, result);
            ApplyStaticBatching(target, preset.static_batching, result);

            if (platform != SupportedPlatform.Windows
             && platform != SupportedPlatform.macOS)
            {
                ApplyTextureCompression(target, preset.texture_compression, result);
            }

            ApplyManagedStrippingLevel(group, preset.managed_stripping_level, result);
            ApplyIL2CPPCodeGeneration(target, preset.il2cpp_code_generation, result);
            ApplyBurstSettings(target, preset, result);
        }

        static void ApplyBuildSettings(PresetType type, PresetApplicationReport result = null)
        {
            EditorUserBuildSettings.development = type == PresetType.Development;
            result?.AddApplied("Development Build");
        }

        static void ApplyBuildSettingsOptions(BuildPreset preset, PresetApplicationReport result = null)
        {
            try
            {
                PropertyInfo allowDebuggingProp = typeof(EditorUserBuildSettings).GetProperty("allowDebugging", PublicStatic);

                if (allowDebuggingProp != null)
                {
                    allowDebuggingProp.SetValue(obj: null, preset.script_debugging);
                    result?.AddApplied("Script Debugging");
                }
                else
                {
                    result?.AddNotFound("Script Debugging");
                }

                PropertyInfo deepProfilingProp =
                    typeof(EditorUserBuildSettings).GetProperty("buildWithDeepProfilingSupport", PublicStatic);

                if (deepProfilingProp != null)
                {
                    deepProfilingProp.SetValue(obj: null, preset.deep_profiling_support);
                    result?.AddApplied("Deep Profiling Support");
                }
                else
                {
                    result?.AddNotFound("Deep Profiling Support");
                }

                PropertyInfo scriptsOnlyProp = typeof(EditorUserBuildSettings).GetProperty("buildScriptsOnly", PublicStatic);

                if (scriptsOnlyProp != null)
                {
                    scriptsOnlyProp.SetValue(obj: null, preset.scripts_only_build);
                    result?.AddApplied("Scripts Only Build");
                }
                else
                {
                    result?.AddNotFound("Scripts Only Build");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildPresetApplier] Error applying BuildSettings options: {ex.Message}");
                result?.AddFailed("Build Settings Options", ex.Message);
            }
        }

        static void ApplyWebGLSettings(BuildPreset preset, PresetApplicationReport result = null)
        {
            Type webgl = typeof(PlayerSettings).GetNestedType("WebGL");

            if (webgl == null)
            {
                result?.AddNotFound("WebGL Settings");
                return;
            }

            SetEnumProperty(webgl, "compressionFormat", preset.compression);
            result?.AddApplied($"WebGL Compression ({preset.compression})");
            SetProperty(webgl, "decompressionFallback", preset.decompression_fallback);
            result?.AddApplied($"Decompression Fallback ({preset.decompression_fallback})");
            ApplyWebGLExceptionSupportDirect(preset.enable_exceptions);
            result?.AddApplied($"WebGL Exceptions ({preset.enable_exceptions})");
            SetProperty(webgl, "initialMemorySize", preset.initial_memory_size);
            result?.AddApplied($"Initial Memory Size ({preset.initial_memory_size} MB)");
            SetProperty(webgl, "maximumMemorySize", preset.maximum_memory_size);
            result?.AddApplied($"Maximum Memory Size ({preset.maximum_memory_size} MB)");
            SetEnumProperty(webgl, "memoryGrowthMode", preset.memory_growth_mode);
            result?.AddApplied($"Memory Growth Mode ({preset.memory_growth_mode})");
            SetProperty(webgl, "dataCaching", preset.data_caching);
            result?.AddApplied($"Data Caching ({preset.data_caching})");
            SetProperty(webgl, "nameFilesAsHashes", preset.name_files_as_hashes);
            result?.AddApplied($"Name Files As Hashes ({preset.name_files_as_hashes})");
            SetProperty(webgl, "showDiagnostics", preset.show_diagnostics);
            result?.AddApplied($"Show Diagnostics ({preset.show_diagnostics})");
            SetProperty(webgl, "debugSymbols", preset.debug_symbols);
            result?.AddApplied($"Debug Symbols ({preset.debug_symbols})");
            SetProperty(webgl, "threadsSupport", preset.threads_support);
            result?.AddApplied($"Threads Support ({preset.threads_support})");
            SetEnumProperty(webgl, "powerPreference", preset.power_preference);
            result?.AddApplied($"Power Preference ({preset.power_preference})");
            ApplyWebAssembly2023(preset.webassembly_2023_feature_set);
            result?.AddApplied($"WebAssembly 2023 ({preset.webassembly_2023_feature_set})");

            string codeOpt = MapCodeOptimizationToUnity(preset.code_optimization);
            EditorUserBuildSettings.SetPlatformSettings("WebGL", "CodeOptimization", codeOpt);
            result?.AddApplied($"Code Optimization ({preset.code_optimization})");

            TrySetBuildSubtarget();
        }

        static void ApplyCodeOptimization(SupportedPlatform platform, string codeOptimization,
            PresetApplicationReport result = null)
        {
            try
            {
                string platformName = platform switch
                {
                    SupportedPlatform.Android => "Android",
                    SupportedPlatform.iOS => "iOS",
                    SupportedPlatform.Windows => "Standalone",
                    SupportedPlatform.macOS => "Standalone",
                    _ => null
                };

                if (string.IsNullOrEmpty(platformName)
                 || string.IsNullOrEmpty(codeOptimization))
                {
                    result?.AddNotFound("Code Optimization");
                    return;
                }

                string codeOpt = MapCodeOptimizationToUnity(codeOptimization);
                EditorUserBuildSettings.SetPlatformSettings(platformName, "CodeOptimization", codeOpt);
                result?.AddApplied($"Code Optimization ({codeOptimization})");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BuildPresetApplier] Could not apply Code Optimization for {platform}: {ex.Message}");
                result?.AddFailed("Code Optimization", ex.Message);
            }
        }

        static void TrySetBuildSubtarget()
        {
            try
            {
                PropertyInfo prop = typeof(EditorUserBuildSettings).GetProperty("webGLBuildSubtarget", PublicStatic);

                if (prop == null)
                {
                    Debug.LogWarning("[BuildPresetApplier] webGLBuildSubtarget property not found");
                    return;
                }

                prop.SetValue(obj: null, Enum.ToObject(prop.PropertyType, value: 0));
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BuildPresetApplier] Failed to set webGLBuildSubtarget: {ex.Message}");
            }
        }

        static void ApplyWebGLExceptionSupportDirect(string exceptionSupport)
        {
            try
            {
                Type webgl = typeof(PlayerSettings).GetNestedType("WebGL");

                if (webgl == null)
                {
                    Debug.LogError("[BuildPresetApplier] PlayerSettings.WebGL type not found");
                    return;
                }

                PropertyInfo prop = webgl.GetProperty("exceptionSupport");

                if (prop == null)
                {
                    Debug.LogError("[BuildPresetApplier] exceptionSupport property not found in PlayerSettings.WebGL");
                    return;
                }

                Type enumType = prop.PropertyType;
                string[] enumNames = Enum.GetNames(enumType);

                string mappedValue = exceptionSupport switch
                {
                    "ExplicitlyThrown" => "ExplicitlyThrownExceptionsOnly",
                    "FullWithStacktrace" => "FullWithStacktrace",
                    "FullWithoutStacktrace" => "FullWithoutStacktrace",
                    "None" => "None",
                    _ => exceptionSupport
                };

                object enumValue = null;

                if (!Enum.TryParse(enumType, mappedValue, ignoreCase: true, out enumValue))
                {
                    string matchingName = enumNames.FirstOrDefault(n =>
                        string.Equals(n, mappedValue, StringComparison.OrdinalIgnoreCase)
                     || n.Replace("_", "").Equals(mappedValue.Replace("_", ""), StringComparison.OrdinalIgnoreCase)
                    );

                    if (matchingName != null)
                    {
                        enumValue = Enum.Parse(enumType, matchingName);
                    }
                }

                if (enumValue != null)
                {
                    prop.SetValue(obj: null, enumValue);

                    object currentValue = prop.GetValue(null);

                    if (!currentValue.ToString().Equals(enumValue.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.LogWarning(
                            $"[BuildPresetApplier] exceptionSupport value mismatch! Expected: {enumValue}, Got: {currentValue}"
                        );
                    }
                }
                else
                {
                    Debug.LogWarning(
                        $"[BuildPresetApplier] Could not find exceptionSupport value '{mappedValue}' (from '{exceptionSupport}') in available values: {string.Join(", ", enumNames)}"
                    );
                }

                for (int attempt = 0; attempt < 3; attempt++)
                {
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(ProjectSettingsPath);

                    if (assets != null
                     && assets.Length > 0)
                    {
                        var so = new SerializedObject(assets[0]);
                        so.Update();
                        SerializedProperty exceptionProp = so.FindProperty("webGLExceptionSupport");

                        if (exceptionProp != null
                         && exceptionProp.propertyType == SerializedPropertyType.Integer)
                        {
                            int fileValue = mappedValue switch
                            {
                                "None" => 0,
                                "ExplicitlyThrownExceptionsOnly" => 1,
                                "FullWithoutStacktrace" => 2,
                                "FullWithStacktrace" => 3,
                                _ => 0
                            };

                            exceptionProp.intValue = fileValue;
                            so.ApplyModifiedProperties();
                            EditorUtility.SetDirty(assets[0]);
                        }
                    }

                    prop.SetValue(obj: null, enumValue);
                    AssetDatabase.SaveAssets();

                    object verifyValue = prop.GetValue(null);

                    if (verifyValue.ToString().Equals(enumValue.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    if (attempt == 2)
                    {
                        Debug.LogWarning(
                            $"[BuildPresetApplier] exceptionSupport not applied correctly after 3 attempts. Current: {verifyValue}, Expected: {enumValue}"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    $"[BuildPresetApplier] Error applying WebGL exception support directly: {ex.Message}\n{ex.StackTrace}"
                );
            }
        }

        static void ApplyWebAssembly2023(bool enabled)
        {
#if !UNITY_6000_0_OR_NEWER
        Debug.LogWarning("[BuildPresetApplier] WebAssembly 2023 feature requires Unity 6 or newer");
        return;
#endif
            try
            {
                int value = enabled
                    ? 1
                    : 0;

                for (int attempt = 0; attempt < 3; attempt++)
                {
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(ProjectSettingsPath);

                    if (assets != null
                     && assets.Length > 0)
                    {
                        var so = new SerializedObject(assets[0]);
                        so.Update();

                        string[] propertyNames =
                        {
                            "webWasm2023",
                            "webAssembly2023FeatureSet"
                        };

                        SerializedProperty prop = FindPropertyInProjectSettings(so, propertyNames);

                        if (prop != null)
                        {
                            if (prop.propertyType == SerializedPropertyType.Integer)
                            {
                                prop.intValue = value;
                            }
                            else if (prop.propertyType == SerializedPropertyType.Boolean)
                            {
                                prop.boolValue = value == 1;
                            }
                            else
                            {
                                Debug.LogWarning(
                                    $"[BuildPresetApplier] webWasm2023 property has unexpected type: {prop.propertyType}"
                                );

                                prop = null;
                            }

                            if (prop != null)
                            {
                                so.ApplyModifiedProperties();
                                EditorUtility.SetDirty(assets[0]);
                                AssetDatabase.SaveAssets();

                                so.Update();
                                SerializedProperty verifyProp = FindPropertyInProjectSettings(so, propertyNames);

                                if (verifyProp != null)
                                {
                                    bool matches = false;

                                    if (verifyProp.propertyType == SerializedPropertyType.Integer)
                                    {
                                        matches = verifyProp.intValue == value;
                                    }
                                    else if (verifyProp.propertyType == SerializedPropertyType.Boolean)
                                    {
                                        matches = verifyProp.boolValue && value == 1 || !verifyProp.boolValue && value == 0;
                                    }

                                    if (matches)
                                    {
                                        break;
                                    }

                                    if (attempt == 2)
                                    {
                                        Debug.LogWarning(
                                            $"[BuildPresetApplier] webWasm2023 value mismatch after 3 attempts! Expected: {value}, Got: {(verifyProp.propertyType == SerializedPropertyType.Integer ? verifyProp.intValue.ToString() : verifyProp.boolValue.ToString())}"
                                        );
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (attempt == 0)
                            {
                                Debug.LogWarning(
                                    "[BuildPresetApplier] webWasm2023 property not found or wrong type in ProjectSettings"
                                );
                            }

                            if (attempt >= 1)
                            {
                                try
                                {
                                    Type playerSettingsType = typeof(PlayerSettings);

                                    FieldInfo field = playerSettingsType.GetField(
                                        "webWasm2023",
                                        AnyInstance | BindingFlags.Static
                                    );

                                    if (field != null)
                                    {
                                        field.SetValue(obj: null, value);
                                        AssetDatabase.SaveAssets();
                                        Debug.Log($"[BuildPresetApplier] Applied webWasm2023 via reflection fallback: {value}");

                                        object verifyValue = field.GetValue(null);

                                        if (verifyValue is int intVal
                                         && intVal == value)
                                        {
                                            break;
                                        }

                                        if (verifyValue is bool boolVal
                                         && (boolVal && value == 1 || !boolVal && value == 0))
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        PropertyInfo reflectionProp = playerSettingsType.GetProperty(
                                            "webWasm2023",
                                            AnyInstance | BindingFlags.Static
                                        );

                                        if (reflectionProp != null
                                         && reflectionProp.CanWrite)
                                        {
                                            reflectionProp.SetValue(obj: null, value);
                                            AssetDatabase.SaveAssets();

                                            Debug.Log(
                                                $"[BuildPresetApplier] Applied webWasm2023 via property reflection fallback: {value}"
                                            );

                                            break;
                                        }
                                    }
                                }
                                catch (Exception fallbackEx)
                                {
                                    if (attempt == 2)
                                    {
                                        Debug.LogWarning(
                                            $"[BuildPresetApplier] Reflection fallback also failed: {fallbackEx.Message}"
                                        );
                                    }
                                }
                            }

                            if (attempt == 2)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (attempt == 0)
                        {
                            Debug.LogWarning("[BuildPresetApplier] Could not load ProjectSettings.asset");
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildPresetApplier] Error applying WebAssembly 2023: {ex.Message}\n{ex.StackTrace}");
            }
        }

        static void ApplyMobileSettings(BuildPreset preset, PresetApplicationReport result = null)
        {
            try
            {
                if (string.IsNullOrEmpty(preset.accelerometer_frequency))
                {
                    result?.AddNotFound("Accelerometer Frequency");
                    return;
                }

                MethodInfo method = typeof(PlayerSettings).GetMethod("SetAccelerometerFrequency", PublicStatic);

                if (method == null)
                {
                    Debug.LogWarning("[BuildPresetApplier] SetAccelerometerFrequency method not found");
                    result?.AddNotFound("Accelerometer Frequency");
                    return;
                }

                Type frequencyType = method.GetParameters()[0].ParameterType;
                object frequencyValue = Enum.Parse(frequencyType, preset.accelerometer_frequency);
                method.Invoke(obj: null, new[] { frequencyValue });
                result?.AddApplied($"Accelerometer Frequency ({preset.accelerometer_frequency})");
            }
            catch (Exception ex)
            {
                Debug.LogWarning(
                    $"[BuildPresetApplier] Failed to set accelerometer frequency '{preset.accelerometer_frequency}': {ex.Message}"
                );

                result?.AddFailed("Accelerometer Frequency", ex.Message);
            }
        }

        static void ApplyGPUSkinning(string value, PresetApplicationReport result = null)
        {
            try
            {
                int index = value switch
                {
                    "CPU" => 0,
                    "GPU" => 1,
                    "GPUBatched" => 2,
                    _ => 1
                };

                PropertyInfo prop = typeof(PlayerSettings).GetProperty("gpuSkinning", PublicStatic);

                if (prop != null)
                {
                    if (prop.PropertyType == typeof(bool))
                    {
                        bool boolValue = index != 0;
                        prop.SetValue(obj: null, boolValue);
                    }
                    else if (prop.PropertyType == typeof(int))
                    {
                        prop.SetValue(obj: null, index);
                    }

                    result?.AddApplied($"GPU Skinning ({value})");
                }
                else
                {
                    Debug.LogWarning("[BuildPresetApplier] PlayerSettings.gpuSkinning property not found");
                    result?.AddNotFound("GPU Skinning");
                }

                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(ProjectSettingsPath);

                if (assets != null
                 && assets.Length > 0)
                {
                    var so = new SerializedObject(assets[0]);
                    so.Update();
                    SerializedProperty serializedProp = so.FindProperty("gpuSkinning");

                    if (serializedProp != null
                     && serializedProp.propertyType == SerializedPropertyType.Integer)
                    {
                        if (serializedProp.intValue != index)
                        {
                            serializedProp.intValue = index;
                            so.ApplyModifiedProperties();
                            EditorUtility.SetDirty(assets[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildPresetApplier] Failed to apply GPU Skinning '{value}': {ex.Message}");
                result?.AddFailed("GPU Skinning", ex.Message);
            }
        }

        static void ApplyStaticBatching(BuildTarget target, bool enabled, PresetApplicationReport result = null)
        {
            try
            {
                MethodInfo method = typeof(PlayerSettings).GetMethod("SetStaticBatchingForPlatform", PublicStatic);

                if (method == null)
                {
                    Debug.LogWarning($"[BuildPresetApplier] SetStaticBatchingForPlatform method not found for {target}");
                    result?.AddNotFound("Static Batching");
                    return;
                }

                method.Invoke(
                    obj: null,
                    new object[]
                    {
                        target,
                        enabled
                    }
                );

                result?.AddApplied($"Static Batching ({enabled})");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BuildPresetApplier] Failed to set static batching for {target}: {ex.Message}");
                result?.AddFailed("Static Batching", ex.Message);
            }
        }

        static void ApplyTextureCompression(BuildTarget target, string format, PresetApplicationReport result = null)
        {
            try
            {
                string internalName = format switch
                {
                    "DXT" => "DXTC",
                    "ETC2" => "ETC2",
                    "ASTC" => "ASTC",
                    "PVRTC" => "PVRTC",
                    _ => "ASTC"
                };

                Type formatType = typeof(EditorUserBuildSettings).Assembly.GetType("UnityEditor.TextureCompressionFormat");

                if (formatType == null)
                {
                    Debug.LogWarning("[BuildPresetApplier] TextureCompressionFormat type not found");
                    result?.AddNotFound("Texture Compression");
                    return;
                }

                MethodInfo method = typeof(PlayerSettings).GetMethod(
                    "SetDefaultTextureCompressionFormat",
                    PublicStatic | BindingFlags.NonPublic
                );

                if (method == null)
                {
                    Debug.LogWarning($"[BuildPresetApplier] SetDefaultTextureCompressionFormat method not found for {target}");
                    result?.AddNotFound("Texture Compression");
                    return;
                }

                method.Invoke(
                    obj: null,
                    new[]
                    {
                        target,
                        Enum.Parse(formatType, internalName)
                    }
                );

                result?.AddApplied($"Texture Compression ({format})");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BuildPresetApplier] Failed to set texture compression '{format}' for {target}: {ex.Message}");
                result?.AddFailed("Texture Compression", ex.Message);
            }
        }

        static void ApplyManagedStrippingLevel(BuildTargetGroup group, string level, PresetApplicationReport result = null)
        {
            try
            {
                object namedTarget = GetNamedBuildTarget(group);

                if (namedTarget == null)
                {
                    Debug.LogWarning($"[BuildPresetApplier] NamedBuildTarget not found for {group}");
                    result?.AddNotFound("Managed Stripping Level");
                    return;
                }

                Type strippingType = typeof(ManagedStrippingLevel);
                Type nbtType = namedTarget.GetType();

                MethodInfo method = typeof(PlayerSettings).GetMethod(
                    "SetManagedStrippingLevel",
                    new[]
                    {
                        nbtType,
                        strippingType
                    }
                );

                if (method == null)
                {
                    Debug.LogWarning($"[BuildPresetApplier] SetManagedStrippingLevel method not found for {group}");
                    result?.AddNotFound("Managed Stripping Level");
                    return;
                }

                method.Invoke(
                    obj: null,
                    new[]
                    {
                        namedTarget,
                        Enum.Parse(strippingType, level)
                    }
                );

                result?.AddApplied($"Managed Stripping Level ({level})");
            }
            catch (Exception ex)
            {
                Debug.LogWarning(
                    $"[BuildPresetApplier] Failed to set managed stripping level '{level}' for {group}: {ex.Message}"
                );

                result?.AddFailed("Managed Stripping Level", ex.Message);
            }
        }

        static void ApplyIL2CPPCodeGeneration(BuildTarget target, string codeGen, PresetApplicationReport result = null)
        {
            try
            {
                BuildTargetGroup group = target switch
                {
                    BuildTarget.WebGL => BuildTargetGroup.WebGL,
                    BuildTarget.StandaloneWindows64 or BuildTarget.StandaloneOSX => BuildTargetGroup.Standalone,
                    BuildTarget.Android => BuildTargetGroup.Android,
                    BuildTarget.iOS => BuildTargetGroup.iOS,
                    _ => BuildTargetGroup.Unknown
                };

                if (group == BuildTargetGroup.Unknown)
                {
                    Debug.LogWarning($"[BuildPresetApplier] Unknown BuildTargetGroup for {target}");
                    result?.AddNotFound("IL2CPP Code Generation");
                    return;
                }

                object namedTarget = GetNamedBuildTarget(group);

                if (namedTarget == null)
                {
                    Debug.LogWarning($"[BuildPresetApplier] NamedBuildTarget not found for {group}");
                    result?.AddNotFound("IL2CPP Code Generation");
                    return;
                }

                Type nbtType = namedTarget.GetType();
                MethodInfo getMethod = typeof(PlayerSettings).GetMethod("GetIl2CppCodeGeneration", new[] { nbtType });

                if (getMethod == null)
                {
                    Debug.LogWarning($"[BuildPresetApplier] GetIl2CppCodeGeneration method not found for {group}");
                    result?.AddNotFound("IL2CPP Code Generation");
                    return;
                }

                Type codeGenType = getMethod.ReturnType;

                MethodInfo setMethod = typeof(PlayerSettings).GetMethod(
                    "SetIl2CppCodeGeneration",
                    new[]
                    {
                        nbtType,
                        codeGenType
                    }
                );

                if (setMethod == null)
                {
                    Debug.LogWarning($"[BuildPresetApplier] SetIl2CppCodeGeneration method not found for {group}");
                    result?.AddNotFound("IL2CPP Code Generation");
                    return;
                }

                setMethod.Invoke(
                    obj: null,
                    new[]
                    {
                        namedTarget,
                        Enum.Parse(codeGenType, codeGen)
                    }
                );

                result?.AddApplied($"IL2CPP Code Generation ({codeGen})");
            }
            catch (Exception ex)
            {
                Debug.LogWarning(
                    $"[BuildPresetApplier] Failed to set IL2CPP code generation '{codeGen}' for {target}: {ex.Message}"
                );

                result?.AddFailed("IL2CPP Code Generation", ex.Message);
            }
        }

        static void ApplyBurstSettings(BuildTarget target, BuildPreset preset, PresetApplicationReport result = null)
        {
            try
            {
                object settings = GetBurstSettings(target);

                if (settings == null)
                {
                    Debug.LogWarning("[BuildPresetApplier] Burst settings not found for target: " + target);
                    result?.AddNotFound("Burst Settings");
                    return;
                }

                var so = new SerializedObject(settings as Object);
                so.Update();

                SetSerializedBool(so, "EnableBurstCompilation", preset.burst_enable_compilation);
                result?.AddApplied($"Burst Compilation ({preset.burst_enable_compilation})");
                SetSerializedBool(so, "EnableOptimisations", preset.burst_enable_optimizations);
                result?.AddApplied($"Burst Optimizations ({preset.burst_enable_optimizations})");
                SetSerializedBool(so, "EnableDebugInAllBuilds", preset.burst_force_debug_info);
                result?.AddApplied($"Burst Debug Info ({preset.burst_force_debug_info})");
                SetSerializedEnum(so, "DebugDataKind", preset.burst_debug_data_kind);
                SetSerializedEnum(so, "OptimizeFor", preset.burst_optimize_for);
                result?.AddApplied($"Burst Optimize For ({preset.burst_optimize_for})");
                SetSerializedEnum(so, "FloatMode", preset.burst_float_mode);
                result?.AddApplied($"Burst Float Mode ({preset.burst_float_mode})");

                if (so.hasModifiedProperties)
                {
                    so.ApplyModifiedProperties();
                    SaveBurstSettings(settings, target);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildPresetApplier] Error applying Burst settings: {ex.Message}\n{ex.StackTrace}");
            }
        }

        static void ApplyBurstDebugDataKind(BuildTarget target, string debugDataKind, PresetApplicationReport result = null)
        {
            try
            {
                object settings = GetBurstSettings(target);

                if (settings == null)
                {
                    Debug.LogWarning("[BuildPresetApplier] Burst settings not found for DebugDataKind");
                    result?.AddNotFound("Burst Debug Data Kind");
                    return;
                }

                string mappedValue = debugDataKind;

                for (int attempt = 0; attempt < 3; attempt++)
                {
                    var so = new SerializedObject(settings as Object);
                    so.Update();

                    SerializedProperty prop = so.FindProperty("DebugDataKind");

                    if (prop?.propertyType == SerializedPropertyType.Enum)
                    {
                        int targetIndex = -1;

                        for (int i = 0; i < prop.enumNames.Length; i++)
                        {
                            if (string.Equals(prop.enumNames[i], mappedValue, StringComparison.OrdinalIgnoreCase))
                            {
                                targetIndex = i;
                                break;
                            }
                        }

                        if (targetIndex < 0)
                        {
                            for (int i = 0; i < prop.enumNames.Length; i++)
                            {
                                string enumName = prop.enumNames[i];

                                if (enumName
                                       .Replace("_", "")
                                       .Equals(mappedValue.Replace("_", ""), StringComparison.OrdinalIgnoreCase)
                                 || enumName.Contains(mappedValue, StringComparison.OrdinalIgnoreCase)
                                 || mappedValue.Contains(enumName, StringComparison.OrdinalIgnoreCase))
                                {
                                    targetIndex = i;
                                    break;
                                }
                            }
                        }

                        if (targetIndex >= 0
                         && prop.enumValueIndex != targetIndex)
                        {
                            prop.enumValueIndex = targetIndex;
                        }
                        else if (targetIndex >= 0
                         && prop.enumValueIndex == targetIndex)
                        {
                            result?.AddApplied($"Burst Debug Data Kind ({debugDataKind})");
                            return;
                        }
                        else
                        {
                            Debug.LogWarning(
                                $"[BuildPresetApplier] DebugDataKind value '{mappedValue}' (from '{debugDataKind}') not found. Available: {string.Join(", ", prop.enumNames)}"
                            );

                            break;
                        }
                    }

                    if (so.hasModifiedProperties)
                    {
                        so.ApplyModifiedProperties();
                        SaveBurstSettings(settings, target);
                        AssetDatabase.SaveAssets();

                        so.Update();
                        SerializedProperty verifyProp = so.FindProperty("DebugDataKind");

                        if (verifyProp?.propertyType == SerializedPropertyType.Enum)
                        {
                            int currentIndex = verifyProp.enumValueIndex;

                            int expectedIndex = -1;

                            for (int i = 0; i < verifyProp.enumNames.Length; i++)
                            {
                                if (string.Equals(verifyProp.enumNames[i], mappedValue, StringComparison.OrdinalIgnoreCase))
                                {
                                    expectedIndex = i;
                                    break;
                                }
                            }

                            if (currentIndex == expectedIndex)
                            {
                                result?.AddApplied($"Burst Debug Data Kind ({debugDataKind})");
                                return;
                            }

                            if (attempt == 2)
                            {
                                Debug.LogWarning(
                                    $"[BuildPresetApplier] DebugDataKind not applied correctly after 3 attempts. Current: {verifyProp.enumNames[currentIndex]}, Expected: {mappedValue}"
                                );

                                result?.AddFailed(
                                    "Burst Debug Data Kind",
                                    $"Expected {mappedValue}, got {verifyProp.enumNames[currentIndex]}"
                                );
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildPresetApplier] Error applying Burst DebugDataKind: {ex.Message}\n{ex.StackTrace}");
                result?.AddFailed("Burst Debug Data Kind", ex.Message);
            }
        }
#endregion

#region Private Helpers
        static SerializedProperty FindPropertyInProjectSettings(SerializedObject so, string[] propertyNames)
        {
            SerializedProperty prop = null;

            foreach (string propName in propertyNames)
            {
                SerializedProperty iterator = so.GetIterator();

                if (iterator.Next(true))
                {
                    do
                    {
                        if (iterator.name == propName)
                        {
                            prop = iterator.Copy();
                            break;
                        }
                    }
                    while (iterator.Next(false));
                }

                if (prop == null)
                {
                    prop = so.FindProperty(propName);
                }

                if (prop != null)
                {
                    break;
                }
            }

            return prop;
        }

        static PlatformPresets GetPlatformPresets(BuildPresetsRoot root, SupportedPlatform platform)
        {
            return platform switch
            {
                SupportedPlatform.WebGL => root.platforms.WebGL,
                SupportedPlatform.Windows => root.platforms.Windows,
                SupportedPlatform.macOS => root.platforms.macOS,
                SupportedPlatform.Android => root.platforms.Android,
                SupportedPlatform.iOS => root.platforms.iOS,
                _ => null
            };
        }

        static PlatformPresets GetOrCreatePlatformPresets(BuildPresetsRoot root, SupportedPlatform platform)
        {
            root.platforms ??= new PlatformsContainer();

            return platform switch
            {
                SupportedPlatform.WebGL => root.platforms.WebGL ??= new PlatformPresets(),
                SupportedPlatform.Windows => root.platforms.Windows ??= new PlatformPresets(),
                SupportedPlatform.macOS => root.platforms.macOS ??= new PlatformPresets(),
                SupportedPlatform.Android => root.platforms.Android ??= new PlatformPresets(),
                SupportedPlatform.iOS => root.platforms.iOS ??= new PlatformPresets(),
                _ => throw new ArgumentException($"Unknown platform: {platform}")
            };
        }

        static object GetNamedBuildTarget(BuildTargetGroup group)
        {
            var nbtType = Type.GetType("UnityEditor.Build.NamedBuildTarget, UnityEditor");

            if (nbtType == null)
            {
                return null;
            }

            string fieldName = group switch
            {
                BuildTargetGroup.WebGL => "WebGL",
                BuildTargetGroup.Standalone => "Standalone",
                BuildTargetGroup.Android => "Android",
                BuildTargetGroup.iOS => "iOS",
                _ => null
            };

            return fieldName == null
                ? null
                : nbtType.GetField(fieldName, PublicStatic)?.GetValue(null);
        }

        static object GetBurstSettings(BuildTarget target)
        {
            try
            {
                Assembly assembly = AppDomain
                   .CurrentDomain
                   .GetAssemblies()
                   .FirstOrDefault(a => a.GetName().Name == "Unity.Burst.Editor");

                MethodInfo method = assembly
                  ?.GetType("Unity.Burst.Editor.BurstPlatformAotSettings")
                  ?.GetMethod("GetOrCreateSettings", PublicStatic | BindingFlags.NonPublic);

                if (method == null)
                {
                    return null;
                }

                ParameterInfo[] parameters = method.GetParameters();

                return method.Invoke(
                    obj: null,
                    parameters.Length == 1 && parameters[0].ParameterType == typeof(BuildTarget?)
                        ? new object[] { (BuildTarget?)target }
                        : new object[] { target }
                );
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BuildPresetApplier] Failed to get Burst settings for {target}: {ex.Message}");
                return null;
            }
        }

        static void SaveBurstSettings(object settings, BuildTarget target)
        {
            MethodInfo saveMethod = settings.GetType().GetMethod("Save", AnyInstance);

            if (saveMethod == null)
            {
                return;
            }

            ParameterInfo[] parameters = saveMethod.GetParameters();

            if (parameters.Length == 1
             && parameters[0].ParameterType == typeof(BuildTarget?))
            {
                saveMethod.Invoke(settings, new object[] { (BuildTarget?)target });
            }
            else if (parameters.Length == 1
             && parameters[0].ParameterType == typeof(BuildTarget))
            {
                saveMethod.Invoke(settings, new object[] { target });
            }
            else
            {
                saveMethod.Invoke(settings, parameters: null);
            }
        }

        static void SetProperty(Type type, string name, object value)
        {
            try
            {
                PropertyInfo prop = type?.GetProperty(name);

                if (prop == null)
                {
                    return;
                }

                for (int attempt = 0; attempt < 2; attempt++)
                {
                    prop.SetValue(obj: null, value);
                    object currentValue = prop.GetValue(null);

                    if (currentValue != null
                     && currentValue.Equals(value))
                    {
                        break;
                    }

                    if (attempt == 1)
                    {
                        Debug.LogWarning(
                            $"[BuildPresetApplier] {type?.Name}.{name} not applied correctly after 2 attempts. Current: {currentValue}, Expected: {value}"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildPresetApplier] Error setting property {type?.Name}.{name} to {value}: {ex.Message}");
            }
        }

        static void SetEnumProperty(Type type, string name, string value)
        {
            try
            {
                PropertyInfo prop = type?.GetProperty(name);

                if (prop == null)
                {
                    return;
                }

                Type enumType = prop.PropertyType;
                object enumValue;

                if (Enum.TryParse(enumType, value, ignoreCase: true, out enumValue))
                {
                    prop.SetValue(obj: null, enumValue);
                }
                else
                {
                    string[] enumNames = Enum.GetNames(enumType);

                    string matchingName = enumNames.FirstOrDefault(n =>
                        string.Equals(n, value, StringComparison.OrdinalIgnoreCase)
                     || n.Replace("_", "").Equals(value.Replace("_", ""), StringComparison.OrdinalIgnoreCase)
                    );

                    if (matchingName != null)
                    {
                        enumValue = Enum.Parse(enumType, matchingName);
                        prop.SetValue(obj: null, enumValue);
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"[BuildPresetApplier] Failed to parse enum value '{value}' for {type?.Name}.{name}. Available values: {string.Join(", ", enumNames)}"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildPresetApplier] Error setting enum property {type?.Name}.{name} to {value}: {ex.Message}");
            }
        }

        static void SetSerializedBool(SerializedObject so, string name, bool value)
        {
            SerializedProperty prop = so.FindProperty(name);

            if (prop?.propertyType == SerializedPropertyType.Boolean)
            {
                prop.boolValue = value;
            }
        }

        static void SetSerializedEnum(SerializedObject so, string name, string value)
        {
            try
            {
                SerializedProperty prop = so.FindProperty(name);

                if (prop?.propertyType != SerializedPropertyType.Enum)
                {
                    Debug.LogWarning($"[BuildPresetApplier] Property {name} is not an enum type");
                    return;
                }

                for (int i = 0; i < prop.enumNames.Length; i++)
                {
                    if (string.Equals(prop.enumNames[i], value, StringComparison.OrdinalIgnoreCase))
                    {
                        prop.enumValueIndex = i;
                        return;
                    }
                }

                int matchingIndex = -1;

                for (int i = 0; i < prop.enumNames.Length; i++)
                {
                    string enumName = prop.enumNames[i];

                    if (enumName.Replace("_", "").Equals(value.Replace("_", ""), StringComparison.OrdinalIgnoreCase)
                     || enumName.Contains(value, StringComparison.OrdinalIgnoreCase)
                     || value.Contains(enumName, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingIndex = i;
                        break;
                    }
                }

                if (matchingIndex >= 0)
                {
                    prop.enumValueIndex = matchingIndex;
                    return;
                }

                Debug.LogWarning(
                    $"[BuildPresetApplier] Failed to find enum value '{value}' for {name}. Available values: {string.Join(", ", prop.enumNames)}"
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildPresetApplier] Error setting serialized enum {name} to {value}: {ex.Message}");
            }
        }
#endregion
    }
}
