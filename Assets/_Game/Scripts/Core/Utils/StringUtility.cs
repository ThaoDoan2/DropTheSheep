using System;
using System.Linq;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public enum ConvertTimeType
{
    ddHH,
    HHmmss,
    HHmm,
    mmss,
    ms,
    Hms,
}

public static class StringUtility
{
    /// <summary>
    /// Converts a long number to a standard string with thousand separators.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <returns>A string representing the number with thousand separators.</returns>
    public static string StandardNumber(long number)
    {
        if (number == 0)
            return "0";
        long tmp = number;
        string result = "";
        while (tmp > 0)
        {
            long x = tmp % 1000;
            tmp = tmp / 1000;
            string cur = x.ToString();
            if (tmp > 0)
            {
                if (x < 10) cur = "00" + x.ToString();
                else if (x < 100) cur = "0" + x.ToString();
            }
            if (result.Length > 0) result = cur + "," + result;
            else result = cur;
        }

        return result;
    }

    /// <summary>
    /// Converts a long number to a short string representation with suffix (K, M, B).
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <returns>A short string representation of the number.</returns>
    public static string NumberToShortString(long number)
    {
        if (number == 0)
            return "0";
        string currency = "";
        int value = 0;
        if (number >= 1e9)
        {
            currency = "B";
            value = (int)(number / 1e9);
        }

        else if (number >= 1e6)
        {
            currency = "M";
            value = (int)(number / 1e6);
        }

        else if (number >= 1e3)
        {
            currency = "K";
            value = (int)number / 1000;
        }
        else value = (int)number;
        return value + currency;
    }

    /// <summary>
    /// Converts a total number of seconds to a formatted time string based on the specified format type.
    /// </summary>
    /// <param name="convertTimeType">The format type for the time string.</param>
    /// <param name="totalSeconds">The total number of seconds to convert.</param>
    /// <returns>A formatted time string.</returns>
    public static string ConvertIntToTimeStr(ConvertTimeType convertTimeType, double totalSeconds)
    {
        string result = String.Empty;
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        switch (convertTimeType)
        {
            case ConvertTimeType.ddHH:
                result = string.Format("{0}d{1:D2}h",
                    time.Days,
                    time.Hours);
                break;
            case ConvertTimeType.HHmmss:
                result = string.Format("{0}h{1:D2}m{2:D2}s",
                    time.Hours,
                    time.Minutes,
                    time.Seconds);
                break;
            case ConvertTimeType.mmss:
                result = string.Format("{0}m{1:D2}s",
                    time.Minutes,
                    time.Seconds);
                break;
            case ConvertTimeType.ms:
                result = string.Format("{0}:{1:D2}",
                    time.Minutes,
                    time.Seconds);
                break;
            case ConvertTimeType.HHmm:
                result = $"{time.Hours}h{time.Minutes:D2}m";
                break;
            case ConvertTimeType.Hms:
                result = string.Format("{0}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
                break;
        }

        return result;
    }

    /// <summary>
    /// Determines the appropriate ConvertTimeType based on the total number of seconds.
    /// </summary>
    /// <param name="totalSeconds">The total number of seconds.</param>
    /// <returns>The appropriate ConvertTimeType.</returns>
    private static ConvertTimeType GetConvertTimeTypeByParam(double totalSeconds)
    {
        return totalSeconds switch
        {
            < 3600 => ConvertTimeType.ms,
            < 86400 => ConvertTimeType.Hms,
            _ => ConvertTimeType.ddHH
        };
    }

    private static ConvertTimeType GetShortTimeTypeByParam(double totalSeconds)
    {
        return totalSeconds switch
        {
            < 3600 => ConvertTimeType.ms,
            < 86400 => ConvertTimeType.HHmm,
            _ => ConvertTimeType.ddHH
        };
    }

    /// <summary>
    /// Converts a total number of seconds to a formatted time string based on inferred format type.
    /// </summary>
    /// <param name="totalSeconds">The total number of seconds to convert.</param>
    /// <returns>A formatted time string.</returns>
    public static string ConvertIntToTimeStr(double totalSeconds)
    {
        ConvertTimeType convertTimeType = GetConvertTimeTypeByParam(totalSeconds);
        return ConvertIntToTimeStr(convertTimeType, totalSeconds);
    }

    public static string ConvertIntToShortStr(double totalSeconds)
    {
        ConvertTimeType type = GetShortTimeTypeByParam(totalSeconds);
        return ConvertIntToTimeStr(type, totalSeconds);
    }

    /// <summary>
    /// Compares two version strings.
    /// </summary>
    /// <param name="v1">The first version string.</param>
    /// <param name="v2">The second version string.</param>
    /// <returns>-1 if v1 is less than v2, 1 if v1 is greater than v2, 0 if they are equal.</returns>
    public static int CompareVersion(string v1, string v2)
    {
        if (v1 == v2) return 0;
        string[] codeStrArray1 = v1.Split('.');
        string[] codeStrArray2 = v2.Split('.');

        int n = Math.Min(codeStrArray1.Length, codeStrArray2.Length);
        for (int i = 0; i < n; i++)
        {
            int code1 = int.Parse(codeStrArray1[i]);
            int code2 = int.Parse(codeStrArray2[i]);
            if (code1 > code2) return 1;
            else if (code1 < code2) return -1;
        }

        if (codeStrArray1.Length > n) return 1;
        else if (codeStrArray2.Length > n) return -1;

        return 0;
    }

    public static int MAX_NAME_LENGTH = 18;
    public static bool CheckValidName(string str)
    {
        if (str.Length > MAX_NAME_LENGTH)
            return false;
        bool specialChacracter = str.Any(ch => !(char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch)));
        if (specialChacracter)
            return false;

        return true;
    }

    public static string GetLocalizeString(string key)
    {
        var tableAsync = LocalizationSettings.StringDatabase.GetDefaultTableAsync();

        var table = tableAsync.Result;

        var entry = table.GetEntry(key);
        if (entry == null || string.IsNullOrEmpty(entry.GetLocalizedString()))
        {
            return key;
        }
        return entry.GetLocalizedString();
    }

    public static string GetLevelStr(int level)
    {
        if (level < 10) return $"000{level}";
        if (level < 100) return $"00{level}";
        if (level < 1000) return $"0{level}";
        return level.ToString();
    }

}
