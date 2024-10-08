#region

using UnityEngine;
using Sirenix.OdinInspector;

#endregion

[CreateAssetMenu(fileName = "HCBuildSupport", menuName = "HyperCat/Build Support")]
public class HCBuildSupport : ScriptableObject
{
    [FolderPath]
    public string windowBuildPath = "D:/HyperCat Build";

    [FolderPath]
    public string macOsBuildPath = "/Volumes/SSD/AndroidBuild";

#if UNITY_EDITOR
    [Button(ButtonSizes.Large, ButtonStyle.Box)]
    [InfoBox("Test build")]
    [LabelText("Build APK")]
    [GUIColor(0.4f, 0.8f, 1)]
    [PropertySpace(10, 10)]
    public static void BuildApk()
    {
        HCTools.BuildApk();
    }

    [Button(ButtonSizes.Large, ButtonStyle.Box)]
    [InfoBox("Submit build")]
    [LabelText("Build AAB")]
    [GUIColor(0.4f, 0.8f, 1)]
    [PropertySpace(10, 10)]
    public static void BuildAab()
    {
        HCTools.BuildAab();
    }
#endif
}