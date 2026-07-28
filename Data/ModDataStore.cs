using STS2ChineseNumbersEverything.Settings;
using STS2RitsuLib;
using STS2RitsuLib.Utils.Persistence;

namespace STS2ChineseNumbersEverything.Data
{
    internal static class ModDataStore
    {
        public const string SettingsKey = "settings";

        private const string SettingsFileName = "settings.json";

        private static readonly STS2RitsuLib.Data.ModDataStore Store =
            STS2RitsuLib.Data.ModDataStore.For(Const.ModId);

        public static void Initialize()
        {
            using (RitsuLibFramework.BeginModDataRegistration(Const.ModId))
            {
                Store.Register(
                    SettingsKey,
                    SettingsFileName,
                    SaveScope.Global,
                    () => new ChineseNumberSettings(),
                    true);
            }
        }

        public static ChineseNumberSettings GetSettings()
        {
            return Store.Get<ChineseNumberSettings>(SettingsKey);
        }
    }
}
