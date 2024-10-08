#region

using System.Collections.Generic;
using UnityEngine;

#endregion

public static class GameHelper
{
    public static PanelType GetPanelType(this string key)
    {
        if (key.Contains("Screen"))
            return PanelType.Screen;
        if (key.Contains("Popup"))
            return PanelType.Popup;
        if (key.Contains("Noti"))
            return PanelType.Notification;

        return PanelType.None;
    }

    public static void SetActive(this GameObject[] list, bool on)
    {
        if (list == null || list.Length == 0)
            return;

        foreach (var gameObject in list)
            gameObject.SetActive(on);
    }

    public static void SetActive(this List<GameObject> list, bool on)
    {
        if (list == null || list.Count == 0)
            return;

        foreach (var gameObject in list)
            gameObject.SetActive(on);
    }

    public static void SetChildActive(this Transform parent, bool on, int exception = -1)
    {
        if (parent == null || parent.childCount == 0)
            return;

        for (var i = 0; i < parent.childCount; i++)
            if (i == exception)
                parent.GetChild(i).gameObject.SetActive(!on);
            else
                parent.GetChild(i).gameObject.SetActive(on);
    }
}