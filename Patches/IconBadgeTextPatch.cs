using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Relics;
using STS2ChineseNumbersEverything.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2ChineseNumbersEverything.Patches
{
    public sealed class IconBadgeTextPatch : IPatchMethod
    {
        public static string PatchId => "icon_badge_text_to_chinese_numbers";
        public static bool IsCritical => false;
        public static string Description => "Convert power and relic icon badge text after it is refreshed";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new ModPatchTarget(typeof(NPower), "RefreshAmount", Type.EmptyTypes),
                new ModPatchTarget(typeof(NRelicInventoryHolder), "RefreshAmount", Type.EmptyTypes)
            ];
        }

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(Node __instance)
        {
            TransformDescendantText(__instance);
        }

        private static void TransformDescendantText(Node parent)
        {
            foreach (var child in parent.GetChildren())
            {
                switch (child)
                {
                    case MegaLabel label:
                        TransformText(label);
                        break;
                    case MegaRichTextLabel richTextLabel:
                        TransformText(richTextLabel);
                        break;
                }

                TransformDescendantText(child);
            }
        }

        private static void TransformText(MegaLabel label)
        {
            var text = ChineseNumberTextTransformer.Transform(label.Text);
            if (text == label.Text)
            {
                return;
            }

            SetTextAutoSize(label, text);
        }

        private static void TransformText(MegaRichTextLabel label)
        {
            var text = ChineseNumberTextTransformer.Transform(label.Text);
            if (text == label.Text)
            {
                return;
            }

            SetTextAutoSize(label, text);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetTextAutoSize(MegaLabel label, string text)
        {
            label.SetTextAutoSize(text);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetTextAutoSize(MegaRichTextLabel label, string text)
        {
            label.SetTextAutoSize(text);
        }
    }
}
