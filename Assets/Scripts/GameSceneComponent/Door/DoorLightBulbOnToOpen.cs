using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DoorLightBulbOnToOpen : BaseDoor, ITriggerableByPlayer, IHintable
{

    public event EventHandler OnHintShow;
    public event EventHandler OnHintHidden;

    [SerializeField] private List<LightBulb> lightBulbList;
    [SerializeField] private float checkInterval = 0.5f;

    public void Trigger(Player player)
    {
        OnHintShow?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        StartCoroutine(CheckLightBulbsCoroutine());
    }

    private IEnumerator CheckLightBulbsCoroutine()
    {
        while (true)
        {
            if (CheckAllLightBulbsOn())
            {
                OpenDoor();
                OnHintHidden?.Invoke(this, EventArgs.Empty);
                yield break;
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }
    private bool CheckAllLightBulbsOn()
    {
        foreach (var lightBulb in lightBulbList)
        {
            if (!lightBulb.HasLightOn())
            {
                return false;
            }
        }
        return true;
    }
}
