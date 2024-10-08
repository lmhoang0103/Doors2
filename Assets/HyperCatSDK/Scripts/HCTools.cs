#if UNITY_EDITOR

#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using AppLovinMax.Scripts.IntegrationManager.Editor;
using Newtonsoft.Json;
using UnityEditor;
using GoogleMobileAds.Editor;
using UnityEngine;
using UnityEngine.Networking;

#endregion

public class HCTools : Editor
{
    private static HCGameSetting _gameSetting;

    private static HCBuildSupport _buildSupport;

    public static HCGameSetting GameSetting
    {
        get
        {
            if (_gameSetting == null)
                _gameSetting = GetGameSetting();

            return _gameSetting;
        }
    }

    public static HCBuildSupport BuildSupport
    {
        get
        {
            if (_buildSupport == null)
                _buildSupport = GetBuildSupport();

            return _buildSupport;
        }
    }

    public static HCGameSetting GetGameSetting()
    {
        var path = "Assets/HyperCatSDK/";
        var fileEntries = Directory.GetFiles(path);
        for (var i = 0; i < fileEntries.Length; i++)
            if (fileEntries[i].EndsWith(".asset"))
            {
                var item =
                    AssetDatabase.LoadAssetAtPath<HCGameSetting>(fileEntries[i].Replace("\\", "/"));
                if (item != null)
                    return item;
            }

        return null;
    }

    public static HCBuildSupport GetBuildSupport()
    {
        var path = "Assets/HyperCatSDK/";
        var fileEntries = Directory.GetFiles(path);
        for (var i = 0; i < fileEntries.Length; i++)
            if (fileEntries[i].EndsWith(".asset"))
            {
                var item =
                    AssetDatabase.LoadAssetAtPath<HCBuildSupport>(fileEntries[i].Replace("\\", "/"));
                if (item != null)
                    return item;
            }

        return null;
    }

    public static void EditGameSetting()
    {
        Selection.activeObject = GameSetting;
        ShowInspector();
    }

    [MenuItem("HyperCat Toolkit/Documents/SDK Manual")]
    public static void OpenSdkManual()
    {
        Application.OpenURL("https://hypercatstudio.notion.site/HyperCat-SDK-Manual-bacfabce72e24ebbbe79f764e27cd489");
    }

    [MenuItem("HyperCat Toolkit/Documents/SDK Change Logs")]
    public static void OpenSdkChangeLogs()
    {
        Application.OpenURL("https://docs.google.com/spreadsheets/d/1-FJ6spGsCN_HPGcSuxyRf_s-eWqR3CTNws2XTmq9_xk");
    }

    [MenuItem("HyperCat Toolkit/Documents/Game Requirements")]
    public static void OpenGameRequirement()
    {
        Application.OpenURL("https://docs.google.com/document/d/1HBBpZSXmC4deb0iEN7TxXpe2z_kzwbeOcOUvZVprigE");
    }

    [MenuItem("HyperCat Toolkit/Documents/Applovin Adapters Status")]
    public static void OpenApplovinAdaptersStatus()
    {
        Application.OpenURL("https://hypercatstudio.notion.site/4d15ddac180048fcbe26df95d83c7079?v=5fb09f0cdc094b4a89da872a11f22f3f");
    }

    [MenuItem("HyperCat Toolkit/Documents/Submit Build Double Check Manual")]
    public static void SubmitBuildDoubleCheckManual()
    {
        Application.OpenURL("https://hypercatstudio.notion.site/Quy-tr-nh-double-check-build-aab-6291703a8a0c4648a05f47650da82428");
    }

    #region Third-party SDK

    [MenuItem("HyperCat Toolkit/Verify Ads Ids")]
    public static void VerifyAdsIds()
    {
#if UNITY_ANDROID
        //AppLovinSettings.Instance.AdMobAndroidAppId = GameSetting.admobAndroidId;
        GoogleMobileAdsSettings.LoadInstance().GoogleMobileAdsAndroidAppId = GameSetting.admobAndroidId;
        
        IronSourceEditorUtils.IronSourceMediationSettings.AndroidAppKey = GameSetting.androidAppKey;
        IronSourceEditorUtils.IronSourceMediatedNetworkSettings.AdmobAndroidAppId = GameSetting.admobAndroidId;
#elif UNITY_IOS
        //AppLovinSettings.Instance.AdMobIosAppId = GameSetting.admobIosId;
        GoogleMobileAdsSettings.LoadInstance().GoogleMobileAdsIOSAppId = GameSetting.admobIosId;

        IronSourceEditorUtils.IronSourceMediationSettings.IOSAppKey = GameSetting.iosAppKey;
        IronSourceEditorUtils.IronSourceMediatedNetworkSettings.AdmobIOSAppId = GameSetting.admobIosId;
#endif
        
        IronSourceEditorUtils.IronSourceMediatedNetworkSettings.EnableAdmob = true;
        
        EditorUtility.SetDirty(IronSourceEditorUtils.IronSourceMediationSettings);
        EditorUtility.SetDirty(IronSourceEditorUtils.IronSourceMediatedNetworkSettings);
        AssetDatabase.SaveAssets();

        HCDebug.Log("Ads id verified!", HcColor.Green);
    }

    #endregion

    #region HC UI Button

    [MenuItem("GameObject/UI/Button - HC Button", false, 2031)]
    private static void CreateHcButton(MenuCommand menuCommand)
    {
        var parent = menuCommand.context as GameObject;
        if (parent == null || parent.GetComponentsInParent<Canvas>(true).Length == 0)
        {
            Debug.LogError("Invalid parent!");
            return;
        }

        var hcButton = Instantiate(GetUiConfig().prefabButton);
        Undo.RegisterCreatedObjectUndo(hcButton, "HC Button");
        hcButton.name = "HC Button";
        SetParentAndAlign(hcButton, parent);
        Selection.activeObject = hcButton;
    }

    private static void SetParentAndAlign(GameObject child, GameObject parent)
    {
        if (parent == null)
            return;

        Undo.SetTransformParent(child.transform, parent.transform, "");

        var rectTransform = child.transform as RectTransform;
        if (rectTransform)
        {
            rectTransform.anchoredPosition = Vector2.zero;
            var localPosition = rectTransform.localPosition;
            localPosition.z = 0;
            rectTransform.localPosition = localPosition;
        }
        else
        {
            child.transform.localPosition = Vector3.zero;
        }

        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;

        SetLayerRecursively(child, parent.layer);
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        var t = go.transform;
        for (var i = 0; i < t.childCount; i++)
            SetLayerRecursively(t.GetChild(i).gameObject, layer);
    }

    #endregion

    #region Utility

    public static void SaveGameSetting()
    {
        EditorUtility.SetDirty(GameSetting);
        AssetDatabase.SaveAssets();
    }

    public static void ShowNotification(HcNotiType notiType, string content, string confirmLabel = "OK")
    {
        var title = string.Empty;
        switch (notiType)
        {
            case HcNotiType.Normal:
                title = "HyperCat SDK Notification";
                break;

            case HcNotiType.Warning:
                title = "HyperCat SDK Warning";
                break;
        }

        EditorUtility.DisplayDialog(title, content, confirmLabel);
    }

    public static void ShowInspector()
    {
        EditorApplication.ExecuteMenuItem("Window/General/Inspector");
    }

    public static void ForceResolver()
    {
        EditorApplication.ExecuteMenuItem("Assets/External Dependency Manager/Android Resolver/Force Resolver");
    }

    #endregion

    #region Build

    [MenuItem("HyperCat Toolkit/Build Android/Verify Player Setting")]
    public static void ValidatePlayerSetting()
    {
        PlayerSettings.companyName = "HyperCat Studio";
        PlayerSettings.productName = GameSetting.gameName;
        if (Application.HasProLicense())
            PlayerSettings.SplashScreen.showUnityLogo = false;
        PlayerSettings.bundleVersion = string.Format("{0}.{1}.{2}", GameSetting.gameVersion, GameSetting.bundleVersion, GameSetting.buildVersion);

#if UNITY_ANDROID || UNITY_IOS
        PlayerSettings.defaultInterfaceOrientation = GameSetting.deviceOrientation;
#endif

#if UNITY_ANDROID
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        if (!defines.Contains("FIREBASE"))
            defines += ";FIREBASE";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, GameSetting.packageName);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.bundleVersionCode = GameSetting.bundleVersion;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        PlayerSettings.Android.androidTargetDevices = AndroidTargetDevices.PhonesTabletsAndTVDevicesOnly;
#elif UNITY_IOS
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        if (!defines.Contains("FIREBASE"))
            defines += ";FIREBASE";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defines);
#endif
    }

    [MenuItem("HyperCat Toolkit/Build Android/Build APK (Testing)")]
    public static void BuildApk()
    {
        GameSetting.buildVersion += 1;
        EditorUtility.SetDirty(GameSetting);
        AssetDatabase.SaveAssets();
        VerifyAdsIds();
        ValidatePlayerSetting();
        // ForceResolver();

        PlayerSettings.Android.useCustomKeystore = false;

        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.allowDebugging = false;
#if UNITY_2021_1_OR_NEWER
        EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Disabled;
#else
        EditorUserBuildSettings.androidCreateSymbolsZip = false;
#endif
        EditorUserBuildSettings.buildAppBundle = false;

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        var buildPath = string.Empty;
        if (Application.platform == RuntimePlatform.WindowsEditor)
            buildPath = BuildSupport.windowBuildPath;
        else if (Application.platform == RuntimePlatform.OSXEditor)
            buildPath = BuildSupport.macOsBuildPath;

        var playerPath = $"{buildPath}/{GameSetting.packageName} {PlayerSettings.bundleVersion}.apk";
        BuildPipeline.BuildPlayer(GetScenePaths(), playerPath, BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("HyperCat Toolkit/Build Android/Build AAB (Submit)")]
    public static void BuildAab()
    {
        PlayerSettings.Android.useCustomKeystore = true;

#if UNITY_ANDROID
        PlayerSettings.Android.keystoreName = string.Format("{0}.keystore", GameSetting.packageName);
        PlayerSettings.Android.keyaliasName = "hypercat";
        PlayerSettings.Android.keystorePass = "123456";
        PlayerSettings.Android.keyaliasPass = "123456";
#endif

        if (string.IsNullOrEmpty(PlayerSettings.keyaliasPass))
        {
            ShowNotification(HcNotiType.Warning, "Publishing Setting - Verify your keystore setting before performing a submit build!", "Yes, sir!");
            SettingsService.OpenProjectSettings("Project/Player");
            return;
        }

        var hasGoogleServiceFile = false;
        var path = "Assets/StreamingAssets/";
        var fileEntries = Directory.GetFiles(path);
        for (var i = 0; i < fileEntries.Length; i++)
            if (fileEntries[i].Contains("google-services"))
            {
                hasGoogleServiceFile = true;
                break;
            }

        if (!hasGoogleServiceFile)
        {
            ShowNotification(HcNotiType.Warning, "google-services.json file not found. Please contact your manager to get the correct file!", "Yes, sir!");
            HCDebug.LogError("google-services.json file not found at Assets/StreamingAssets/");
            return;
        }

        GameSetting.buildVersion = 1;
        GameSetting.bundleVersion += 1;
        EditorUtility.SetDirty(GameSetting);
        AssetDatabase.SaveAssets();
        VerifyAdsIds();
        ValidatePlayerSetting();
        // ForceResolver();

        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.allowDebugging = false;
#if UNITY_2021_1_OR_NEWER
        EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Disabled;
#else
        EditorUserBuildSettings.androidCreateSymbolsZip = false;
#endif
        EditorUserBuildSettings.buildAppBundle = true;

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        var buildPath = string.Empty;
        if (Application.platform == RuntimePlatform.WindowsEditor)
            buildPath = BuildSupport.windowBuildPath;
        else if (Application.platform == RuntimePlatform.OSXEditor)
            buildPath = BuildSupport.macOsBuildPath;

        var playerPath = $"{buildPath}/{GameSetting.packageName} {PlayerSettings.bundleVersion}.aab";;
        BuildPipeline.BuildPlayer(GetScenePaths(), playerPath, BuildTarget.Android, BuildOptions.None);
    }

    private static string[] GetScenePaths()
    {
        var scenes = new string[EditorBuildSettings.scenes.Length];
        for (var i = 0; i < scenes.Length; i++)
            scenes[i] = EditorBuildSettings.scenes[i].path;

        return scenes;
    }

    #endregion

    #region Configs

    public static void ShowGameConfig()
    {
        var cfg = GetGameConfig();
        if (cfg == null)
        {
            cfg = CreateInstance<GameConfig>();
            AssetDatabase.CreateAsset(cfg, "Assets/Configs/GameConfig.asset");
            AssetDatabase.SaveAssets();
        }

        cfg = GetGameConfig();
        Selection.activeObject = cfg;
        ShowInspector();
    }

    public static GameConfig GetGameConfig()
    {
        var path = "Assets/Configs/";
        var fileEntries = Directory.GetFiles(path);
        for (var i = 0; i < fileEntries.Length; i++)
            if (fileEntries[i].EndsWith(".asset"))
            {
                var item =
                    AssetDatabase.LoadAssetAtPath<GameConfig>(fileEntries[i].Replace("\\", "/"));
                if (item != null)
                    return item;
            }

        return null;
    }

    public static void ShowSoundConfig()
    {
        var cfg = GetSoundConfig();
        if (cfg == null)
        {
            cfg = CreateInstance<SoundConfig>();
            AssetDatabase.CreateAsset(cfg, "Assets/Configs/SoundConfig.asset");
            AssetDatabase.SaveAssets();
        }

        cfg = GetSoundConfig();
        Selection.activeObject = cfg;
        ShowInspector();
    }

    public static SoundConfig GetSoundConfig()
    {
        var path = "Assets/Configs/";
        var fileEntries = Directory.GetFiles(path);
        for (var i = 0; i < fileEntries.Length; i++)
            if (fileEntries[i].EndsWith(".asset"))
            {
                var item =
                    AssetDatabase.LoadAssetAtPath<SoundConfig>(fileEntries[i].Replace("\\", "/"));
                if (item != null)
                    return item;
            }

        return null;
    }

    public static void ShowUiConfig()
    {
        var cfg = GetUiConfig();
        if (cfg == null)
        {
            cfg = CreateInstance<UiConfig>();
            AssetDatabase.CreateAsset(cfg, "Assets/Configs/UiConfig.asset");
            AssetDatabase.SaveAssets();
        }

        cfg = GetUiConfig();
        Selection.activeObject = cfg;
        ShowInspector();
    }

    public static UiConfig GetUiConfig()
    {
        var path = "Assets/Configs/";
        var fileEntries = Directory.GetFiles(path);
        for (var i = 0; i < fileEntries.Length; i++)
            if (fileEntries[i].EndsWith(".asset"))
            {
                var item =
                    AssetDatabase.LoadAssetAtPath<UiConfig>(fileEntries[i].Replace("\\", "/"));
                if (item != null)
                    return item;
            }

        return null;
    }

    public static void ShowIapConfig()
    {
        var cfg = GetIapConfig();
        if (cfg == null)
        {
            cfg = CreateInstance<IapConfig>();
            AssetDatabase.CreateAsset(cfg, "Assets/Configs/IapConfig.asset");
            AssetDatabase.SaveAssets();
        }

        cfg = GetIapConfig();
        Selection.activeObject = cfg;
        ShowInspector();
    }

    public static IapConfig GetIapConfig()
    {
        var path = "Assets/Configs/";
        var fileEntries = Directory.GetFiles(path);
        for (var i = 0; i < fileEntries.Length; i++)
            if (fileEntries[i].EndsWith(".asset"))
            {
                var item =
                    AssetDatabase.LoadAssetAtPath<IapConfig>(fileEntries[i].Replace("\\", "/"));
                if (item != null)
                    return item;
            }

        return null;
    }

    public static T GetConfig<T>(string path) where T : ScriptableObject
    {
        var fileEntries = Directory.GetFiles(path, ".", SearchOption.AllDirectories);

        foreach (var fileEntry in fileEntries)
            if (fileEntry.EndsWith(".asset"))
            {
                var item =
                    AssetDatabase.LoadAssetAtPath<T>(fileEntry.Replace("\\", "/"));
                if (item)
                    return item;
            }

        return null;
    }

    public static List<T> GetConfigs<T>(string path) where T : ScriptableObject
    {
        var fileEntries = Directory.GetFiles(path, ".", SearchOption.AllDirectories);
        var result = new List<T>();
        foreach (var fileEntry in fileEntries)
            if (fileEntry.EndsWith(".asset"))
            {
                var item = AssetDatabase.LoadAssetAtPath<T>(fileEntry.Replace("\\", "/"));

                if (item)
                    result.Add(item);
            }

        if (result.Count > 0)
            return result;

        return null;
    }

    #endregion

    #region Get Game Id from Server

    public static async void GetGameIdFromServer()
    {
        var ids = await GetText(GameSetting.packageName);

        if (ids != null)
            ApplySetting(ids);
    }

    public static async Task<GameId> GetText(string fileName)
    {
        string url;
        
#if UNITY_ANDROID
        url = "https://raw.githubusercontent.com/duyiosdev/HyperCat/main/GameIDs/Android/" + fileName + ".json";
#elif UNITY_IOS
        url = "https://raw.githubusercontent.com/duyiosdev/HyperCat/main/GameIDs/iOS/" + fileName + ".json";
#endif
        
        using (var www = UnityWebRequest.Get(url))
        {
            www.SendWebRequest();
            while (!www.isDone)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                HCDebug.LogError(www.error);
                return null;
            }

            var ids = JsonConvert.DeserializeObject<GameId>(www.downloadHandler.text);
            return ids;
        }
    }

    private static void ApplySetting(GameId ids)
    {
        if (ids.packageName != GameSetting.packageName)
        {
            HCDebug.LogError("Wrong file! Please check your file in cloud");
            ShowNotification(HcNotiType.Warning, "Wrong file! Please check your file in cloud");
            return;
        }

#if UNITY_ANDROID
        GameSetting.admobAndroidId = ids.admobId;
#elif UNITY_IOS
        GameSetting.admobIosId = ids.admobId;
#endif
        /*GameSetting.bannerAd = ids.bannerId;
        GameSetting.interAd = ids.interId;
        GameSetting.rewardedAd = ids.rewardId;*/

        GameSetting.androidAppKey = ids.ironSourceAppKey;
        GameSetting.aoaListIds = ids.aoaIds;

        SaveGameSetting();
        VerifyAdsIds();

        HCDebug.Log("Get Ids from server successfully!", HcColor.Orange);
        ShowNotification(HcNotiType.Normal, "Game ids synced successfully!");
    }

    public class GameId
    {
        public string ironSourceAppKey;
        public string admobId;
        /*public string bannerId;
        public string interId;
        public string rewardId;*/
        public List<string> aoaIds;
        public string packageName;
    }

    #endregion

    #region Check Adapter Ads

    public static async void CheckRequiredNetworks()
    {
        var requiredNetworks = await GetRequiredNetworks();
        GetMissingNetworks(requiredNetworks, missingNetworks =>
        {
            if (missingNetworks.Length > 0)
            {
                HCDebug.LogError($"The following adapter(s) are missing: {string.Join(", ", missingNetworks)}");
                ShowNotification(HcNotiType.Warning, "Adapters are missing, please check the log for more detail!");
            }

            else
            {
                HCDebug.Log("MAX adapters verified successfully!", HcColor.Orange);
                ShowNotification(HcNotiType.Normal, "MAX adapters verified successfully!");
            }
        });
    }

    public static async Task<string[]> GetRequiredNetworks()
    {
        //var url = "https://raw.githubusercontent.com/duyiosdev/HyperCat/main/MaxRequiredNetworks.json";
        var url = "https://raw.githubusercontent.com/duyiosdev/HyperCat/main/IronSourceRequiredNetworks.json";
        
        using (var www = UnityWebRequest.Get(url))
        {
            www.SendWebRequest();
            while (!www.isDone)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                HCDebug.LogError(www.error);
                return null;
            }

            var requiredNetworks = JsonConvert.DeserializeObject<string[]>(www.downloadHandler.text);
            return requiredNetworks;
        }
    }

    public static void GetMissingNetworks(string[] requiredNetworks, Action<string[]> onComplete)
    {
        #region Iron Source

        var installedIronSourceNetworks = IronSourceEditorUtils.GetMediatedNetworksName(); 
        var missingIronSourceNetworks = requiredNetworks.Where(displayName => !installedIronSourceNetworks.Contains(displayName)).ToArray();

        onComplete?.Invoke(missingIronSourceNetworks);
        
        #endregion
    }

    #endregion
}

public enum HcNotiType
{
    Normal,
    Warning
}
#endif