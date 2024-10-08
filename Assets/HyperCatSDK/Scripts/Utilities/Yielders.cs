#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

public static class Yielders
{
    #region wait for second

    private static readonly Dictionary<float, WaitForSeconds> TimeInterval = new Dictionary<float, WaitForSeconds>(32);

    public static WaitForSeconds Get(float seconds)
    {
        seconds = (float) Math.Round(seconds, 2);
        if (!TimeInterval.ContainsKey(seconds))
            TimeInterval.Add(seconds, new WaitForSeconds(seconds));
        return TimeInterval[seconds];
    }

    #endregion

    #region wait for end of frame

    public static WaitForEndOfFrame EndOffFrame { get; } = new WaitForEndOfFrame();

    #endregion
}