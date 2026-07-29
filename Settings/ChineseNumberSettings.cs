using System.Text.Json.Serialization;

namespace STS2ChineseNumbersEverything.Settings
{
    public enum NumberDisplayStyle
    {
        Chinese,
        ChineseFinancial
    }

    public sealed class ChineseNumberSettings
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;

        [JsonPropertyName("style")] public NumberDisplayStyle Style { get; set; } = NumberDisplayStyle.Chinese;
    }
}
