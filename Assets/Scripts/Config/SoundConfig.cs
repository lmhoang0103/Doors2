#region

using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#endregion

[CreateAssetMenu(menuName = "Configs/Sounds", fileName = "SoundConfig")]
public class SoundConfig : SerializedScriptableObject
{
    private Dictionary<TypeSound, AudioClip> _dictAudioClip;

    [SerializeField]
    [TableList(DrawScrollView = false, ShowPaging = true, NumberOfItemsPerPage = 25, ShowIndexLabels = true)] //(AlwaysExpanded = true)]
    public List<Bgm> listBgm = new List<Bgm>();

    [SerializeField]
    [TableList(DrawScrollView = false, ShowPaging = true, NumberOfItemsPerPage = 25, ShowIndexLabels = true)] //(AlwaysExpanded = true)]
    public List<Sfx> listSfx = new List<Sfx>();

    public float volumeBgm = 0.5f;
    public float volumeSfx = 1f;

    public void ResetCache()
    {
        if (!_dictAudioClip.CheckIsNullOrEmpty())
            _dictAudioClip.Clear();
    }

    public AudioClip GetAudio(TypeSound typeSound)
    {
        if (_dictAudioClip == null)
        {
            _dictAudioClip = new Dictionary<TypeSound, AudioClip>(listSfx.Count);
            for (var i = 0; i < listSfx.Count; i++)
                _dictAudioClip.Add(listSfx[i].typeSound, listSfx[i].Audioclip);
        }

        if (_dictAudioClip.ContainsKey(typeSound))
            return _dictAudioClip[typeSound];
        return _dictAudioClip[TypeSound.Button];
    }

#if UNITY_EDITOR
    [FolderPath]
    public string path = "Assets/Sounds";

    [Button]
    public void GenerateSfx()
    {
        listSfx.Clear();
        var fileEntries = Directory.GetFiles(path);
        var index = 0;
        for (var i = 0; i < fileEntries.Length; i++)
            if (!fileEntries[i].EndsWith(".meta"))
            {
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(fileEntries[i].Replace("\\", "/"));

                var hc = new Sfx();
                hc.Audioclip = clip;
                hc.typeSound = (TypeSound) index;
                listSfx.Add(hc);

                index++;
            }
    }
#endif
}

[Serializable]
public class Bgm
{
    public string name;
    public AudioClip track;
}

[Serializable]
public class Sfx
{
    [SerializeField]
    [TableColumnWidth(250, Resizable = false)]
    private AudioClip audioClip;

    [SerializeField]
    [TableColumnWidth(150)]
    public TypeSound typeSound;

    public AudioClip Audioclip
    {
        get => audioClip;
        set => audioClip = value;
    }
}