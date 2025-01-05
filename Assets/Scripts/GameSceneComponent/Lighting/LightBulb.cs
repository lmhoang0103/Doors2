using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LightBulb : MonoBehaviour {
    [SerializeField] private Light lightSource;

    [SerializeField] private List<Switch> lightSwitchList;

    public event EventHandler OnLightBulbStateChanged;


    private void Awake() {
        foreach (var lightSwitch in lightSwitchList) {
            lightSwitch.OnswitchTurnOff += LightSwitch_OnswitchTurnOff;
            lightSwitch.OnSwitchTurnOn += LightSwitch_OnSwitchTurnOn;
        }
        lightSource.enabled = false;
    }

    private void Start() {
    }

    private void LightSwitch_OnSwitchTurnOn(object sender, System.EventArgs e) {
        ToggleLight();
    }

    private void LightSwitch_OnswitchTurnOff(object sender, System.EventArgs e) {
        ToggleLight();
    }


    private void ToggleLight() {
        lightSource.enabled = !lightSource.enabled;
        OnLightBulbStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool HasLightOn() {
        return lightSource.enabled;
    }
}
