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

                var styleBinding = ModSettingsBindings.WithDefault(
                    ModSettingsBindings.Global<ChineseNumberSettings, NumberDisplayStyle>(
                        Const.ModId,
                        ModDataStore.SettingsKey,
                        settings => settings.Style,
                        (settings, value) => settings.Style = value),
                    () => NumberDisplayStyle.Chinese);

                RitsuLibFramework.RegisterModSettings(Const.ModId, page => page
                    .WithModDisplayName(T("Chinese Numbers Everything", "全都中文数字"))
                    .WithTitle(T("Settings", "设置"))
                    .WithDescription(T(
                        "Convert displayed numbers to standard or uppercase financial Chinese numerals before other text transformation mods.",
                        "在其他文本转换模组之前，将显示的数字转换为普通或财务大写中文数字。"))
                    .AddSection("general", section => section
                        .WithTitle(T("General", "通用"))
                        .AddToggle(
                            "enabled",
                            T("Enable number conversion", "启用数字转换"),
                            enabledBinding,
                            T(
                                "When disabled, newly displayed numbers remain unchanged.",
                                "关闭后，新显示的数字将保持原样。"))
                        .AddEnumChoice(
                            "style",
                            T("Number style", "数字样式"),
                            styleBinding,
                            StyleLabel,
                            T(
                                "Both numeral styles apply only while a Chinese locale is selected.",
                                "两种数字样式都仅在中文语言下生效。"),
                            ModSettingsChoicePresentation.Dropdown)
                        .WithEntryEnabledWhen(
                            "style",
                            () => ChineseNumberSettingsService.Enabled)));

                _initialized = true;
            }
        }

        private static ModSettingsText StyleLabel(NumberDisplayStyle style)
        {
            return style switch
            {
                NumberDisplayStyle.ChineseFinancial =>
                    T("Uppercase financial numerals", "中文大写数字"),
                _ => T("Chinese numerals", "中文数字")
            };
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
