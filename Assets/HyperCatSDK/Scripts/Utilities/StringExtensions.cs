#region

using System;
using System.Globalization;

#endregion

public static class StringExtensions
{
    public static T ToEnum<T>(this string value)
    {
        try
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }
        catch (Exception)
        {
            return (T) Enum.Parse(typeof(T), "None", true);
        }
    }

    public static bool IsEnum<T>(this string value)
    {
        return !Equals(value.ToEnum<T>(), "None");
    }

    public static string GetPrefix(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        var index = value.IndexOf('_');
        if (index < 0)
            return "";

        return value.Substring(0, index);
    }

    public static string GetContent(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        var index = value.IndexOf('_');
        if (index < 0)
            return "";

        return value.Substring(index + 1, value.Length - index - 1);
    }
}