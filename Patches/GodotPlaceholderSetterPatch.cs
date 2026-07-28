using Godot;
using HarmonyLib;
using STS2ChineseNumbersEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ChineseNumbersEverything.Patches
{
    public sealed class GodotPlaceholderSetterPatch : IPatchMethod
    {
        public static string PatchId => "godot_placeholder_properties_to_chinese_numbers";
        public static bool IsCritical => false;
        public static string Description => "Convert displayed input placeholder numbers without changing user input";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new ModPatchTarget(typeof(LineEdit), nameof(LineEdit.PlaceholderText), null, true, MethodType.Setter),
                new ModPatchTarget(typeof(TextEdit), nameof(TextEdit.PlaceholderText), null, true, MethodType.Setter)
            ];
        }

        [HarmonyBefore(Const.PinyinEverythingPatcherId, Const.ExclaimEverythingPatcherId)]
        public static void Prefix(ref string value)
        {
            value = ChineseNumberTextTransformer.Transform(value);
        }
    }
}
