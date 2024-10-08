using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using System.Text;

public static class MzU
{

    #region Timer
    public static readonly float StopValue = float.NegativeInfinity;

    public static T RandomEnumValue<T>()
    {

        var v = System.Enum.GetValues(typeof(T));
        return (T)v.GetValue(new System.Random().Next(v.Length));
    }

    /// <summary>
    /// Manual Invoke to time
    /// </summary>
    /// <param name="value">ref param to count</param>
    /// <param name="time">Time to invoke,time less than zero to alway run</param>
    /// <returns></returns>
    public static int ManualInvoke(ref float value, float time)
    {
        //value = 0;
        //		Debug.Log (value);
        if (time < 0)
            return 1;
        if (value == StopValue)
            return -1;
        value += Time.deltaTime;
        if (value >= time)
        {
            value = StopValue;
            return 1;
        }
        return 0;
    }

    public static int ManualCountdown(ref float value, float manualTime = 0)
    {

        if (value == StopValue)
            return -1;
        if (manualTime <= 0)
            value -= Time.deltaTime;
        else
            value -= 0.1f;
        if (value <= 0)
        {
            value = StopValue;
            return 1;
        }
        return 0;
    }

    [System.Serializable]
    public class ValueObject
    {
        public float min, max;
        public ValueObject(float min)
        {
            this.min = min;
            this.max = min;
        }
        public ValueObject(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// get random value from min and max
        /// </summary>
        /// <returns></returns>
        public float GetValue()
        {
            if (min == max)
                return min;
            else
                return UnityEngine.Random.Range(min, max);
        }
    }
    public class ValueIntObject
    {
        public int min, max;
        public ValueIntObject(int min)
        {
            this.min = min;
            this.max = min;
        }
        public ValueIntObject(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// get random value from min and max
        /// </summary>
        /// <returns></returns>
        public int GetValue()
        {
            if (min == max)
                return min;
            else
                return UnityEngine.Random.Range(min, max);
        }
    }
    #endregion
    #region Convert
    public static bool TryGetCurrencySymbol(string ISOCurrencySymbol, out string symbol)
    {
        symbol = CultureInfo
            .GetCultures(CultureTypes.AllCultures)
            .Where(c => !c.IsNeutralCulture)
            .Select(culture =>
            {
                try
                {
                    return new RegionInfo(culture.LCID);
                }
                catch
                {
                    return null;
                }
            })
            .Where(ri => ri != null && ri.ISOCurrencySymbol == ISOCurrencySymbol)
            .Select(ri => ri.CurrencySymbol)
            .FirstOrDefault();
        Debug.Log(ISOCurrencySymbol + " goto " + symbol);
        return symbol != null;
    }

    /// <summary>
    /// Move UI to Game Object's position
    /// </summary>
    /// <param name="rect">UI object</param>
    /// <param name="_gameObject">Game object target</param>
    public static void UIFollowGameobject(RectTransform rect, GameObject _gameObject)
    {
        Vector2 pos = _gameObject.transform.position;  // get the game object position
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(pos);  //convert game object position to VievportPoint

        // set MIN and MAX Anchor values(positions) to the same position (ViewportPoint)
        rect.anchorMin = viewportPoint;
        rect.anchorMax = viewportPoint;
    }
    public static void UIFollowPosition(RectTransform rect, Vector3 pos)
    {
        //  Vector2 pos = _gameObject.transform.position;  // get the game object position
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(pos);  //convert game object position to VievportPoint

        // set MIN and MAX Anchor values(positions) to the same position (ViewportPoint)
        rect.anchorMin = viewportPoint;
        rect.anchorMax = viewportPoint;
    }
    public static Sprite TextureToSpriteFull(Texture2D text)
    {
        return Sprite.Create(text, new Rect(0, 0, text.width, text.height), Vector2.zero);
    }
    #endregion

    public static int PefectRandom(int min, int max, int me)
    {
        int temp = UnityEngine.Random.Range(min, max);
        if (temp == me)
            return PefectRandom(min, max, me);
        else
            return temp;

    }
    public static bool isContain(int[] input, int target)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == target)
                return true;
        }
        return false;
    }

    #region Suffle
    private static System.Random rng = new System.Random();

    /// <summary>
    /// List<Product> products = GetProducts();
    ///products.Shuffle();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    public static int[] MakeRandomListWithSum(int num, int sum)
    {
        int[] _output = new int[num];
        float[] outPut = new float[num];
        float _sum = 0;
        for (int i = 0; i < num; i++)
        {
            outPut[i] = rng.Next(1, sum);
            _sum += outPut[i];
        }
        _sum /= (float)sum;
        for (int i = 0; i < num; i++)
        {
            outPut[i] /= _sum;
            _output[i] = (int)outPut[i];
            if (_output[i] <= 0)
                _output[i] = 1;
        }
        return _output;
    }

    public static int[] MakeRandomListWithSum2(int num, int sum, int randomRange)
    {
        int[] _output = new int[num];
        int _temp = sum / num;
        if (randomRange < 0)
        {
            randomRange = _temp / 2;
        }
        for (int i = 0; i < num; i++)
        {
            _output[i] = _temp + rng.Next(-randomRange, randomRange);
        }
        return _output;
    }
    #endregion
    //#region Tween
    //public static void TweenUIValue(Text UI_text, float startValue, float endValue, float time)
    //{

    //}
    //#endregion
    //public static Bounds OrthographicBounds(this Camera camera)
    //{
    //    float screenAspect = (float)Screen.width / (float)Screen.height;
    //    float cameraHeight = camera.orthographicSize * 2;
    //    Bounds bounds = new Bounds(
    //        camera.transform.position,
    //        new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
    //    return bounds;
    //}
    #region Tween
    public static Tween TweenUI(Text ui_target, string[] targets, float time)
    {
        int step = 0;
        return DOTween.To(() => step, (x) =>
             {
                 step = x;
                 ui_target.text = targets[step];

             }, targets.Length - 1, time);
    }
    #endregion

    public static void RemoveUnuseableObjects<T>(List<T> things) where T : Component
    {
        for (int i = things.Count - 1; i >= 0; i--)
        {
            if (things[i] == null || !things[i].gameObject.activeSelf)
            {
                things.RemoveAt(i);
            }
        }
    }

    #region Byte
    // Convert an object to a byte array
    public static byte[] ObjectToByteArray(System.Object obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    // Convert a byte array to an Object
    public static System.Object ByteArrayToObject(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        System.Object obj = (System.Object)binForm.Deserialize(memStream);

        return obj;
    }
    #endregion

    public static int RandomExcept(int start, int end, int except)
    {
        int _temp = UnityEngine.Random.Range(start, end);
        if (_temp == except)
            return RandomExcept(start, end, except);
        else
            return _temp;
    }
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[rng.Next(s.Length)]).ToArray());
    }

    public static List<string> PlayersName = new List<string>();

    static string MakeNameCorrect(string name)
    {
        if (name.Length < 3)
        {
            int _newLength = UnityEngine.Random.Range(0, 15 - name.Length);
            for (int i = 0; i < _newLength; i++)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                    name += RandomString(1);
                else
                    name += RandomString(1).ToLower();
            }
        }
        if (name.Length > 15)
        {
            int _newLength = UnityEngine.Random.Range(name.Length - 15, name.Length - 3);
            for (int i = 0; i < _newLength; i++)
                name.Remove(UnityEngine.Random.Range(0, name.Length));
        }
        StringBuilder sb = new StringBuilder(name);
        int _newLengthChange = UnityEngine.Random.Range(0, (int)(name.Length / 3.0f));
        for (int i = 0; i < _newLengthChange; i++)
        {
            if (UnityEngine.Random.Range(0, 3) != 0)
                continue;
            int pos = UnityEngine.Random.Range(0, name.Length);
            sb.Remove(pos, 1);
            if (UnityEngine.Random.Range(0, 3) == 0)
                sb.Insert(pos, RandomString(1));
            else
                sb.Insert(pos, RandomString(1).ToLower());
        }
        for (int i = 0; i < (int)(sb.Length / 2.0f); i++)
        {
            if (UnityEngine.Random.Range(0, 3) == 0)
            {
                char c = sb[i];
                sb.Remove(i, 1);
                sb.Insert(i, c.ToString().ToUpper());
            }
        }
        name = sb.ToString();

        return name;
    }
    public static Vector2 GetCirclePoints(Vector2 center, double radius, float angle)
    {

        float newX = (float)(center.x + radius * Mathf.Cos(angle));
        float newY = (float)(center.y + radius * Mathf.Sin(angle));
        return new Vector2(newX, newY);
    }
    public static Vector3 GetVector3WithoutZ(this Vector3 target)
    {
        return new Vector3(target.x, target.y, 0);
    }
    public static Vector3 GetVector2(this Vector3 target)
    {
        return new Vector2(target.x, target.y);
    }
    public static Vector3 GetVector3From2(this Vector2 target)
    {
        return new Vector3(target.x, target.y, 0);
    }
    public static string ConvertString_StringArray_Send(string[] input)
    {
        var o = new MemoryStream();
        var bf = new BinaryFormatter();
        bf.Serialize(o, input);
        return Convert.ToBase64String(o.GetBuffer());
    }
    public static string[] ConvertString_StringArray_Receiver(string input)
    {
        var bf = new BinaryFormatter();
        var ins = new MemoryStream(Convert.FromBase64String(input));
        string[] output = (string[])bf.Deserialize(ins);
        return output;
    }
    public static int[] StringToNumberList(this string input)
    {
        string[] _temp = input.Split(',', '/', ';');
        List<int> _out = new List<int>();
        for (int i = 0; i < _temp.Length; i++)
        {
            int _o;
            if (int.TryParse(_temp[i], out _o))
                _out.Add(_o);
        }
        return _out.ToArray();
    }

    public static long[] StringToLongList(this string input)
    {
        string[] _temp = input.Split(',', '/', ';');
        List<long> _out = new List<long>();
        for (int i = 0; i < _temp.Length; i++)
        {
            long _o;
            if (long.TryParse(_temp[i], out _o))
                _out.Add(_o);
        }
        return _out.ToArray();
    }

    public static int[] StringFloatToNumberList(this string input)
    {
        string[] _temp = input.Split(',', '/', ';');
        List<int> _out = new List<int>();
        for (int i = 0; i < _temp.Length; i++)
        {
            float _o;
            if (float.TryParse(_temp[i], out _o))
                _out.Add((int)_o);
        }
        return _out.ToArray();
    }

    public static float[] StringToFloatList(this string input)
    {
        string[] _temp = input.Split(',', '/', ';');
        List<float> _out = new List<float>();
        for (int i = 0; i < _temp.Length; i++)
        {
            float _o;
            if (float.TryParse(_temp[i], out _o))
                _out.Add(_o);
        }
        return _out.ToArray();
    }

    public static T[] StringToEnumList<T>(this string input)
    {
        string[] _temp = input.Split(',', '/', ';');
        List<T> _out = new List<T>();
        for (int i = 0; i < _temp.Length; i++)
        {
            _out.Add((T)Enum.Parse(typeof(T), _temp[i]));
        }
        return _out.ToArray();
    }

    public static T ParseEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static int[] StringToNumberList(this string input, char splitChar)
    {
        string[] _temp = input.Split(splitChar);
        List<int> _out = new List<int>();
        for (int i = 0; i < _temp.Length; i++)
        {
            int _o;
            if (int.TryParse(_temp[i], out _o))
                _out.Add(_o);
        }
        return _out.ToArray();
    }

    public static List<int> StringToListInt(this string input, char splitChar)
    {
        string[] _temp = input.Split(splitChar);
        List<int> _out = new List<int>();
        for (int i = 0; i < _temp.Length; i++)
        {
            int _o;
            if (int.TryParse(_temp[i], out _o))
                _out.Add(_o);
        }
        return _out;
    }

    //    "1:2:3:4"
    //eu = 0,
    //    /// <summary>US servers (East Coast).</summary>
    //    us = 1,
    //    /// <summary>Asian servers in Singapore.</summary>
    //    asia = 2,
    //    /// <summary>Japanese servers in Tokyo.</summary>
    //    jp = 3,
    //    /// <summary>Australian servers in Melbourne.</summary>
    //    au = 5,
    //    ///<summary>USA West, San José, usw</summary>
    //    usw = 6,
    //    ///<summary>South America, Sao Paulo, sa</summary>
    //    sa = 7,
    //    ///<summary>Canada East, Montreal, cae</summary>
    //    cae = 8,
    //    ///<summary>South Korea, Seoul, kr</summary>
    //    kr = 9,
    //    ///<summary>India, Chennai, in</summary>
    //    @in = 10,

    //    /// <summary>No region selected.</summary>
    //    none = 4

    public static Vector3 RandomVector
    {
        get
        {
            return new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), 0);
        }
    }

    #region GameLogic
    class MakeTeamData
    {
        public int playerID = 0;
        public int trophieValue = 0;
    }
    private static List<ToHop.ToHopOutPut> Make(int k, int n)
    {
        ToHop th = new ToHop();
        List<ToHop.ToHopOutPut> _outPut = th.main(k, n);
        return _outPut;
        //     Debug.Log(_outPut.Count);

    }

    #endregion
}


[System.Serializable]
public class RandomValue
{
    public float min;
    public float max;
    public float GetValue()
    {
        return UnityEngine.Random.Range(min, max);
    }
}

public class ToHop
{
    public class ToHopOutPut
    {
        public List<int> outputInner = new List<int>();
        public override string ToString()
        {
            string _temp = "";
            foreach (int _int in outputInner)
            {
                _temp += _int + ",";
            }
            return _temp;
        }
    }
    List<ToHopOutPut> output = new List<ToHopOutPut>();
    int[] a;
    int k, n;
    int total;
    void printResult()
    { // hàm dùng để in một cấu hình ra ngoài
        ToHopOutPut _o = new ToHopOutPut();
        for (int i = 1; i <= k; i++)
        {
            _o.outputInner.Add(a[i]);
            //  Debug.Log(a[i]);
            //    cout << a[i] << " ";
        }
        output.Add(_o);
        //  Debug.Log("============:" + total);
    }
    void backtrack(int i)
    { // hàm quay lui
        for (int j = a[i - 1] + 1; j <= n - k + i; j++)
        { // xét các khả năng của j
            a[i] = j; // ghi nhận một giá trị của j
            if (i == k)
            { // nếu cấu hình đã đủ k phần tử
              // in một cấu hình ra ngoài
                total++;
                printResult();

            }
            else
            {
                backtrack(i + 1); // quay lui
            }

        }
    }
    void toHop()
    { // hàm liệt kê các tổ hợp
        if (k >= 0 && k <= n)
        {
            a[0] = 0; // khởi tạo giá trị a[0]
            backtrack(1);
        }
        else
        {
            Debug.Log("Loi: khong thoa dieu kien 0<=k<=n ");
        }

    }


    public List<ToHopOutPut> main(int k, int n)
    {
        this.k = k;
        this.n = n;
        a = new int[n];
        output = new List<ToHopOutPut>();
        toHop();
        return output;
    }


}

