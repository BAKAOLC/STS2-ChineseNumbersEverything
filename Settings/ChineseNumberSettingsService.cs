using STS2ChineseNumbersEverything.Data;

namespace STS2ChineseNumbersEverything.Settings
{
    internal static class ChineseNumberSettingsService
    {
        public static bool Enabled
        {
            get
            {
                try
                {
                    return ModDataStore.GetSettings().Enabled;
                }
                catch
                {
                    return true;
                }
            }
        }
    }
}
