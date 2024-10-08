#if UNITY_EDITOR

#region

using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

#endregion

public class HyperCatSdkPanel : OdinMenuEditorWindow
{
    [MenuItem("HyperCat Toolkit/Toolkit Panel")]
    private static void OpenWindow()
    {
        GetWindow<HyperCatSdkPanel>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;

        tree.Add("Game Settings", HCTools.GetGameSetting());
        tree.Add("Game Config", HCTools.GetGameConfig());
        tree.Add("Sound Config", HCTools.GetSoundConfig());
        tree.Add("UI Config", HCTools.GetUiConfig());
        tree.Add("IAP Config", HCTools.GetIapConfig());
        tree.Add("Builder", HCTools.GetBuildSupport());

        return tree;
    }
}
#endif