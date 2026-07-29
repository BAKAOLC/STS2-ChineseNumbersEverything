using System.Collections.Concurrent;
using System.Text;
using MegaCrit.Sts2.Core.Localization;
using STS2ChineseNumbersEverything.Settings;

namespace STS2ChineseNumbersEverything.Utils
{
    internal static class ChineseNumberTextTransformer
    {
        private static readonly ConcurrentDictionary<ConversionCacheKey, string> NumberCache = [];

        public static string Transform(string text)
        {
            if (string.IsNullOrEmpty(text) || !ChineseNumberSettingsService.Enabled)
            {
                return text;
            }

            var style = ChineseNumberSettingsService.Style;
            if (!IsChineseLocale())
            {
                return text;
            }

            return ConvertVisibleNumbers(text, style);
        }

        internal static string ConvertVisibleNumbers(string text, NumberDisplayStyle style)
        {
            StringBuilder? builder = null;
            var copyStart = 0;

            for (var index = 0; index < text.Length;)
            {
                if (text[index] == '[' && TrySkipBbCode(text, index, out var nextIndex))
                {
                    index = nextIndex;
                    continue;
                }

                if (text[index] == '{' && TrySkipFormatExpression(text, index, out nextIndex))
                {
                    index = nextIndex;
                    continue;
                }

                if (!TryReadNumber(text, index, out var token, out nextIndex))
                {
                    index++;
                    continue;
                }

                var converted = NumberCache.GetOrAdd(
                    new ConversionCacheKey(token, style),
                    static key => ConvertNumberToken(key.Token, key.Style));

                builder ??= new StringBuilder(text.Length + converted.Length);
                builder.Append(text, copyStart, index - copyStart);
                builder.Append(converted);

                copyStart = nextIndex;
                index = nextIndex;
            }

            if (builder == null)
            {
                return text;
            }

            builder.Append(text, copyStart, text.Length - copyStart);
            return builder.ToString();
        }

        private static bool IsChineseLocale()
        {
            return LocManager.Instance is { Language: "zhs" or "zht" };
        }

        private static bool TryReadNumber(
            string text,
            int index,
            out NumberToken token,
            out int nextIndex)
        {
            token = default;
            nextIndex = index;

            var isNegative = IsMinusSign(text[index]) &&
                             index + 1 < text.Length &&
                             IsDigit(text[index + 1]) &&
                             CanStartSignedNumber(text, index);
            if (!isNegative && !IsDigit(text[index]))
            {
                return false;
            }

            var cursor = index + (isNegative ? 1 : 0);
            var integerBuilder = new StringBuilder();
            while (cursor < text.Length && TryGetDigit(text[cursor], out var digit))
            {
                integerBuilder.Append(digit);
                cursor++;
            }

            if (integerBuilder.Length == 0)
            {
                return false;
            }

            if (integerBuilder.Length <= 3)
            {
                while (cursor < text.Length &&
                       IsThousandsSeparator(text[cursor]) &&
                       TryReadThousandsGroup(text, cursor + 1, integerBuilder, out var groupEnd))
                {
                    cursor = groupEnd;
                }
            }

            string? fractionalDigits = null;
            if (cursor + 1 < text.Length &&
                IsDecimalPoint(text[cursor]) &&
                IsDigit(text[cursor + 1]))
            {
                cursor++;
                var fractionalBuilder = new StringBuilder();
                while (cursor < text.Length && TryGetDigit(text[cursor], out var digit))
                {
                    fractionalBuilder.Append(digit);
                    cursor++;
                }

                fractionalDigits = fractionalBuilder.ToString();
            }

            var isPercent = cursor < text.Length && IsPercentSign(text[cursor]);
            if (isPercent)
            {
                cursor++;
            }

            token = new NumberToken(integerBuilder.ToString(), fractionalDigits, isNegative, isPercent);
            nextIndex = cursor;
            return true;
        }

        private static string ConvertNumberToken(NumberToken token, NumberDisplayStyle style)
        {
            Func<string, string?, bool, string> convert = style == NumberDisplayStyle.ChineseFinancial
                ? ChineseNumberConverter.ConvertFinancial
                : ChineseNumberConverter.Convert;

            if (!token.IsPercent)
            {
                return convert(
                    token.IntegerDigits,
                    token.FractionalDigits,
                    token.IsNegative);
            }

            var magnitude = convert(
                token.IntegerDigits,
                token.FractionalDigits,
                false);
            return token.IsNegative
                ? $"负百分之{magnitude}"
                : $"百分之{magnitude}";
        }

        private static bool CanStartSignedNumber(string text, int signIndex)
        {
            if (signIndex == 0)
            {
                return true;
            }

            var previous = text[signIndex - 1];
            return !IsDigit(previous);
        }

        private static bool TryReadThousandsGroup(
            string text,
            int index,
            StringBuilder integerBuilder,
            out int nextIndex)
        {
            nextIndex = index;
            if (index + 3 > text.Length)
            {
                return false;
            }

            Span<char> groupDigits = stackalloc char[3];
            for (var digitIndex = 0; digitIndex < groupDigits.Length; digitIndex++)
            {
                if (!TryGetDigit(text[index + digitIndex], out groupDigits[digitIndex]))
                {
                    return false;
                }
            }

            nextIndex = index + groupDigits.Length;
            if (nextIndex < text.Length && IsDigit(text[nextIndex]))
            {
                return false;
            }

            integerBuilder.Append(groupDigits);
            return true;
        }

        private static bool TrySkipFormatExpression(string text, int index, out int nextIndex)
        {
            nextIndex = index;
            var depth = 0;

            for (var cursor = index; cursor < text.Length; cursor++)
            {
                if (text[cursor] == '{')
                {
                    depth++;
                    continue;
                }

                if (text[cursor] != '}')
                {
                    continue;
                }

                depth--;
                if (depth != 0)
                {
                    continue;
                }

                nextIndex = cursor + 1;
                return true;
            }

            return false;
        }

        private static bool TrySkipBbCode(string text, int index, out int nextIndex)
        {
            if (!TrySkipBbCodeTag(text, index, out nextIndex, out var isImageOpen) || !isImageOpen)
            {
                return nextIndex > index;
            }

            var imageCloseIndex = text.IndexOf("[/img]", nextIndex, StringComparison.OrdinalIgnoreCase);
            if (imageCloseIndex < 0)
            {
                nextIndex = text.Length;
                return true;
            }

            nextIndex = imageCloseIndex + "[/img]".Length;
            return true;
        }

        private static bool TrySkipBbCodeTag(
            string text,
            int index,
            out int nextIndex,
            out bool isImageOpen)
        {
            var closeIndex = text.IndexOf(']', index + 1);
            if (closeIndex < 0)
            {
                nextIndex = index;
                isImageOpen = false;
                return false;
            }

            var tag = text.AsSpan(index + 1, closeIndex - index - 1).Trim();
            isImageOpen = tag.Equals("img".AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                          tag.StartsWith("img=".AsSpan(), StringComparison.OrdinalIgnoreCase);
            nextIndex = closeIndex + 1;
            return true;
        }

        private static bool IsDigit(char value)
        {
            return TryGetDigit(value, out _);
        }

        private static bool TryGetDigit(char value, out char digit)
        {
            if (char.IsAsciiDigit(value))
            {
                digit = value;
                return true;
            }

            if (value is >= '０' and <= '９')
            {
                digit = (char)('0' + value - '０');
                return true;
            }

            digit = default;
            return false;
        }

        private static bool IsMinusSign(char value)
        {
            return value is '-' or '−' or '－';
        }

        private static bool IsDecimalPoint(char value)
        {
            return value is '.' or '．';
        }

        private static bool IsPercentSign(char value)
        {
            return value is '%' or '％';
        }

        private static bool IsThousandsSeparator(char value)
        {
            return value is ',' or '，';
        }

        private readonly record struct NumberToken(
            string IntegerDigits,
            string? FractionalDigits,
            bool IsNegative,
            bool IsPercent);

        private readonly record struct ConversionCacheKey(
            NumberToken Token,
            NumberDisplayStyle Style);
    }
}
