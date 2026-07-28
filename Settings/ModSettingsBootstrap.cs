using MegaCrit.Sts2.Core.Localization;
using STS2ChineseNumbersEverything.Data;
using STS2RitsuLib;
using STS2RitsuLib.Settings;

namespace STS2ChineseNumbersEverything.Settings
{
    internal static class ModSettingsBootstrap
    {
        private static readonly Lock InitLock = new();
        private static bool _initialized;

        public static void Initialize()
        {
            lock (InitLock)
            {
                if (_initialized)
                {
                    return;
                }

                var enabledBinding = ModSettingsBindings.WithDefault(
                    ModSettingsBindings.Global<ChineseNumberSettings, bool>(
                        Const.ModId,
                        ModDataStore.SettingsKey,
                        settings => settings.Enabled,
                        (settings, value) => settings.Enabled = value),
                    () => true);

                RitsuLibFramework.RegisterModSettings(Const.ModId, page => page
                    .WithModDisplayName(T("Chinese Numbers Everything", "全都中文数字"))
                    .WithTitle(T("Settings", "设置"))
                    .WithDescription(T(
                        "Convert displayed numbers to Chinese numerals before other text transformation mods.",
                        "在其他文本转换模组之前，将显示的数字转换为中文数字。"))
                    .AddSection("general", section => section
                        .WithTitle(T("General", "通用"))
                        .AddToggle(
                            "enabled",
                            T("Enable Chinese number conversion", "启用中文数字转换"),
                            enabledBinding,
                            T(
                                "When disabled, newly displayed numbers remain unchanged.",
                                "关闭后，新显示的数字将保持原样。"))));

                _initialized = true;
            }
        }

        private static ModSettingsText T(string english, string simplifiedChinese)
        {
            return ModSettingsText.Dynamic(() => IsSimplifiedChinese() ? simplifiedChinese : english);
        }

        private static bool IsSimplifiedChinese()
        {
            try
            {
                return string.Equals(LocManager.Instance?.Language, "zhs", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
