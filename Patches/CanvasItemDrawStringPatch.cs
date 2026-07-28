using Godot;
using HarmonyLib;
using STS2ChineseNumbersEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ChineseNumbersEverything.Patches
{
    public sealed class CanvasItemDrawStringPatch : IPatchMethod
    {
        public static string PatchId => "canvas_draw_string_to_chinese_numbers";
        public static bool IsCritical => false;
        public static string Description => "Convert numbers in custom-drawn CanvasItem strings";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new ModPatchTarget(typeof(CanvasItem), nameof(CanvasItem.DrawString),
                [
                    typeof(Font),
                    typeof(Vector2),
                    typeof(string),
                    typeof(HorizontalAlignment),
                    typeof(float),
                    typeof(int),
                    typeof(Color?),
                    typeof(TextServer.JustificationFlag),
                    typeof(TextServer.Direction),
                    typeof(TextServer.Orientation)
                ]),
                new ModPatchTarget(typeof(CanvasItem), nameof(CanvasItem.DrawString),
                [
                    typeof(Font),
                    typeof(Vector2),
                    typeof(string),
                    typeof(HorizontalAlignment),
                    typeof(float),
                    typeof(int),
                    typeof(Color?),
                    typeof(TextServer.JustificationFlag),
                    typeof(TextServer.Direction),
                    typeof(TextServer.Orientation),
                    typeof(float)
                ])
            ];
        }

        [HarmonyBefore(Const.PinyinEverythingPatcherId, Const.ExclaimEverythingPatcherId)]
        public static void Prefix(ref string text)
        {
            text = ChineseNumberTextTransformer.Transform(text);
        }
    }
}
