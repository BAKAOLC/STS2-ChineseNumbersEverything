using STS2ChineseNumbersEverything.Data;

namespace STS2ChineseNumbersEverything.Settings
{
    internal static class ChineseNumberSettingsService
    {
        public static bool Enabled => Read(static settings => settings.Enabled, true);

        public static NumberDisplayStyle Style =>
            Read(static settings => settings.Style, NumberDisplayStyle.Chinese);

        private static TValue Read<TValue>(Func<ChineseNumberSettings, TValue> selector, TValue fallback)
        {
            try
            {
                return selector(ModDataStore.GetSettings());
            }
            catch
            {
                return fallback;
            }
        }
    }
}
