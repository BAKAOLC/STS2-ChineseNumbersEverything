using System.Text.Json.Serialization;

namespace STS2ChineseNumbersEverything.Settings
{
    public sealed class ChineseNumberSettings
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
    }
}
