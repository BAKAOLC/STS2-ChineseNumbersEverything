using Godot;
using HarmonyLib;
using STS2ChineseNumbersEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ChineseNumbersEverything.Patches
{
    public sealed class GodotTextSetterPatch : IPatchMethod
    {
        public static string PatchId => "godot_text_properties_to_chinese_numbers";
        public static bool IsCritical => false;
        public static string Description => "Convert numbers in generic Godot control text";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new ModPatchTarget(typeof(Button), nameof(Button.Text), null, true, MethodType.Setter),
                new ModPatchTarget(typeof(Label), nameof(Label.Text), null, true, MethodType.Setter),
                new ModPatchTarget(typeof(RichTextLabel), nameof(RichTextLabel.Text), null, true, MethodType.Setter)
            ];
        }

        [HarmonyBefore(Const.PinyinEverythingPatcherId, Const.ExclaimEverythingPatcherId)]
        public static void Prefix(ref string value)
        {
            value = ChineseNumberTextTransformer.Transform(value);
        }
    }
}
