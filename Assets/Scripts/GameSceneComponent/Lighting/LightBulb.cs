using System.Collections.Generic;
using UnityEngine;

public class LightBulb : MonoBehaviour
{
    [SerializeField] private Light lightSource;

    [SerializeField] private List<Switch> lightSwitchList;

    private void Awake()
    {
        foreach (var lightSwitch in lightSwitchList)
        {
            lightSwitch.OnswitchTurnOff += LightSwitch_OnswitchTurnOff;
            lightSwitch.OnSwitchTurnOn += LightSwitch_OnSwitchTurnOn;
        }
    }

    private void Start()
    {
        lightSource.enabled = false;
    }

    private void LightSwitch_OnSwitchTurnOn(object sender, System.EventArgs e)
    {
        ToggleLight();
    }

    private void LightSwitch_OnswitchTurnOff(object sender, System.EventArgs e)
    {
        ToggleLight();
    }


    private void ToggleLight()
    {
        lightSource.enabled = !lightSource.enabled;
    }

    public bool HasLightOn()
    {
        return lightSource.enabled;
    }
}
