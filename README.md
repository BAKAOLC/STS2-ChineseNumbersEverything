# Chinese Numbers Everything

A RitsuLib-based Slay the Spire 2 mod that automatically replaces numbers in displayed Chinese text with Chinese numerals.

## Examples

- `32` becomes `三十二`.
- `-12` becomes `负十二`.
- `3.14` becomes `三点一四`.
- `32%` becomes `百分之三十二`.
- `-12%` becomes `负百分之十二`.
- Leading-zero values such as `007` become `零零七` so their information is preserved.

The conversion runs only while Simplified Chinese (`zhs`) or Traditional Chinese (`zht`) is selected. It preserves BBCode tags, inline image bodies, and complete nested localization format expressions such as `{Damage:diff()}` and `{energyPrefix:energyIcons(1)}`. Dynamic numeric values are converted after localization formatting succeeds.

When Pinyin Everything or Exclaim Everything is installed, shared display patches explicitly run in this order:

1. Chinese Numbers Everything
2. Pinyin Everything
3. Exclaim Everything

This allows text such as `32` to become `三十二` and then `sān shí èr`.

## Coverage

- All text returned by the game's `LocString` localization system.
- Dynamically assigned MegaLabel and MegaRichTextLabel content.
- Generic Godot Button, Label, and RichTextLabel content.
- LineEdit and TextEdit placeholder text without modifying user input.
- Strings drawn directly through CanvasItem.DrawString.
- Scene-authored MegaText content after the control becomes ready.
- A RitsuLib settings page with an enable switch.

Settings changes affect newly displayed text immediately. Already-created controls may require reopening the current screen.

## Requirements

- Slay the Spire 2 `0.107.1` or newer.
- RitsuLib `0.4.38` or newer.

## Build

```powershell
dotnet build .\STS2-ChineseNumbersEverything.csproj
```

This is a DLL-only mod. The build copies the DLL, manifest, and license into the configured local game mods directory.

## License

This project is licensed under the GNU Affero General Public License v3.0 or later. See [LICENSE](LICENSE).
