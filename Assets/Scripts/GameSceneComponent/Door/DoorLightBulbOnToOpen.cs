using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DoorLightBulbOnToOpen : BaseDoor, ITriggerableByPlayer, IHintable {

    public event EventHandler OnHintShow;
    public event EventHandler OnHintHidden;

    [SerializeField] private List<LightBulb> lightBulbList;

    public void Trigger(Player player) {
        OnHintShow?.Invoke(this, EventArgs.Empty);
    }

    private void Start() {
        foreach (var lightBulb in lightBulbList) {
            lightBulb.OnLightBulbStateChanged += LightBulb_OnLightBulbStateChanged;
        }
    }

    private void LightBulb_OnLightBulbStateChanged(object sender, EventArgs e) {
        if (IsAllLightBulbsOn()) {
            OpenDoor();
            OnHintHidden?.Invoke(this, EventArgs.Empty);
        }
    }


    private bool IsAllLightBulbsOn() {
        foreach (var lightBulb in lightBulbList) {
            if (!lightBulb.HasLightOn()) {
                return false;
            }
        }
        return true;
    }
}
