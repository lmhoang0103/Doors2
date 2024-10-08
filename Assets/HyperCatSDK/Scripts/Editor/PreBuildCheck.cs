using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

//using AppLovinMax.Scripts.IntegrationManager.Editor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PreBuildCheck : IPreprocessBuildWithReport
{
    public int callbackOrder => -1;

    public void OnPreprocessBuild(BuildReport report)
    {
        HCTools.VerifyAdsIds();
        
        if (!EditorUserBuildSettings.buildAppBundle)
            return;

        CheckAdsId();

        #region Reimport file google-services
        
        string path = "Assets/StreamingAssets";
        var fileEntries = Directory.GetFiles(path);
        bool hasFile = false;
        foreach (var fileEntry in fileEntries)
        {
            if (fileEntry.EndsWith("google-services.json"))
            {
                hasFile = true;
                bool hasPackageName = false;
                
                StreamReader r = new StreamReader(fileEntry);
                JObject p = JObject.Parse(r.ReadToEnd());
                
                foreach (var client in p["client"])
                {
                    if (PlayerSettings.applicationIdentifier ==
                        (string)client["client_info"]["android_client_info"]["package_name"])
                        hasPackageName = true;
                }

                if (!hasPackageName)
                    throw new BuildFailedException($"Your bundle ID {PlayerSettings.applicationIdentifier} " +
                                                   $"is not present in your Firebase configuration.");
                
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ImportRecursive);
            }
        }
        
        if(!hasFile)
            throw new BuildFailedException($"google-services.json file not found!");
        
        if (PlayerSettings.applicationIdentifier != HCTools.GameSetting.packageName)
            throw new BuildFailedException("Bundle ID not match with HCGameSetting!");
        
        #endregion

        CheckRequiredNetworks();
    }

    private async void CheckRequiredNetworks()
    {
        string[] requiredNetworks = await HCTools.GetRequiredNetworks();
        HCTools.GetMissingNetworks(requiredNetworks, missingNetworks =>
        {
            if (missingNetworks.Length > 0)
                throw new BuildFailedException($"The following adapter(s) are missing: {string.Join(", ", missingNetworks)}");
        });
    }
    
    async void CheckAdsId()
    {
        HCTools.GameId ids;
        ids = await HCTools.GetText(HCTools.GameSetting.packageName);

        if (ids == null)
            throw new BuildFailedException("Can not get ads id from server!");

        if (HCTools.GameSetting.androidAppKey != ids.ironSourceAppKey)
            throw new BuildFailedException("Iron Source app key not match with cloud setting!");
        
        /*if (HCTools.GameSetting.bannerAd != ids.bannerId)
            throw new BuildFailedException("Banner ID not match with cloud setting!");

        if (HCTools.GameSetting.interAd != ids.interId)
            throw new BuildFailedException("Inter ID not match with cloud setting!");

        if (HCTools.GameSetting.rewardedAd != ids.rewardId)
            throw new BuildFailedException("Reward ID not match with cloud setting!");*/

        for (var i = 0; i < HCTools.GameSetting.aoaListIds.Count; i++)
        {
            if (HCTools.GameSetting.aoaListIds[i] != ids.aoaIds[i])
                throw new BuildFailedException($"AOA ID tier {i} not match with cloud setting!");
        }
    }
}