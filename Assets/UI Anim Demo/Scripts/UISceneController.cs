using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISceneController : SerializedMonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [OdinSerialize] public List<IDemoPanel> demoPanels;
    
    private IDemoPanel _panel;
    
    void Start()
    {
        _panel = demoPanels[0];
    }

    public void UpdateDropdown()
    {
        _panel = demoPanels[dropdown.value];
    }

    public void ShowPanel()
    {
        _panel.DemoShow();
    }
    
    [Button]
    private void SetDropDownOptions()
    {
        dropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
        
        foreach (var demoPanel in demoPanels)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(demoPanel.GetType().ToString());
            optionDatas.Add(optionData);
        }
        
        dropdown.AddOptions(optionDatas);
    }
}
