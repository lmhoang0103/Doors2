#region

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

public class IapProduct : ScriptableObject
{
    public IapProductName id;
    public bool canBuyOnce = true;
    public string globalPrice;
    public bool isSaleOff;
#if UNITY_EDITOR
    [ShowIf("isSaleOff")]
#endif
    public string originPrice;

    public string GetProductId()
    {
        return string.Format("{0}.{1}", GameManager.Instance.gameSetting.packageName, name.ToLower());
    }
}