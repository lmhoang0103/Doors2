#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = System.Random;

#endregion

public static class Utils
{
    #region Vector
    
    public static Vector2 Vector3To2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }

    public static Vector3 Round(this Vector3 vector, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (var i = 0; i < decimalPlaces; i++)
            multiplier *= 10f;

        return new Vector3(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier);
    }

    public static void FlipVector3(this List<Vector3> lsV3, int flipX = 1, int flipY = 1)
    {
        for (var i = 0; i < lsV3.Count; i++)
        {
            var temp = lsV3[i];
            temp.x *= flipX;
            temp.y *= flipY;
            lsV3[i] = temp;
        }
    }

    public static void FlipVector3(this Vector3[] lsV3, int flipX = 1, int flipY = 1)
    {
        for (var i = 0; i < lsV3.Length; i++)
        {
            var temp = lsV3[i];
            temp.x *= flipX;
            temp.y *= flipY;
            lsV3[i] = temp;
        }
    }

    public static Vector2 Round(this Vector2 vector, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (var i = 0; i < decimalPlaces; i++)
            multiplier *= 10f;

        return new Vector2(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier);
    }

    public static Vector3 ScaleVector(Vector3 original, float x, float y, float z)
    {
        return new Vector3(original.x * x, original.y * y, original.z * z);
    }
    
    #endregion

    #region String

    public static string StringReplaceAt(string value, int index, char newchar)
    {
        if (value.Length <= index)
            return value;
        var sb = new StringBuilder(value);
        sb[index] = newchar;
        return sb.ToString();
    }

    public static string ToTimeFormat(this int seconds)
    {
        return ToTimeFormat((long) seconds);
    }
    
    public static string ToTimeFormatCompact(this int seconds)
    {
        return ToTimeFormatCompact((long) seconds);
    }
    
    public static string ToTimeFormat(this long seconds)
    {
        if (seconds <= 0)
            return "";

        TimeSpan t = TimeSpan.FromSeconds(seconds);

        string formatTime;
        
        if (seconds >= 3600)
        {
            formatTime = string.Format("{0:D1}h {1:D2}m",
                t.Hours,
                t.Minutes);
        }
        else if(seconds >= 60)
        {
            formatTime = string.Format("{0:D2}m {1:D2}s",
                t.Minutes,
                t.Seconds);
        }
        else
        {
            formatTime = string.Format("{0:D1}s",
                t.Seconds);
        }
        
        return formatTime;
    }
    
    public static string ToTimeFormatCompact(this long seconds)
    {
        if (seconds < 0)
            return "";

        TimeSpan t = TimeSpan.FromSeconds(seconds);

        string formatTime;
        
        if (seconds >= 3600)
        {
            formatTime = string.Format("{0:D1}:{1:D2}:{2:D2}",
                t.Hours + t.Days * 24,
                t.Minutes,
                t.Seconds);
        }
        else
        {
            formatTime = string.Format("{0:D2}:{1:D2}",
                t.Minutes,
                t.Seconds);
        }
        
        return formatTime;
    }

    #endregion

    #region Collection

    public static bool CheckArrayHasString(string[] c, string b)
    {
        var d = false;
        foreach (var child in c)
        {
            if (child != b)
                continue;
            d = true;
        }

        return d;
    }

    public static T GetRandom<T>(this ICollection<T> collection)
    {
        if (collection == null)
            return default;
        var t = UnityEngine.Random.Range(0, collection.Count);
        foreach (var element in collection)
        {
            if (t == 0)
                return element;
            t--;
        }

        return default;
    }

    public static T GetLast<T>(this T[] collection)
    {
        if (collection == null)
            return default;
        return collection[collection.Length - 1];
    }

    public static T GetLast<T>(this List<T> collection)
    {
        if (collection == null)
            return default;
        return collection[collection.Count - 1];
    }

    public static bool CheckHaveItemNull<T>(this List<T> collection)
    {
        foreach (var element in collection)
            if (element == null)
                return true;

        return false;
    }

    public static bool CheckHaveItemNull<T>(this T[] collection)
    {
        foreach (var element in collection)
            if (element == null)
                return true;

        return false;
    }

    public static bool CheckHaveItemNull<T, TZ>(this Dictionary<T, TZ> collection)
    {
        foreach (var element in collection)
            if (element.Value == null)
                return true;

        return false;
    }

    public static bool CheckIsNullOrEmpty<T>(this T[] collection)
    {
        if (collection == null || collection.Length == 0)
            return true;
        return false;
    }

    public static bool CheckIsNullOrEmpty<T>(this List<T> collection)
    {
        if (collection == null || collection.Count == 0)
            return true;
        return false;
    }

    public static bool CheckIsNullOrEmpty<T, TZ>(this Dictionary<T, TZ> collection)
    {
        if (collection == null || collection.Count == 0)
            return true;
        return false;
    }

    public static T GetRandom<T>(this List<T> collection)
    {
        if (collection == null)
            return default;
        var t = UnityEngine.Random.Range(0, collection.Count);
        return collection[t];
    }

    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).SingleOrDefault();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }

    public static IList<T> Shuffle<T>(this IList<T> list, Random random)
    {
        if (random == null)
            random = new Random(DateTime.Now.Millisecond);
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = random.Next(n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    public static IList<T> PickRandom<T>(this IList<T> source, int count, Random random = null)
    {
        return source.Shuffle(random).Take(count).ToList();
    }

    public static T PickRandom<T>(this IList<T> source, Random random)
    {
        return source.PickRandom(1, random).SingleOrDefault();
    }

    public static int GetMaxElementCount<T>(
        this IEnumerable<T> source,
        int numberElementDesireToGet
    )
    {
        if (source.Count() < numberElementDesireToGet)
            return source.Count();

        return numberElementDesireToGet;
    }

    public static bool Compare<T>(this List<T> list1, List<T> list2)
    {
        if (list1.Count != list2.Count)
            return false;

        for (var i = 0; i < list1.Count; i++)
            if (!list1[i].Equals(list2[i]))
                return false;

        var firstNotSecond = list1.Except(list2).ToList();
        var secondNotFirst = list2.Except(list1).ToList();
        return !firstNotSecond.Any() && !secondNotFirst.Any();
    }

    public static bool Compare<T>(this T[] list1, T[] list2)
    {
        if (list1.Length != list2.Length)
            return false;

        for (var i = 0; i < list1.Length; i++)
            if (!list1[i].Equals(list2[i]))
                return false;

        var firstNotSecond = list1.Except(list2).ToList();
        var secondNotFirst = list2.Except(list1).ToList();
        return !firstNotSecond.Any() && !secondNotFirst.Any();
    }
    
    #endregion
    
    #region UI

    public static void ChangeText(this Text text, int start, int end, float duration)
    {
        DOTween.defaultTimeScaleIndependent = true;
        DOTween.To(x => text.text = ((int) x).ToString(), start, end, duration);
    }

    public static void ChangeText(this Text text, long start, long end, float duration, string format)
    {
        DOTween.defaultTimeScaleIndependent = true;
        DOTween.To(x => text.text = string.Format(format, (int) x), start, end, duration);
    }

    public static void ChangeText(this TMP_Text text, int start, int end, float duration)
    {
        DOTween.defaultTimeScaleIndependent = true;
        DOTween.To(x => text.text = ((int) x).ToString(), start, end, duration);
    }

    public static void ChangeText(this TMP_Text text, float start, float end, float duration, string format)
    {
        DOTween.defaultTimeScaleIndependent = true;
        DOTween.To(x => text.text = string.Format(format, x.ToString()), start, end, duration);
    }

    public static void ChangeText(this TMP_Text text, long start, long end, float duration, string format)
    {
        DOTween.defaultTimeScaleIndependent = true;
        DOTween.To(x => text.text = string.Format(format, (int) x), start, end, duration);
    }

    public static void ChangeImageFill(this Image image, float start, float end, float duration)
    {
        DOTween.To(x => image.fillAmount = x, start, end, duration);
    }

    public static void ChangeImageWidth(this RectTransform image, float end, float duration)
    {
        image.DOSizeDelta(new Vector2(end, image.sizeDelta.y), duration);
    }

    public static bool HasElementAtMousePos()
    {
        var eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        raycastResults.RemoveAll(x => x.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"));
        return raycastResults.Count > 0;
    }

    public static void SnapTo(this ScrollRect scroller, RectTransform child, float duration, float delay = 0, Action onComplete = null)
    {
        Vector2 contentPos = (Vector2)scroller.transform.InverseTransformPoint(scroller.content.position);
        Vector2 childPos = (Vector2)scroller.transform.InverseTransformPoint(child.position);
        Vector2 endPos = contentPos - childPos;

        // If no horizontal scroll, then don't change contentPos.x
        if (!scroller.horizontal) endPos.x = contentPos.x;
        // If no vertical scroll, then don't change contentPos.y
        if (!scroller.vertical) endPos.y = contentPos.y;
        if (duration <= 0)
        {
            scroller.content.anchoredPosition = endPos;
            onComplete?.Invoke();
        }
        else
        {
            scroller.content.DOAnchorPos(endPos, duration).SetDelay(delay).OnComplete(() => onComplete?.Invoke());
        }
    }
    
    #endregion

    #region Name Generator

    [Serializable]
    private class NamesList
    {
        public List<string> names;
    }

    static NamesList _namesList;
    static NamesList CurrentNamesList
    {
        get
        {
            if (_namesList == null)
            {
                TextAsset textAsset = ConfigManager.Instance.gameCfg.extraFeatureConfig.nameList;
                if (textAsset != null) 
                    _namesList = JsonUtility.FromJson<NamesList>(textAsset.text);
            }
            return _namesList;
        }
    }

    public static string GetRandomName()
    {
        return CurrentNamesList.names[UnityEngine.Random.Range(0, CurrentNamesList.names.Count)];
    }

    public static string GetNameByRank(int rank)
    {
        return CurrentNamesList.names[rank % CurrentNamesList.names.Count];
    }
    
    public static List<string> GetRandomNames(int nbNames)
    {
        if (nbNames > CurrentNamesList.names.Count)
            throw new Exception("Asking for more random names than there actually are!");
        
        NamesList copy = new NamesList();
        copy.names = new List<string>(CurrentNamesList.names);

        List<string> result = new List<string>();

        for (int i = 0; i < nbNames; i++)
        {
            int rnd = UnityEngine.Random.Range(0, copy.names.Count);
            result.Add(copy.names[rnd]);
            copy.names.RemoveAt(rnd);
        }

        return result;
    }

    #endregion
    
    #region Others

    public static IEnumerator WaitFor(Func<bool> action, float delay)
    {
        float time = 0;
        while (time <= delay)
        {
            if (action())
                time += Time.deltaTime;
            else
                time = 0;
            yield return 0;
        }
    }
    
    public static float GetRandomPercent()
    {
        return UnityEngine.Random.Range(0, 100f);
    }
    
    public static bool IsCollider(
        Vector2 posObject1,
        BoxCollider2D boxObject1,
        Vector2 posObject2,
        BoxCollider2D boxObject2
    )
    {
        float l1X, l1Y, r1X, r1Y;
        float l2X, l2Y, r2X, r2Y;

        var bounds1 = boxObject1.bounds;
        l1X = posObject1.x - bounds1.size.x / 2;
        l1Y = posObject1.y + bounds1.size.y / 2;
        r1X = posObject1.x + bounds1.size.x / 2;
        r1Y = posObject1.y - bounds1.size.y / 2;

        var bounds = boxObject2.bounds;
        l2X = posObject2.x - bounds.size.x / 2;
        l2Y = posObject2.y + bounds.size.y / 2;
        r2X = posObject2.x + bounds.size.x / 2;
        r2Y = posObject2.y - bounds.size.y / 2;

        if (l1X > r2X || l2X > r1X)
            return false;

        // If one rectangle is above other  
        if (l1Y < r2Y || l2Y < r1Y)
            return false;

        return true;
    }

    public static string ToCountFormat(int value, int digits)
    {
        var s = value.ToString();
        var originLength = s.Length;
        if (originLength >= digits)
            return s;

        for (var i = 0; i < digits - originLength; i++)
            s = "0" + s;

        return s;
    }

    public static string ToFormatString(this long s)
    {
        return $"{s:n0}";
    }

    public static string ToFormatString(this int s)
    {
        return $"{s:n0}";
    }

    public static string ToFormatString(this double s)
    {
        return $"{s:n0}";
    }

    public static string ToQuantityString(this long s)
    {
        return $"x{s:n0}";
    }

    public static string ToQuantityString(this int s)
    {
        return $"x{s:n0}";
    }

    public static string ToQuantityString(this double s)
    {
        return $"x{s:n0}";
    }

    public static List<T> Clone<T>(this List<T> collection)
    {
        var newList = new List<T>();
        foreach (var element in collection)
            newList.Add(element);

        return newList;
    }

    public static int RaiseFlag(int value, int flagIdx)
    {
        return value | (1 << flagIdx);
    }

    public static bool CheckFlag(int value, int flagIdx) //value la database,flag_idx laf client truyen len
    {
        return (value & (1 << flagIdx)) != 0;
    }

    public static int DownFlag(int value, int flagIdx)
    {
        return value & ~(1 << flagIdx);
    }

    public static long RaiseFlagLong(long value, int flagIdx)
    {
        var flagVal = (long) 1 << flagIdx;
        return value | flagVal;
    }

    public static long DownFlagLong(long value, int flagIdx)
    {
        var flagVal = (long) 1 << flagIdx;

        return value & ~flagVal;
    }

    public static bool CheckFlagLong(long value, int flagIdx) //gia ti la database,flag_idx laf client truyen len
    {
        var flagVal = (long) 1 << flagIdx;
        var andVal = value & flagVal;
        return andVal != 0;
    }

    public static string GetRomanNumeralsForShipTier(int value)
    {
        var tierStr = string.Empty;

        switch (value)
        {
            case 0:
                tierStr = "I";
                break;

            case 1:
                tierStr = "II";
                break;

            case 2:
                tierStr = "III";
                break;

            case 3:
                tierStr = "IV";
                break;
        }

        return tierStr;
    }
    
    public static void DestroyChildren(Transform parent)
    {
        int childCount = parent.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Object.Destroy(parent.GetChild(i).gameObject);
        }
    }

    public static void DestroyChildrenImmediate(Transform parent)
    {
        int childCount = parent.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }

    
    public static void SetCurveLinear(this AnimationCurve curve) {
        for (int i = 0; i < curve.keys.Length; ++i) {
            float inTangent = 0;
            float outTangent = 0;
            bool inTangentSet = false;
            bool outTangentSet = false;
            Vector2 point1;
            Vector2 point2;
            Vector2 deltaPoint;
            Keyframe key = curve[i];
 
            if (i == 0) {
                inTangent = 0; inTangentSet = true;
            }
 
            if (i == curve.keys.Length - 1) {
                outTangent = 0; outTangentSet = true;
            }
 
            if (!inTangentSet) {
                point1.x = curve.keys[i - 1].time;
                point1.y = curve.keys[i - 1].value;
                point2.x = curve.keys[i].time;
                point2.y = curve.keys[i].value;
 
                deltaPoint = point2 - point1;
 
                inTangent = deltaPoint.y / deltaPoint.x;
            }
            if (!outTangentSet) {
                point1.x = curve.keys[i].time;
                point1.y = curve.keys[i].value;
                point2.x = curve.keys[i + 1].time;
                point2.y = curve.keys[i + 1].value;
 
                deltaPoint = point2 - point1;
 
                outTangent = deltaPoint.y / deltaPoint.x;
            }
 
            key.inTangent = inTangent;
            key.outTangent = outTangent;
            curve.MoveKey(i, key);
        }
    }

    #endregion
}

[Serializable]
public class Int2
{
    public int x;
    public int y;

    public Int2(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public Int2()
    {
        x = 0;
        y = 0;
    }
}