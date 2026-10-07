using System;
using UnityEditor;
using UnityEngine;

namespace BuildPresetTool
{
    public class BuildPresetWindow : EditorWindow
    {
        const float LabelWidth = 220f;

        SupportedPlatform platform = SupportedPlatform.WebGL;
        PresetType preset = PresetType.Development;

        Vector2 scroll;

        string selectedCustomPresetName = "";
        string saveAsCustomName = "";

        BuildPreset GetCurrentPreset()
        {
            if (preset == PresetType.Custom)
            {
                return BuildPresetApplier.GetCustomPreset(platform, selectedCustomPresetName);
            }

            return BuildPresetApplier.GetPreset(platform, preset);
        }

#region Menu Items
        [MenuItem("Tools/Build Presets/Settings Window", priority = 0)]
        public static void ShowWindow()
        {
            ShowWindow(platform: null, preset: null);
        }

        static void ShowWindow(SupportedPlatform? platform, PresetType? preset)
        {
            var window = GetWindow<BuildPresetWindow>("Build Presets");
            window.minSize = new Vector2(x: 500, y: 400);

            var savedPlatform = SupportedPlatform.WebGL;
            var savedPresetType = PresetType.Development;
            string savedCustomName = "";

            bool needSaved = !platform.HasValue || !preset.HasValue;

            bool hasSaved = needSaved
             && BuildPresetApplier.TryLoadLastAppliedPreset(out savedPlatform, out savedPresetType, out savedCustomName);

            window.platform = ResolveInitialPlatform(platform, hasSaved, savedPlatform);

            (window.preset, window.selectedCustomPresetName) = ResolveInitialPresetState(
                preset,
                hasSaved,
                savedPresetType,
                savedCustomName
            );
        }

        static SupportedPlatform ResolveInitialPlatform(SupportedPlatform? platform, bool hasSaved,
            SupportedPlatform savedPlatform)
        {
            if (platform.HasValue)
            {
                return platform.Value;
            }

            if (hasSaved)
            {
                return savedPlatform;
            }

            BuildTarget activeTarget = EditorUserBuildSettings.activeBuildTarget;
            return SettingsReader.GetSupportedPlatformFromBuildTarget(activeTarget);
        }

        static (PresetType preset, string customPresetName) ResolveInitialPresetState(PresetType? preset, bool hasSaved,
            PresetType savedPresetType, string savedCustomName)
        {
            if (preset.HasValue)
            {
                return (preset.Value, "");
            }

            if (hasSaved)
            {
                return (savedPresetType, savedCustomName ?? "");
            }

            return (PresetType.Development, "");
        }

        [MenuItem("Tools/Build Presets/Android/Development", priority = 11)]
        public static void ApplyAndroidDev()
        {
            BuildPresetApplier.ApplyPreset(SupportedPlatform.Android, PresetType.Development);
        }

        [MenuItem("Tools/Build Presets/Android/Production", priority = 12)]
        public static void ApplyAndroidProd()
        {
            BuildPresetApplier.ApplyPreset(SupportedPlatform.Android, PresetType.Production);
        }

        [MenuItem("Tools/Build Presets/Android/Custom", priority = 13)]
        public static void ApplyAndroidCustom()
        {
            ShowWindow(SupportedPlatform.Android, PresetType.Custom);
        }

        [MenuItem("Tools/Build Presets/iOS/Development", priority = 21)]
        public static void ApplyiOSDev()
        {
            BuildPresetApplier.ApplyPreset(SupportedPlatform.iOS, PresetType.Development);
        }

        [MenuItem("Tools/Build Presets/iOS/Production", priority = 22)]
        public static void ApplyiOSProd()
        {
            BuildPresetApplier.ApplyPreset(SupportedPlatform.iOS, PresetType.Production);
        }

        [MenuItem("Tools/Build Presets/iOS/Custom", priority = 23)]
        public static void ApplyiOSCustom()
        {
            ShowWindow(SupportedPlatform.iOS, PresetType.Custom);
        }

        [MenuItem("Tools/Build Presets/macOS/Development", priority = 31)]
        public static void ApplyMacOSDev()
        {
            BuildPresetApplier.ApplyPreset(SupportedPlatform.macOS, PresetType.Development);
        }

        [MenuItem("Tools/Build Presets/macOS/Production", priority = 32)]
        public static void ApplyMacOSProd()
        {
            BuildPresetApplier.ApplyPreset(SupportedPlatform.macOS, PresetType.Production);
        }

        [MenuItem("Tools/Build Presets/macOS/Custom", priority = 33)]
        public static void ApplyMacOSCustom()
        {
            ShowWindow(SupportedPlatform.macOS, PresetType.Custom);
        }

        [MenuItem("Tools/Build Presets/Windows/Development", priority = 41)]
        public static void ApplyWindowsDev()
        {
            BuildPresetApplier.ApplyPreset(SupportedPlatform.Windows, PresetType.Development);
        }

        [MenuItem("Tools/Build Presets/Windows/Production", priority = 42)]
        public static void ApplyWindowsProd()
        {
            BuildPresetApplier.ApplyPreset(SupportedPlatform.Windows, PresetType.Production);
        }

        [MenuItem("Tools/Build Presets/Windows/Custom", priority = 43)]
        public static void ApplyWindowsCustom()
        {
            ShowWindow(SupportedPlatform.Windows, PresetType.Custom);
        }

        [MenuItem("Tools/Build Presets/WebGL/Development", priority = 51)]
        public static void ApplyWebGLDev()
        {
            BuildPresetApplier.ApplyPreset(SupportedPlatform.WebGL, PresetType.Development);
        }

        [MenuItem("Tools/Build Presets/WebGL/Production", priority = 52)]
        public static void ApplyWebGLProd()
        {
            BuildPresetApplier.ApplyPreset(SupportedPlatform.WebGL, PresetType.Production);
        }

        [MenuItem("Tools/Build Presets/WebGL/Custom", priority = 53)]
        public static void ApplyWebGLCustom()
        {
            ShowWindow(SupportedPlatform.WebGL, PresetType.Custom);
        }
#endregion

#region GUI
        void OnGUI()
        {
            DrawHeader();
            DrawValidation();
            DrawButtons();
            DrawComparison();
            DrawHints();
        }

        void DrawHeader()
        {
            EditorGUILayout.LabelField("Build Presets", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            DrawPlatformDropdown();
            DrawPresetDropdown();
            EditorGUILayout.EndHorizontal();

            if (preset == PresetType.Custom)
            {
                string[] names = BuildPresetApplier.GetCustomPresetNames(platform);
                bool hasCustom = names != null && names.Length > 0;

                int index = hasCustom
                    ? Array.IndexOf(names, selectedCustomPresetName)
                    : -1;

                if (index < 0 && hasCustom)
                {
                    index = 0;
                }

                if (index >= 0
                 && index < names.Length)
                {
                    selectedCustomPresetName = names[index];
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Custom preset:", GUILayout.Width(90));

                int newIndex = EditorGUILayout.Popup(
                    index,
                    hasCustom
                        ? names
                        : new[] { "(no custom presets)" }
                );

                if (hasCustom
                 && newIndex >= 0
                 && newIndex < names.Length)
                {
                    selectedCustomPresetName = names[newIndex];
                }

                if (hasCustom && !string.IsNullOrEmpty(selectedCustomPresetName))
                {
                    if (GUILayout.Button("Delete", GUILayout.Width(55), GUILayout.Height(18)))
                    {
                        if (EditorUtility.DisplayDialog(
                            "Delete Preset",
                            $"Delete preset \"{selectedCustomPresetName}\"?",
                            "Delete",
                            "Cancel"
                        ))
                        {
                            BuildPresetApplier.DeleteCustomPreset(platform, selectedCustomPresetName);
                            selectedCustomPresetName = "";
                            Repaint();
                        }
                    }
                }

                string deleteAllTooltip =
                    $"Delete all custom presets for {platform}. Development and Production presets are not affected.";

                EditorGUI.BeginDisabledGroup(!hasCustom);

                if (GUILayout.Button(
                    new GUIContent($"Delete all custom ({platform})", deleteAllTooltip),
                    GUILayout.Width(180),
                    GUILayout.Height(18)
                ))
                {
                    int count = names?.Length ?? 0;

                    if (EditorUtility.DisplayDialog(
                        "Delete All Custom Presets",
                        $"Delete all {count} custom presets for {platform}? This cannot be undone.",
                        "Delete",
                        "Cancel"
                    ))
                    {
                        BuildPresetApplier.DeleteAllCustomPresets(platform);
                        selectedCustomPresetName = "";
                        Repaint();
                    }
                }

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }
        }

        void DrawPlatformDropdown()
        {
            EditorGUILayout.LabelField("Platform:", GUILayout.Width(60));
            platform = (SupportedPlatform)EditorGUILayout.EnumPopup(platform, GUILayout.Width(100));
        }

        void DrawPresetDropdown()
        {
            EditorGUILayout.LabelField("Preset:", GUILayout.Width(50));
            preset = (PresetType)EditorGUILayout.EnumPopup(preset);
        }

        void DrawValidation()
        {
            BuildPreset preset = GetCurrentPreset();

            if (preset == null)
            {
                return;
            }

            PresetValidationResult validation = PresetValidator.ValidatePreset(preset, platform);

            if (!validation.IsValid
             || validation.Warnings.Count > 0)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                if (!validation.IsValid)
                {
                    EditorGUILayout.LabelField("Validation Errors:", EditorStyles.boldLabel);

                    foreach (string error in validation.Errors)
                    {
                        EditorGUILayout.HelpBox($"❌ {error}", MessageType.Error);
                    }
                }

                if (validation.Warnings.Count > 0)
                {
                    if (!validation.IsValid)
                    {
                        EditorGUILayout.Space(5);
                    }

                    EditorGUILayout.LabelField("Validation Warnings:", EditorStyles.boldLabel);

                    foreach (string warning in validation.Warnings)
                    {
                        EditorGUILayout.HelpBox($"⚠️ {warning}", MessageType.Warning);
                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
        }

        void DrawButtons()
        {
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            DrawSaveAsCustomButton();
            DrawApplyPresetButton();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preset name:", GUILayout.Width(80));
            saveAsCustomName = EditorGUILayout.TextField(saveAsCustomName);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();
            DrawOpenJsonButton();
            DrawRefreshButton();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
        }

        void DrawApplyPresetButton()
        {
            bool canApply = preset != PresetType.Custom || !string.IsNullOrEmpty(selectedCustomPresetName);
            EditorGUI.BeginDisabledGroup(!canApply);

            if (GUILayout.Button("Apply Preset", GUILayout.Height(25)))
            {
                if (preset == PresetType.Custom)
                {
                    BuildPresetApplier.ApplyPreset(platform, selectedCustomPresetName, UpdateProgress);
                }
                else
                {
                    BuildPresetApplier.ApplyPreset(platform, preset, UpdateProgress);
                }

                Repaint();
            }

            EditorGUI.EndDisabledGroup();
        }

        void UpdateProgress(string message, float progress)
        {
            if (progress < 0)
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            EditorUtility.DisplayProgressBar("Applying Preset", message, progress);
        }

        void DrawSaveAsCustomButton()
        {
            if (GUILayout.Button("Save as Custom", GUILayout.Height(25)))
            {
                string name = (saveAsCustomName ?? "").Trim();

                if (string.IsNullOrEmpty(name))
                {
                    EditorUtility.DisplayDialog("Save as Custom", "Please enter a preset name in the field below.", "OK");
                    return;
                }

                string[] existing = BuildPresetApplier.GetCustomPresetNames(platform);
                bool duplicate = false;

                foreach (string n in existing)
                {
                    if (string.Equals(n, name, StringComparison.OrdinalIgnoreCase))
                    {
                        duplicate = true;
                        break;
                    }
                }

                if (duplicate
                 && !EditorUtility.DisplayDialog(
                        "Save as Custom",
                        $"A preset named \"{name}\" already exists for {platform}. Overwrite?",
                        "Overwrite",
                        "Cancel"
                    ))
                {
                    return;
                }

                BuildPresetApplier.SaveCurrentAsCustom(platform, name);
                saveAsCustomName = "";
                Repaint();
            }
        }

        void DrawOpenJsonButton()
        {
            if (GUILayout.Button("Open JSON", GUILayout.Width(100), GUILayout.Height(25)))
            {
                string assetPath = BuildPresetApplier.GetBuildPresetsJsonAssetPathForEditor();

                TextAsset asset = !string.IsNullOrEmpty(assetPath)
                    ? AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath)
                    : null;

                if (asset != null)
                {
                    AssetDatabase.OpenAsset(asset);
                }
            }
        }

        void DrawRefreshButton()
        {
            if (GUILayout.Button("Refresh", GUILayout.Width(80), GUILayout.Height(25)))
            {
                Repaint();
            }
        }

        void DrawComparison()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);

            BuildPreset preset = GetCurrentPreset();

            if (preset == null)
            {
                string msg = this.preset == PresetType.Custom
                    ? "Select a custom preset or save current settings as custom."
                    : $"Preset {platform}/{this.preset} not found.";

                EditorGUILayout.HelpBox(msg, MessageType.Warning);
                EditorGUILayout.EndScrollView();
                return;
            }

            BuildTarget target = SettingsReader.GetBuildTarget(platform);
            BuildTargetGroup group = SettingsReader.GetBuildTargetGroup(platform);

            DrawPlayerSettings(platform, target, group, preset);

            if (platform == SupportedPlatform.WebGL)
            {
                DrawWebGLSettings(preset);
            }

            if (platform == SupportedPlatform.Android
             || platform == SupportedPlatform.iOS)
            {
                DrawMobileSettings(preset);
            }

            if (platform != SupportedPlatform.WebGL)
            {
                DrawBuildSettings(preset);
            }

            DrawBurstSettings(target, preset);

            EditorGUILayout.EndScrollView();
        }

        void DrawPlayerSettings(SupportedPlatform platform, BuildTarget target, BuildTargetGroup group, BuildPreset preset)
        {
            Section(
                "Player Settings",
                () =>
                {
                    Row(
                        "Static Batching",
                        SettingsReader.GetStaticBatching(target).ToString(),
                        preset.static_batching.ToString()
                    );

                    Row("GPU Skinning", SettingsReader.GetGPUSkinning(), preset.gpu_skinning);
                    Row("Graphics Jobs", PlayerSettings.graphicsJobs.ToString(), preset.graphics_jobs.ToString());
                    Row("Incremental GC", PlayerSettings.gcIncremental.ToString(), preset.incremental_gc.ToString());
                    Row("Managed Stripping", SettingsReader.GetManagedStrippingLevel(group), preset.managed_stripping_level);
                    Row("IL2CPP Code Gen", SettingsReader.GetIL2CPPCodeGeneration(target), preset.il2cpp_code_generation);

                    if (platform != SupportedPlatform.Windows
                     && platform != SupportedPlatform.macOS)
                    {
                        Row("Texture Compression", SettingsReader.GetTextureCompression(target), preset.texture_compression);
                    }

                    Row("Strip Engine Code", PlayerSettings.stripEngineCode.ToString(), preset.strip_engine_code.ToString());
                }
            );
        }

        void DrawWebGLSettings(BuildPreset preset)
        {
            Section(
                "WebGL Settings",
                () =>
                {
                    Row("Compression", WebGL("compressionFormat"), preset.compression);
                    Row("Decompression Fallback", WebGLBool("decompressionFallback"), preset.decompression_fallback.ToString());
                    Row("Enable Exceptions", WebGL("exceptionSupport"), preset.enable_exceptions);
                    Row("Data Caching", WebGLBool("dataCaching"), preset.data_caching.ToString());
                    Row("Name Files as Hashes", WebGLBool("nameFilesAsHashes"), preset.name_files_as_hashes.ToString());
                    Row("Show Diagnostics", WebGLBool("showDiagnostics"), preset.show_diagnostics.ToString());
                    Row("Debug Symbols", WebGLBool("debugSymbols"), preset.debug_symbols.ToString());
                    Row("Enable Native C/C++ Multithreading", WebGLBool("threadsSupport"), preset.threads_support.ToString());
                    Row("Power Preference", WebGL("powerPreference"), preset.power_preference);

                    RowWithNote(
                        "WebAssembly 2023",
                        SettingsReader.GetWebAssembly2023().ToString(),
                        preset.webassembly_2023_feature_set.ToString()
                    );
                }
            );

            Section(
                "Memory Settings",
                () =>
                {
                    Row("Initial Memory Size", WebGL("initialMemorySize"), preset.initial_memory_size.ToString());
                    Row("Maximum Memory Size", WebGL("maximumMemorySize"), preset.maximum_memory_size.ToString());
                    Row("Memory Growth Mode", WebGL("memoryGrowthMode"), preset.memory_growth_mode);
                }
            );

            Section(
                "WebGL Build Settings",
                () =>
                {
                    string codeOpt = EditorUserBuildSettings.GetPlatformSettings("WebGL", "CodeOptimization");
                    codeOpt = BuildPresetApplier.MapCodeOptimizationFromUnity(codeOpt);

                    Row("Code Optimization", codeOpt, preset.code_optimization);
                    Row("Build Profile Texture", SettingsReader.GetWebGLBuildSubtarget(), "Use Player Settings");
                }
            );

            Section(
                "Build Settings",
                () =>
                {
                    Row(
                        "Development Build",
                        EditorUserBuildSettings.development.ToString(),
                        (this.preset == PresetType.Development).ToString()
                    );

                    Row("Script Debugging", SettingsReader.GetScriptDebugging().ToString(), preset.script_debugging.ToString());

                    Row(
                        "Deep Profiling Support",
                        SettingsReader.GetDeepProfilingSupport().ToString(),
                        preset.deep_profiling_support.ToString()
                    );

                    Row(
                        "Scripts Only Build",
                        SettingsReader.GetScriptsOnlyBuild().ToString(),
                        preset.scripts_only_build.ToString()
                    );
                }
            );
        }

        void DrawMobileSettings(BuildPreset preset)
        {
            Section(
                "Mobile Settings",
                () =>
                {
                    Row("Accelerometer Frequency", SettingsReader.GetAccelerometerFrequency(), preset.accelerometer_frequency);
                }
            );
        }

        void DrawBuildSettings(BuildPreset preset)
        {
            Section(
                "Build Settings",
                () =>
                {
                    string platformName = platform switch
                    {
                        SupportedPlatform.Android => "Android",
                        SupportedPlatform.iOS => "iOS",
                        SupportedPlatform.Windows => "Standalone",
                        SupportedPlatform.macOS => "Standalone",
                        _ => ""
                    };

                    if (!string.IsNullOrEmpty(platformName))
                    {
                        string codeOpt = EditorUserBuildSettings.GetPlatformSettings(platformName, "CodeOptimization");

                        if (!string.IsNullOrEmpty(codeOpt))
                        {
                            codeOpt = BuildPresetApplier.MapCodeOptimizationFromUnity(codeOpt);
                            Row("Code Optimization", codeOpt, preset.code_optimization);
                        }
                    }

                    Row(
                        "Development Build",
                        EditorUserBuildSettings.development.ToString(),
                        (this.preset == PresetType.Development).ToString()
                    );

                    Row("Script Debugging", SettingsReader.GetScriptDebugging().ToString(), preset.script_debugging.ToString());

                    Row(
                        "Deep Profiling Support",
                        SettingsReader.GetDeepProfilingSupport().ToString(),
                        preset.deep_profiling_support.ToString()
                    );

                    Row(
                        "Scripts Only Build",
                        SettingsReader.GetScriptsOnlyBuild().ToString(),
                        preset.scripts_only_build.ToString()
                    );
                }
            );
        }

        void DrawBurstSettings(BuildTarget target, BuildPreset preset)
        {
            Section(
                "Burst AOT Settings",
                () =>
                {
                    Row(
                        "Enable Compilation",
                        SettingsReader.GetBurstSetting(target, "EnableBurstCompilation", defaultValue: true).ToString(),
                        preset.burst_enable_compilation.ToString()
                    );

                    Row(
                        "Enable Optimizations",
                        SettingsReader.GetBurstSetting(target, "EnableOptimisations", defaultValue: true).ToString(),
                        preset.burst_enable_optimizations.ToString()
                    );

                    Row(
                        "Force Debug Info",
                        SettingsReader.GetBurstSetting(target, "EnableDebugInAllBuilds", defaultValue: false).ToString(),
                        preset.burst_force_debug_info.ToString()
                    );

                    Row(
                        "Debug Data Kind",
                        SettingsReader.GetBurstSetting<object>(target, "DebugDataKind", defaultValue: null)?.ToString() ?? "None",
                        preset.burst_debug_data_kind
                    );

                    Row(
                        "Optimize For",
                        SettingsReader.GetBurstSetting<object>(target, "OptimizeFor", defaultValue: null)?.ToString()
                     ?? "Default",
                        preset.burst_optimize_for
                    );

                    Row(
                        "Float Mode",
                        SettingsReader.GetBurstSetting<object>(target, "FloatMode", defaultValue: null)?.ToString() ?? "Default",
                        preset.burst_float_mode
                    );
                }
            );

            var noteStyle = new GUIStyle(EditorStyles.label);
            noteStyle.wordWrap = true;
            noteStyle.normal.textColor = EditorStyles.helpBox.normal.textColor;

            EditorGUILayout.LabelField(
                "ℹ️ Note: After applying a preset, Burst AOT Settings are applied correctly, but the Project Settings window may not refresh automatically. If you have the Project Settings window open, switch to another tab and back to see the updated values.",
                noteStyle
            );
        }

        void DrawHints()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Hints", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "• Development — for debugging (fast build, stacktrace)\n"
              + "• Production — for release (optimization, minimal size)\n"
              + "• Custom — enter a name in 'Preset name', then click 'Save as Custom'",
                MessageType.Info
            );
        }
#endregion

#region Helpers
        void Section(string title, Action content)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            content();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(5);
        }

        void Row(string label, string current, string expected)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(220));

            string normalizedCurrent = NormalizeValue(label, current);
            string normalizedExpected = NormalizeValue(label, expected);

            bool match = string.Equals(normalizedCurrent, normalizedExpected, StringComparison.OrdinalIgnoreCase);

            var style = new GUIStyle(EditorStyles.label)
            {
                normal =
                {
                    textColor = match
                        ? Color.green
                        : Color.red
                }
            };

            EditorGUILayout.LabelField($"Current: {current}", style, GUILayout.Width(200));
            EditorGUILayout.LabelField($"Expected: {expected}", GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
        }

        void RowWithNote(string label, string current, string expected)
        {
            Row(label, current, expected);

            Rect rect = EditorGUILayout.GetControlRect(hasLabel: false, height: 14);
            rect.x += LabelWidth;
            rect.width -= LabelWidth;

            Texture infoIcon = EditorGUIUtility.IconContent("console.infoicon")?.image;

            if (infoIcon == null)
            {
                infoIcon = EditorGUIUtility.IconContent("_Help")?.image;
            }

            var noteStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = Color.gray },
                fontSize = 9
            };

            string tooltipText =
                "Unity does not expose a stable public API for this setting.\nBuild Preset Manager applies it directly via ProjectSettings.asset.";

            if (infoIcon != null)
            {
                var iconRect = new Rect(rect.x, rect.y, width: 14, height: 14);
                var iconContent = new GUIContent(infoIcon, tooltipText);
                GUI.Label(iconRect, iconContent);
                rect.x += 18;
                rect.width -= 18;
            }

            var noteContent = new GUIContent("Applied via ProjectSettings.asset (Unity limitation)", tooltipText);
            GUI.Label(rect, noteContent, noteStyle);
        }

        string NormalizeValue(string label, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (label == "Enable Exceptions")
            {
                return value switch
                {
                    "ExplicitlyThrown" => "ExplicitlyThrownExceptionsOnly",
                    "ExplicitlyThrownExceptionsOnly" => "ExplicitlyThrownExceptionsOnly",
                    _ => value
                };
            }

            return value;
        }

        static string WebGL(string propertyName)
        {
            return SettingsReader.GetWebGLProperty(propertyName) ?? "N/A";
        }

        static string WebGLBool(string propertyName)
        {
            return SettingsReader.GetWebGLBool(propertyName).ToString();
        }
#endregion
    }
}
