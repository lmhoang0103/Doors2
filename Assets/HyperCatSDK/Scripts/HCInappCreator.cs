#if UNITY_EDITOR

#region

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

#endregion

public class HCInappCreator : EditorWindow
{
    private const string EnumPath = "Assets/Scripts/IAP/IAPEnums.cs";
    private const string ConfigPath = "Assets/Configs/IAP/";

    private string _productName = string.Empty;

    private bool _canBuyOnce;
    private bool _isSaleOff;
    private string _price;
    private string _originPrice;

    private static int _newEnumValue = 0;

    private IapConfig _iapConfig;

    #region UI Tools

    public static void CreateNewPackage()
    {
        GetWindow<HCInappCreator>("HyperCat IAP Creator");
        _newEnumValue = (int) IapProductName.Total;
    }

    #endregion

    private void OnGUI()
    {
        _iapConfig = HCTools.GetIapConfig();

        _productName = EditorGUILayout.TextField("Product name", _productName);
        if (_productName.ToEnum<IapProductName>() != IapProductName.None
            || _productName.Equals("None") || string.IsNullOrEmpty(_productName))
        {
            GUILayout.Label("Product dupplicated!");
        }
        else if (_productName.Contains(" "))
        {
            GUILayout.Label("Product name must be an enum!");
        }
        else
        {
            _canBuyOnce = EditorGUILayout.Toggle("Can only buy once", _canBuyOnce);
            _price = EditorGUILayout.TextField("USD price", _price);
            _isSaleOff = EditorGUILayout.Toggle("Sale off", _isSaleOff);
            if (_isSaleOff)
                _originPrice = EditorGUILayout.TextField("Price before sale off", _originPrice);
        }

        if (GUILayout.Button("Create"))
        {
            AddNewEnum();
            CreateScriptableObject();
            AddInstanceToList();
            if (EditorUtility.DisplayDialog("HyperCat Notification", "Product created successfully!", "Nice!"))
            {
                Repaint();
                Close();
            }
        }
    }

    private void AddNewEnum()
    {
        var enumScript = File.ReadAllText(EnumPath);
        if (enumScript.Contains(_productName))
            return;

        var index = enumScript.IndexOf("Total");

        var newEnumScript = enumScript.Insert(index, _productName + ",\n    ");

        File.WriteAllText(EnumPath, newEnumScript);
    }

    private void CreateScriptableObject()
    {
        var package = CreateInstance<IapProduct>();
        package.id = (IapProductName) _newEnumValue;
        package.globalPrice = _price;
        package.canBuyOnce = _canBuyOnce;
        package.isSaleOff = _isSaleOff;
        package.originPrice = _originPrice;
        AssetDatabase.CreateAsset(package, ConfigPath + _productName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = package;
    }

    private void AddInstanceToList()
    {
        _iapConfig.GetAllProductConfigs();
        EditorUtility.SetDirty(_iapConfig);
        AssetDatabase.SaveAssets();
    }
}

#endif