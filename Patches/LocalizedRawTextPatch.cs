using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using STS2ChineseNumbersEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ChineseNumbersEverything.Patches
{
    public sealed class LocalizedRawTextPatch : IPatchMethod
    {
        public static string PatchId => "localized_raw_text_to_chinese_numbers";
        public static string Description => "Convert literal numbers outside localization format expressions";

        public static ModPatchTarget[] GetTargets()
        {
            return [new ModPatchTarget(typeof(LocString), nameof(LocString.GetRawText), Type.EmptyTypes)];
        }

        [HarmonyBefore(Const.PinyinEverythingPatcherId, Const.ExclaimEverythingPatcherId)]
        public static void Postfix(ref string __result)
        {
            __result = ChineseNumberTextTransformer.Transform(__result);
        }
    }
}
