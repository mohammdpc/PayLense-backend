namespace PayLense.Application.Common;

public static class CardValidator
{
    public static string Normalize(string number) =>
        new string(number.Where(char.IsDigit).ToArray());

    // Luhn algorithm
    public static bool IsValidNumber(string digits)
    {
        if (digits.Length < 13 || digits.Length > 19) return false;

        int sum = 0;
        bool alternate = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i] - '0';
            if (alternate)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            alternate = !alternate;
        }
        return sum % 10 == 0;
    }

    public static bool IsExpired(int month, int year)
    {
        var firstDayOfNextMonth = new DateTime(year, month, 1).AddMonths(1);
        return firstDayOfNextMonth <= DateTime.UtcNow;
    }
}
