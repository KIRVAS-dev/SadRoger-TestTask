using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BuildPresetTool
{
    public static class SettingsReader
    {
        const string ProjectSettingsPath = "ProjectSettings/ProjectSettings.asset";

        static readonly BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
        static readonly BindingFlags AnyInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        public static BuildTarget GetBuildTarget(SupportedPlatform platform)
        {
            return platform switch
            {
                SupportedPlatform.WebGL => BuildTarget.WebGL,
                SupportedPlatform.Windows => BuildTarget.StandaloneWindows64,
                SupportedPlatform.macOS => BuildTarget.StandaloneOSX,
                SupportedPlatform.Android => BuildTarget.Android,
                SupportedPlatform.iOS => BuildTarget.iOS,
                _ => BuildTarget.WebGL
            };
        }

        public static SupportedPlatform GetSupportedPlatformFromBuildTarget(BuildTarget target)
        {
            return target switch
            {
                BuildTarget.WebGL => SupportedPlatform.WebGL,
                BuildTarget.StandaloneWindows64 => SupportedPlatform.Windows,
                BuildTarget.StandaloneOSX => SupportedPlatform.macOS,
                BuildTarget.Android => SupportedPlatform.Android,
                BuildTarget.iOS => SupportedPlatform.iOS,
                _ => SupportedPlatform.WebGL
            };
        }

        public static BuildTargetGroup GetBuildTargetGroup(SupportedPlatform platform)
        {
            return platform switch
            {
                SupportedPlatform.WebGL => BuildTargetGroup.WebGL,
                SupportedPlatform.Windows or SupportedPlatform.macOS => BuildTargetGroup.Standalone,
                SupportedPlatform.Android => BuildTargetGroup.Android,
                SupportedPlatform.iOS => BuildTargetGroup.iOS,
                _ => BuildTargetGroup.WebGL
            };
        }

        public static BuildPreset ReadCurrentSettings(SupportedPlatform platform)
        {
            BuildTarget target = GetBuildTarget(platform);
            BuildTargetGroup group = GetBuildTargetGroup(platform);

            var preset = new BuildPreset
            {
                strip_engine_code = PlayerSettings.stripEngineCode,
                graphics_jobs = PlayerSettings.graphicsJobs,
                incremental_gc = PlayerSettings.gcIncremental,
                gpu_skinning = GetGPUSkinning(),
                static_batching = GetStaticBatching(target),
                texture_compression = GetTextureCompression(target),
                managed_stripping_level = GetManagedStrippingLevel(group),
                il2cpp_code_generation = GetIL2CPPCodeGeneration(target),
                burst_enable_compilation = GetBurstSetting(target, "EnableBurstCompilation", defaultValue: true),
                burst_enable_optimizations = GetBurstSetting(target, "EnableOptimisations", defaultValue: true),
                burst_force_debug_info = GetBurstSetting(target, "EnableDebugInAllBuilds", defaultValue: false),
                burst_debug_data_kind =
                    GetBurstSetting<object>(target, "DebugDataKind", defaultValue: null)?.ToString() ?? "None",
                burst_optimize_for = GetBurstSetting<object>(target, "OptimizeFor", defaultValue: null)?.ToString() ?? "Default",
                burst_float_mode = GetBurstSetting<object>(target, "FloatMode", defaultValue: null)?.ToString() ?? "Default",
                script_debugging = GetScriptDebugging(),
                deep_profiling_support = GetDeepProfilingSupport(),
                scripts_only_build = GetScriptsOnlyBuild()
            };

            if (platform == SupportedPlatform.WebGL)
            {
                ReadWebGLSettings(preset);
            }

            if (platform == SupportedPlatform.Android
             || platform == SupportedPlatform.iOS)
            {
                ReadMobileSettings(preset);
            }

            if (platform != SupportedPlatform.WebGL)
            {
                ReadCodeOptimization(platform, preset);
            }

            return preset;
        }

        static void ReadWebGLSettings(BuildPreset preset)
        {
            Type webgl = typeof(PlayerSettings).GetNestedType("WebGL");

            if (webgl == null)
            {
                return;
            }

            preset.compression = GetWebGLProperty(webgl, "compressionFormat") ?? "Gzip";
            preset.decompression_fallback = GetWebGLBool(webgl, "decompressionFallback");
            preset.enable_exceptions = GetWebGLProperty(webgl, "exceptionSupport") ?? "FullWithStacktrace";
            preset.initial_memory_size = GetWebGLInt(webgl, "initialMemorySize", defaultValue: 32);
            preset.maximum_memory_size = GetWebGLInt(webgl, "maximumMemorySize", defaultValue: 2048);
            preset.memory_growth_mode = GetWebGLProperty(webgl, "memoryGrowthMode") ?? "Geometric";
            preset.data_caching = GetWebGLBool(webgl, "dataCaching");
            preset.name_files_as_hashes = GetWebGLBool(webgl, "nameFilesAsHashes");
            preset.show_diagnostics = GetWebGLBool(webgl, "showDiagnostics");
            preset.debug_symbols = GetWebGLBool(webgl, "debugSymbols");
            preset.threads_support = GetWebGLBool(webgl, "threadsSupport");
            preset.power_preference = GetWebGLProperty(webgl, "powerPreference") ?? "Default";
            preset.webassembly_2023_feature_set = ReadWebAssembly2023FromProjectSettings();

            string codeOpt = EditorUserBuildSettings.GetPlatformSettings("WebGL", "CodeOptimization");
            preset.code_optimization = BuildPresetApplier.MapCodeOptimizationFromUnity(codeOpt);
        }

        static void ReadMobileSettings(BuildPreset preset)
        {
            try
            {
                MethodInfo method = typeof(PlayerSettings).GetMethod("GetAccelerometerFrequency", PublicStatic);

                if (method != null)
                {
                    object frequency = method.Invoke(obj: null, parameters: null);
                    preset.accelerometer_frequency = frequency?.ToString() ?? "Disabled";
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read Accelerometer Frequency: {ex.Message}");
                preset.accelerometer_frequency = "Disabled";
            }
        }

        static bool ReadWebAssembly2023FromProjectSettings()
        {
#if !UNITY_6000_0_OR_NEWER
            return false;
#endif
            try
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
                            return prop.intValue == 1;
                        }

                        if (prop.propertyType == SerializedPropertyType.Boolean)
                        {
                            return prop.boolValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read WebAssembly 2023 setting: {ex.Message}");
            }

            try
            {
                Type playerSettingsType = typeof(PlayerSettings);
                FieldInfo field = playerSettingsType.GetField("webWasm2023", AnyInstance | BindingFlags.Static);

                if (field != null)
                {
                    object value = field.GetValue(null);

                    if (value is int intValue)
                    {
                        return intValue == 1;
                    }

                    if (value is bool boolValue)
                    {
                        return boolValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Reflection fallback for reading webWasm2023 also failed: {ex.Message}");
            }

            return false;
        }

        static void ReadCodeOptimization(SupportedPlatform platform, BuildPreset preset)
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

                if (string.IsNullOrEmpty(platformName))
                {
                    return;
                }

                string codeOpt = EditorUserBuildSettings.GetPlatformSettings(platformName, "CodeOptimization");

                if (!string.IsNullOrEmpty(codeOpt))
                {
                    preset.code_optimization = BuildPresetApplier.MapCodeOptimizationFromUnity(codeOpt);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read Code Optimization for {platform}: {ex.Message}");
            }
        }

        public static string GetGPUSkinning()
        {
            try
            {
                PropertyInfo prop = typeof(PlayerSettings).GetProperty("gpuSkinning", PublicStatic);

                if (prop != null)
                {
                    object value = prop.GetValue(null);

                    if (prop.PropertyType == typeof(bool))
                    {
                        return (bool)value
                            ? "GPU"
                            : "CPU";
                    }

                    if (prop.PropertyType == typeof(int))
                    {
                        return (int)value switch
                        {
                            0 => "CPU",
                            1 => "GPU",
                            2 => "GPUBatched",
                            _ => ((int)value).ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read GPU Skinning from PlayerSettings: {ex.Message}");
            }

            try
            {
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(ProjectSettingsPath);

                if (assets != null
                 && assets.Length > 0)
                {
                    var so = new SerializedObject(assets[0]);
                    so.Update();
                    SerializedProperty prop = so.FindProperty("gpuSkinning");

                    if (prop != null
                     && prop.propertyType == SerializedPropertyType.Integer)
                    {
                        return prop.intValue switch
                        {
                            0 => "CPU",
                            1 => "GPU",
                            2 => "GPUBatched",
                            var x => x.ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read GPU Skinning from ProjectSettings.asset: {ex.Message}");
            }

            return "N/A";
        }

        public static bool GetStaticBatching(BuildTarget target)
        {
            try
            {
                object result = typeof(PlayerSettings)
                   .GetMethod("GetStaticBatchingForPlatform", PublicStatic)
                  ?.Invoke(obj: null, new object[] { target });

                return result is true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read Static Batching for {target}: {ex.Message}");
                return true;
            }
        }

        public static string GetTextureCompression(BuildTarget target)
        {
            try
            {
                MethodInfo method = typeof(PlayerSettings).GetMethod(
                    "GetDefaultTextureCompressionFormat",
                    PublicStatic | BindingFlags.NonPublic
                );

                string result = method?.Invoke(obj: null, new object[] { target })?.ToString() ?? "";

                return result == "DXTC"
                    ? "DXT"
                    : result;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read Texture Compression for {target}: {ex.Message}");
                return "N/A";
            }
        }

        public static string GetManagedStrippingLevel(BuildTargetGroup group)
        {
            try
            {
                object namedTarget = GetNamedBuildTarget(group);

                if (namedTarget == null)
                {
                    return "N/A";
                }

                return typeof(PlayerSettings)
                       .GetMethod("GetManagedStrippingLevel", new[] { namedTarget.GetType() })
                      ?.Invoke(obj: null, new[] { namedTarget })
                      ?.ToString()
                 ?? "N/A";
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read Managed Stripping Level for {group}: {ex.Message}");
                return "N/A";
            }
        }

        public static string GetIL2CPPCodeGeneration(BuildTarget target)
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
                    return "N/A";
                }

                object namedTarget = GetNamedBuildTarget(group);

                if (namedTarget == null)
                {
                    return "N/A";
                }

                return typeof(PlayerSettings)
                       .GetMethod("GetIl2CppCodeGeneration", new[] { namedTarget.GetType() })
                      ?.Invoke(obj: null, new[] { namedTarget })
                      ?.ToString()
                 ?? "N/A";
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read IL2CPP Code Generation for {target}: {ex.Message}");
                return "N/A";
            }
        }

        public static T GetBurstSetting<T>(BuildTarget target, string fieldName, T defaultValue)
        {
            try
            {
                object settings = GetBurstSettings(target);

                if (settings == null)
                {
                    return defaultValue;
                }

                return (T)settings.GetType().GetField(fieldName, AnyInstance)?.GetValue(settings) ?? defaultValue;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read Burst setting '{fieldName}' for {target}: {ex.Message}");
                return defaultValue;
            }
        }

        public static string GetWebGLProperty(string propertyName)
        {
            Type webgl = typeof(PlayerSettings).GetNestedType("WebGL");
            return GetWebGLProperty(webgl, propertyName);
        }

        public static bool GetWebGLBool(string propertyName)
        {
            Type webgl = typeof(PlayerSettings).GetNestedType("WebGL");
            return GetWebGLBool(webgl, propertyName);
        }

        public static bool GetWebAssembly2023()
        {
            return ReadWebAssembly2023FromProjectSettings();
        }

        public static bool GetScriptDebugging()
        {
            try
            {
                PropertyInfo prop = typeof(EditorUserBuildSettings).GetProperty("allowDebugging", PublicStatic);
                return prop != null && (bool)(prop.GetValue(null) ?? false);
            }
            catch
            {
                return false;
            }
        }

        public static bool GetDeepProfilingSupport()
        {
            try
            {
                PropertyInfo prop = typeof(EditorUserBuildSettings).GetProperty("buildWithDeepProfilingSupport", PublicStatic);
                return prop != null && (bool)(prop.GetValue(null) ?? false);
            }
            catch
            {
                return false;
            }
        }

        public static bool GetScriptsOnlyBuild()
        {
            try
            {
                PropertyInfo prop = typeof(EditorUserBuildSettings).GetProperty("buildScriptsOnly", PublicStatic);
                return prop != null && (bool)(prop.GetValue(null) ?? false);
            }
            catch
            {
                return false;
            }
        }

        internal static string GetAccelerometerFrequency()
        {
            try
            {
                MethodInfo method = typeof(PlayerSettings).GetMethod("GetAccelerometerFrequency", PublicStatic);

                if (method == null)
                {
                    return "N/A";
                }

                object frequency = method.Invoke(obj: null, parameters: null);
                return frequency?.ToString() ?? "Disabled";
            }
            catch
            {
                return "N/A";
            }
        }

        internal static string GetWebGLBuildSubtarget()
        {
            try
            {
                PropertyInfo prop = typeof(EditorUserBuildSettings).GetProperty("webGLBuildSubtarget", PublicStatic);
                string value = prop?.GetValue(null)?.ToString() ?? "";

                return value == "Generic"
                    ? "Use Player Settings"
                    : value;
            }
            catch
            {
                return "N/A";
            }
        }

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
                Debug.LogWarning($"[SettingsReader] Failed to get Burst settings for {target}: {ex.Message}");
                return null;
            }
        }

        static string GetWebGLProperty(Type webglType, string name)
        {
            try
            {
                return webglType?.GetProperty(name)?.GetValue(null)?.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read WebGL property '{name}': {ex.Message}");
                return null;
            }
        }

        static bool GetWebGLBool(Type webglType, string name)
        {
            try
            {
                return (bool)(webglType?.GetProperty(name)?.GetValue(null) ?? false);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read WebGL bool property '{name}': {ex.Message}");
                return false;
            }
        }

        static int GetWebGLInt(Type webglType, string name, int defaultValue)
        {
            try
            {
                return (int)(webglType?.GetProperty(name)?.GetValue(null) ?? defaultValue);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SettingsReader] Failed to read WebGL int property '{name}': {ex.Message}");
                return defaultValue;
            }
        }
    }
}
