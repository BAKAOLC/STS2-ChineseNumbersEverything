using System.Text;

namespace STS2ChineseNumbersEverything.Utils
{
    internal static class ChineseNumberConverter
    {
        private static readonly NumeralSymbols StandardSymbols = new(
            ["零", "一", "二", "三", "四", "五", "六", "七", "八", "九"],
            ["", "十", "百", "千"],
            ["", "万", "亿", "兆"],
            '点',
            '负',
            true);

        private static readonly NumeralSymbols FinancialSymbols = new(
            ["零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖"],
            ["", "拾", "佰", "仟"],
            ["", "万", "亿", "兆"],
            '点',
            '负',
            false);

        public static string Convert(
            string integerDigits,
            string? fractionalDigits = null,
            bool isNegative = false)
        {
            return Convert(integerDigits, fractionalDigits, isNegative, StandardSymbols);
        }

        public static string ConvertFinancial(
            string integerDigits,
            string? fractionalDigits = null,
            bool isNegative = false)
        {
            return Convert(integerDigits, fractionalDigits, isNegative, FinancialSymbols);
        }

        private static string Convert(
            string integerDigits,
            string? fractionalDigits,
            bool isNegative,
            NumeralSymbols symbols)
        {
            ArgumentException.ThrowIfNullOrEmpty(integerDigits);

            var builder = new StringBuilder();
            if (isNegative && !IsZero(integerDigits, fractionalDigits))
            {
                builder.Append(symbols.NegativeSign);
            }

            builder.Append(ConvertInteger(integerDigits, symbols));

            if (!string.IsNullOrEmpty(fractionalDigits))
            {
                builder.Append(symbols.DecimalPoint);
                AppendDigitsIndividually(builder, fractionalDigits, symbols);
            }

            return builder.ToString();
        }

        private static string ConvertInteger(string digits, NumeralSymbols symbols)
        {
            if (digits.Length > 1 && digits[0] == '0')
            {
                return ConvertDigitsIndividually(digits, symbols);
            }

            var firstNonZeroIndex = 0;
            while (firstNonZeroIndex < digits.Length - 1 && digits[firstNonZeroIndex] == '0')
            {
                firstNonZeroIndex++;
            }

            digits = digits[firstNonZeroIndex..];
            if (digits.Length > symbols.LargeUnits.Length * 4)
            {
                return ConvertDigitsIndividually(digits, symbols);
            }

            var groupCount = (digits.Length + 3) / 4;
            var firstGroupLength = digits.Length - (groupCount - 1) * 4;
            var builder = new StringBuilder(digits.Length * 2);
            var pendingZero = false;
            var offset = 0;

            for (var groupPosition = groupCount - 1; groupPosition >= 0; groupPosition--)
            {
                var groupLength = offset == 0 ? firstGroupLength : 4;
                var groupDigits = digits.AsSpan(offset, groupLength);
                offset += groupLength;

                var groupValue = ParseDigits(groupDigits);
                if (groupValue == 0)
                {
                    if (builder.Length > 0)
                    {
                        pendingZero = true;
                    }

                    continue;
                }

                if (builder.Length > 0 && (pendingZero || groupValue < 1000))
                {
                    AppendZero(builder, symbols);
                }

                AppendFourDigitGroup(builder, groupDigits, builder.Length == 0, symbols);
                builder.Append(symbols.LargeUnits[groupPosition]);
                pendingZero = false;
            }

            return builder.Length == 0 ? symbols.Digits[0] : builder.ToString();
        }

        private static void AppendFourDigitGroup(
            StringBuilder builder,
            ReadOnlySpan<char> groupDigits,
            bool isFirstGroup,
            NumeralSymbols symbols)
        {
            var pendingZero = false;

            for (var index = 0; index < groupDigits.Length; index++)
            {
                var digit = groupDigits[index] - '0';
                var unitPosition = groupDigits.Length - index - 1;

                if (digit == 0)
                {
                    if (builder.Length > 0)
                    {
                        pendingZero = true;
                    }

                    continue;
                }

                if (pendingZero)
                {
                    AppendZero(builder, symbols);
                    pendingZero = false;
                }

                var omitOne = symbols.OmitLeadingOneInTens &&
                              digit == 1 &&
                              unitPosition == 1 &&
                              isFirstGroup &&
                              IsStartOfCurrentNumber(builder, symbols);
                if (!omitOne)
                {
                    builder.Append(symbols.Digits[digit]);
                }

                builder.Append(symbols.SmallUnits[unitPosition]);
            }
        }

        private static bool IsStartOfCurrentNumber(StringBuilder builder, NumeralSymbols symbols)
        {
            return builder.Length == 0 ||
                   (builder.Length == 1 && builder[0] == symbols.NegativeSign);
        }

        private static int ParseDigits(ReadOnlySpan<char> digits)
        {
            var value = 0;
            foreach (var digit in digits)
            {
                value = value * 10 + digit - '0';
            }

            return value;
        }

        private static string ConvertDigitsIndividually(string digits, NumeralSymbols symbols)
        {
            var builder = new StringBuilder(digits.Length);
            AppendDigitsIndividually(builder, digits, symbols);
            return builder.ToString();
        }

        private static void AppendDigitsIndividually(
            StringBuilder builder,
            string digits,
            NumeralSymbols symbols)
        {
            foreach (var digit in digits)
            {
                builder.Append(symbols.Digits[digit - '0']);
            }
        }

        private static void AppendZero(StringBuilder builder, NumeralSymbols symbols)
        {
            if (builder.Length == 0 || builder[^1] != symbols.Digits[0][0])
            {
                builder.Append(symbols.Digits[0]);
            }
        }

        private static bool IsZero(string integerDigits, string? fractionalDigits)
        {
            return integerDigits.All(static digit => digit == '0') &&
                   (string.IsNullOrEmpty(fractionalDigits) ||
                    fractionalDigits.All(static digit => digit == '0'));
        }

        private sealed record NumeralSymbols(
            string[] Digits,
            string[] SmallUnits,
            string[] LargeUnits,
            char DecimalPoint,
            char NegativeSign,
            bool OmitLeadingOneInTens);
    }
}
