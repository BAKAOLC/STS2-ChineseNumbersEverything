using System.Text;

namespace STS2ChineseNumbersEverything.Utils
{
    internal static class ChineseNumberConverter
    {
        private static readonly string[] Digits = ["零", "一", "二", "三", "四", "五", "六", "七", "八", "九"];
        private static readonly string[] SmallUnits = ["", "十", "百", "千"];
        private static readonly string[] LargeUnits = ["", "万", "亿", "兆"];

        public static string Convert(string integerDigits, string? fractionalDigits = null, bool isNegative = false)
        {
            ArgumentException.ThrowIfNullOrEmpty(integerDigits);

            var builder = new StringBuilder();
            if (isNegative && !IsZero(integerDigits, fractionalDigits))
            {
                builder.Append('负');
            }

            builder.Append(ConvertInteger(integerDigits));

            if (!string.IsNullOrEmpty(fractionalDigits))
            {
                builder.Append('点');
                AppendDigitsIndividually(builder, fractionalDigits);
            }

            return builder.ToString();
        }

        private static string ConvertInteger(string digits)
        {
            if (digits.Length > 1 && digits[0] == '0')
            {
                return ConvertDigitsIndividually(digits);
            }

            var firstNonZeroIndex = 0;
            while (firstNonZeroIndex < digits.Length - 1 && digits[firstNonZeroIndex] == '0')
            {
                firstNonZeroIndex++;
            }

            digits = digits[firstNonZeroIndex..];
            if (digits.Length > LargeUnits.Length * 4)
            {
                return ConvertDigitsIndividually(digits);
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
                    AppendZero(builder);
                }

                AppendFourDigitGroup(builder, groupDigits, builder.Length == 0);
                builder.Append(LargeUnits[groupPosition]);
                pendingZero = false;
            }

            return builder.Length == 0 ? Digits[0] : builder.ToString();
        }

        private static void AppendFourDigitGroup(
            StringBuilder builder,
            ReadOnlySpan<char> groupDigits,
            bool omitLeadingOne)
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
                    AppendZero(builder);
                    pendingZero = false;
                }

                var omitOne = digit == 1 &&
                              unitPosition == 1 &&
                              omitLeadingOne &&
                              IsStartOfCurrentNumber(builder);
                if (!omitOne)
                {
                    builder.Append(Digits[digit]);
                }

                builder.Append(SmallUnits[unitPosition]);
            }
        }

        private static bool IsStartOfCurrentNumber(StringBuilder builder)
        {
            return builder.Length == 0 || (builder.Length == 1 && builder[0] == '负');
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

        private static string ConvertDigitsIndividually(string digits)
        {
            var builder = new StringBuilder(digits.Length);
            AppendDigitsIndividually(builder, digits);
            return builder.ToString();
        }

        private static void AppendDigitsIndividually(StringBuilder builder, string digits)
        {
            foreach (var digit in digits)
            {
                builder.Append(Digits[digit - '0']);
            }
        }

        private static void AppendZero(StringBuilder builder)
        {
            if (builder.Length == 0 || builder[^1] != '零')
            {
                builder.Append('零');
            }
        }

        private static bool IsZero(string integerDigits, string? fractionalDigits)
        {
            return integerDigits.All(static digit => digit == '0') &&
                   (string.IsNullOrEmpty(fractionalDigits) ||
                    fractionalDigits.All(static digit => digit == '0'));
        }
    }
}
