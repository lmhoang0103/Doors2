#region

using UnityEngine;

#endregion

public static class HCDebug
{
    private const string PrefixNormal = "<b>[HyperCat] </b>";
    private const string PrefixWarning = "<b><color=yellow>[HyperCat] </color></b>";
    private const string PrefixError = "<b><color=red>[HyperCat] </color></b>";
    private const string Prefix = "<b>[HyperCat] </b>";

    public static void Log(object o)
    {
#if UNITY_EDITOR || PROTOTYPE
        Debug.Log(PrefixNormal + o);
#endif
    }

    public static void LogWarning(object o)
    {
#if UNITY_EDITOR || PROTOTYPE
        Debug.LogWarning(PrefixWarning + o);
#endif
    }

    public static void LogError(object o)
    {
#if UNITY_EDITOR || PROTOTYPE
        Debug.LogError(PrefixError + o);
#endif
    }

    public static void Log(object o, HcColor color)
    {
#if UNITY_EDITOR || PROTOTYPE
        Debug.Log(string.Format("{0}<color={1}>{2}</color>", Prefix, color.ToString().ToLower(), o));
#endif
    }
}

public enum HcColor
{
    White,
    Black,
    Red,
    Green,
    Blue,
    Orange,
    Violet,
    Aqua,
    Gray,
    Magenta,
    Purple,
    Yellow
}