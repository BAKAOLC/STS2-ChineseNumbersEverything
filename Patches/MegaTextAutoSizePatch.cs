using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using STS2ChineseNumbersEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ChineseNumbersEverything.Patches
{
    public sealed class MegaTextAutoSizePatch : IPatchMethod
    {
        public static string PatchId => "mega_text_auto_size_to_chinese_numbers";
        public static string Description => "Convert numbers in dynamically assigned MegaText content";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new ModPatchTarget(typeof(MegaLabel), nameof(MegaLabel.SetTextAutoSize), [typeof(string)]),
                new ModPatchTarget(typeof(MegaRichTextLabel), nameof(MegaRichTextLabel.SetTextAutoSize),
                    [typeof(string)])
            ];
        }

        [HarmonyBefore(Const.PinyinEverythingPatcherId, Const.ExclaimEverythingPatcherId)]
        public static void Prefix(ref string text)
        {
            text = ChineseNumberTextTransformer.Transform(text);
        }
    }
}
