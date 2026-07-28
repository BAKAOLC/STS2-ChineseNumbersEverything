using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using STS2ChineseNumbersEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ChineseNumbersEverything.Patches
{
    public sealed class MegaTextReadyPatch : IPatchMethod
    {
        public static string PatchId => "scene_mega_text_to_chinese_numbers";
        public static bool IsCritical => false;
        public static string Description => "Convert numbers in scene-authored MegaText content after initialization";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new ModPatchTarget(typeof(MegaLabel), nameof(MegaLabel._Ready), Type.EmptyTypes),
                new ModPatchTarget(typeof(MegaRichTextLabel), nameof(MegaRichTextLabel._Ready), Type.EmptyTypes)
            ];
        }

        [HarmonyBefore(Const.PinyinEverythingPatcherId, Const.ExclaimEverythingPatcherId)]
        public static void Postfix(object __instance)
        {
            switch (__instance)
            {
                case MegaLabel label:
                    label.SetTextAutoSize(ChineseNumberTextTransformer.Transform(label.Text));
                    break;
                case MegaRichTextLabel richTextLabel:
                    richTextLabel.SetTextAutoSize(ChineseNumberTextTransformer.Transform(richTextLabel.Text));
                    break;
            }
        }
    }
}
