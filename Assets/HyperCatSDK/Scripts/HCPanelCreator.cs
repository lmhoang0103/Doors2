#if UNITY_EDITOR

#region

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

#endregion

public class HCPanelCreator : EditorWindow
{
    private const string TemplatePath = "Assets/HyperCatSDK/Scripts/Templates/Template1.txt";
    private const string EnumPath = "Assets/Scripts/UI/PanelEnums.cs";

    private const string ScriptPath = "Assets/Scripts/UI/Panels/";
    private const string PrefabPath = "Assets/Prefabs/UI/Screen And Popup/";
    private const string PopupPrefix = "Popup";
    private const string ScreenPrefix = "Screen";

    private string _className = string.Empty;

    private bool _isPopup;
    private UiConfig _uiConfig;

    private static bool IsStartedCreateScript
    {
        get => PlayerPrefs.GetInt("HCPanelCreatorIsStartedCreateScript") == 1;

        set => PlayerPrefs.SetInt("HCPanelCreatorIsStartedCreateScript", value ? 1 : 0);
    }

    private static bool IsWaitingForCreateScript
    {
        get => PlayerPrefs.GetInt("HCPanelCreatorIsWaitingForCreateScript") == 1;

        set => PlayerPrefs.SetInt("HCPanelCreatorIsWaitingForCreateScript", value ? 1 : 0);
    }

    private static bool IsDone
    {
        get => PlayerPrefs.GetInt("HCPanelCreatorIsDone") == 1;

        set => PlayerPrefs.SetInt("HCPanelCreatorIsDone", value ? 1 : 0);
    }

    #region UI Tools

    public static void CreateNewScreen()
    {
        GetWindow<HCPanelCreator>("HyperCat UI Panel Creator");
    }

    private void OnDestroy()
    {
        IsDone = false;
        IsWaitingForCreateScript = false;
        IsStartedCreateScript = false;
    }

    #endregion

    private void OnGUI()
    {
        _uiConfig = HCTools.GetUiConfig();

        if (!IsStartedCreateScript)
        {
            _className = EditorGUILayout.TextField("Class name", _className);
            _isPopup = EditorGUILayout.Toggle("Is Popup", _isPopup);
        }

        var type = Type.GetType(_className);
        if (IsWaitingForCreateScript && type != null)
            IsWaitingForCreateScript = false;

        if (!IsWaitingForCreateScript)
        {
            if (!IsStartedCreateScript)
            {
                if (GUILayout.Button("Create"))
                {
                    CorrectClassName();
                    var newType = Type.GetType(_className);
                    if (newType != null)
                    {
                        EditorUtility.DisplayDialog("HyperCat Notification", "This class is existed. Please choose other name.", "Okay");
                    }
                    else
                    {
                        OnCreateScript();
                        IsWaitingForCreateScript = true;
                        IsStartedCreateScript = true;
                    }
                }
            }
            else if (!IsDone)
            {
                OnCreatePrefab();
                IsDone = true;
                _className = string.Empty;
                if (EditorUtility.DisplayDialog("HyperCat Notification", "Panel created successfully!", "Nice!"))
                {
                    IsDone = false;
                    IsWaitingForCreateScript = false;
                    IsStartedCreateScript = false;
                    Repaint();
                    Close();
                }
            }
        }
        else
        {
            GUILayout.Label("Creating " + _className + ".\nPlease wait...");
        }
    }

    private void CorrectClassName()
    {
        if (_isPopup)
        {
            if (_className.Contains(ScreenPrefix))
                _className = _className.Replace(ScreenPrefix, PopupPrefix);
            else if (!_className.Contains(PopupPrefix))
                _className = _className.Insert(0, PopupPrefix);
        }
        else
        {
            if (_className.Contains(PopupPrefix))
                _className = _className.Replace(PopupPrefix, ScreenPrefix);
            else if (!_className.Contains(ScreenPrefix))
                _className = _className.Insert(0, ScreenPrefix);
        }
    }

    private void OnCreateScript()
    {
        AddNewEnum();
        ExportEntity();
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    private void OnCreatePrefab()
    {
        CreatePrefab();
        AddInstanceToList();
    }

    private void AddNewEnum()
    {
        var enumScript = File.ReadAllText(EnumPath);
        if (enumScript.Contains(_className))
            return;

        var index = enumScript.IndexOf("SdkPlaceHolder");

        var newEnumScript = enumScript.Insert(index, _className + ",\n    ");

        File.WriteAllText(EnumPath, newEnumScript);
    }

    private void ExportEntity()
    {
        var templateFilePath = TemplatePath;
        var scriptTemplate = File.ReadAllText(templateFilePath);

        var script = scriptTemplate.Replace("TempClassName", _className);

        Directory.CreateDirectory(ScriptPath);
        File.WriteAllText(ScriptPath + _className + ".cs", script);
    }

    private void CreatePrefab()
    {
        var obj = PrefabUtility.InstantiatePrefab(_isPopup ? _uiConfig.prefabPopup : _uiConfig.prefabScreen) as GameObject;
        PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
        obj.name = _className;
        var type = Type.GetType(_className);
        var panel = (UIPanel) obj.AddComponent(type);
        panel.panelAnimator = obj.GetComponent<PanelAnim>();
        panel.root = obj.GetComponent<Canvas>();
        var newObj = PrefabUtility.SaveAsPrefabAsset(obj, PrefabPath + _className + ".prefab");
        Selection.activeObject = newObj;
        HCTools.ShowInspector();
        DestroyImmediate(obj);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newObj;
    }

    private void AddInstanceToList()
    {
        _uiConfig.GetPanelInstances();
        EditorUtility.SetDirty(_uiConfig);
        AssetDatabase.SaveAssets();
    }
}

#endif