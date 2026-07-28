using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2ChineseNumbersEverything.Data;
using STS2ChineseNumbersEverything.Patches;
using STS2ChineseNumbersEverything.Settings;
using STS2RitsuLib;
using STS2RitsuLib.Patching.Core;

namespace STS2ChineseNumbersEverything
{
    [ModInitializer(nameof(Initialize))]
    public static class Main
    {
        public static readonly Logger Logger = RitsuLibFramework.CreateLogger(Const.ModId);

        public static bool IsModActive { get; private set; }

        public static void Initialize()
        {
            Logger.Info($"Initializing {Const.ModId} {Const.Version}");

            try
            {
                var patcher = RitsuLibFramework.CreatePatcher(Const.ModId, "display-text");
                RegisterPatches(patcher);

                if (!RitsuLibFramework.ApplyRequiredPatcher(patcher, () => IsModActive = false))
                {
                    Logger.Error("Mod initialization failed: required text patches could not be applied");
                    return;
                }

                IsModActive = true;
                ModDataStore.Initialize();
                ModSettingsBootstrap.Initialize();
                Logger.Info("Mod initialization complete");
            }
            catch (Exception ex)
            {
                IsModActive = false;
                Logger.Error($"Mod initialization failed: {ex}");
            }
        }

        private static void RegisterPatches(ModPatcher patcher)
        {
            patcher.RegisterPatch<LocalizedRawTextPatch>();
            patcher.RegisterPatch<LocalizedFormattedTextPatch>();
            patcher.RegisterPatch<MegaTextAutoSizePatch>();
            patcher.RegisterPatch<MegaTextReadyPatch>();
            patcher.RegisterPatch<IconBadgeTextPatch>();
            patcher.RegisterPatch<GodotTextSetterPatch>();
            patcher.RegisterPatch<GodotPlaceholderSetterPatch>();
            patcher.RegisterPatch<CanvasItemDrawStringPatch>();
        }
    }
}
