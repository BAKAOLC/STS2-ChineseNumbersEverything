using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using STS2ChineseNumbersEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ChineseNumbersEverything.Patches
{
    public sealed class LocalizedFormattedTextPatch : IPatchMethod
    {
        public static string PatchId => "localized_formatted_text_to_chinese_numbers";
        public static string Description => "Convert dynamic values after localized string formatting";

        public static ModPatchTarget[] GetTargets()
        {
            return [new ModPatchTarget(typeof(LocString), nameof(LocString.GetFormattedText), Type.EmptyTypes)];
        }

        [HarmonyBefore(Const.PinyinEverythingPatcherId, Const.ExclaimEverythingPatcherId)]
        public static void Postfix(ref string __result)
        {
            __result = ChineseNumberTextTransformer.Transform(__result);
        }
    }
}
