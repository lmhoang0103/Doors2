#region

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#endregion

[CreateAssetMenu(fileName = "IapConfig", menuName = "Configs/IAP")]
public class IapConfig : ScriptableObject
{
    [Header("All IAP Products")]
    public List<IapProduct> products;

    public IapProduct GetProduct(IapProductName id)
    {
        return products.FindLast(e => e.id.Equals(id));
    }

#if UNITY_EDITOR

    [Button(ButtonSizes.Large, ButtonStyle.Box)]
    public void GetAllProductConfigs()
    {
        products.Clear();

        var path = "Assets/Configs/IAP";
        var fileEntries = Directory.GetFiles(path);
        for (var i = 0; i < fileEntries.Length; i++)
            if (fileEntries[i].EndsWith(".asset"))
            {
                var item =
                    AssetDatabase.LoadAssetAtPath<IapProduct>(fileEntries[i].Replace("\\", "/"));

                if (item != null)
                    products.Add(item);
            }
    }

    [PropertySpace(SpaceBefore = 0, SpaceAfter = 20), PropertyOrder(-1), GUIColor(0.4f, 0.8f, 1)]
    [InfoBox("Create new IAP product")]
    [Button(ButtonSizes.Large, ButtonStyle.Box)]
    public void CreateNewProduct()
    {
        HCInappCreator.CreateNewPackage();
    }
#endif
}