#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class IronSourceEditorUtils : Editor
{
    private static IronSourceMediationSettings ironSourceMediationSettings;
    private static IronSourceMediatedNetworkSettings ironSourceMediatedNetworkSettings;
    
    public static IronSourceMediationSettings IronSourceMediationSettings
    {
        get
        {
            if (ironSourceMediationSettings == null)
            {
                ironSourceMediationSettings = AssetDatabase.LoadAssetAtPath<IronSourceMediationSettings>(IronSourceMediationSettings.IRONSOURCE_SETTINGS_ASSET_PATH);
                if (ironSourceMediationSettings == null)
                {
                    IronSourceMediationSettings asset = CreateInstance<IronSourceMediationSettings>();
                    Directory.CreateDirectory(IronSourceConstants.IRONSOURCE_RESOURCES_PATH);
                    AssetDatabase.CreateAsset(asset, IronSourceMediationSettings.IRONSOURCE_SETTINGS_ASSET_PATH);
                    ironSourceMediationSettings = asset;
                }
            }

            return ironSourceMediationSettings;
        }
    }

    public static IronSourceMediatedNetworkSettings IronSourceMediatedNetworkSettings
    {
        get
        {
            if (ironSourceMediatedNetworkSettings == null)
            {
                ironSourceMediatedNetworkSettings = AssetDatabase.LoadAssetAtPath<IronSourceMediatedNetworkSettings>(IronSourceMediatedNetworkSettings.MEDIATION_SETTINGS_ASSET_PATH);
                if (ironSourceMediatedNetworkSettings == null)
                {
                    IronSourceMediatedNetworkSettings asset = CreateInstance<IronSourceMediatedNetworkSettings>();
                    Directory.CreateDirectory(IronSourceConstants.IRONSOURCE_RESOURCES_PATH);
                    AssetDatabase.CreateAsset(asset, IronSourceMediatedNetworkSettings.MEDIATION_SETTINGS_ASSET_PATH);
                    ironSourceMediatedNetworkSettings = asset;
                }
            }
            return ironSourceMediatedNetworkSettings;
        }
    }

    public static string[] GetMediatedNetworksName()
    {
        List<string> networks = new List<string>();
        string xmlPath = "Assets/IronSource/Editor";
        string prefix = $"{xmlPath}\\IS";
        string postfix = "AdapterDependencies.xml";
        
        var fileEntries = Directory.GetFiles(xmlPath, ".", SearchOption.AllDirectories);

        foreach (var fileEntry in fileEntries)
        {
            if (fileEntry.StartsWith(prefix) && fileEntry.EndsWith(postfix))
            {
                networks.Add(
                    fileEntry.Replace(prefix, String.Empty)
                        .Replace(postfix, String.Empty)
                );
            }
        }

        return networks.ToArray();
    }
}

#endif