using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlockedByLever : BaseDoor, IHintable, ITriggerableByPlayer
{
    [SerializeField] private List<Lever> leversArray;
    private int correctLeversCount;

    public event EventHandler OnHintShow;
    public event EventHandler OnHintHidden;

    private void Awake()
    {
        correctLeversCount = 0;
        foreach (Lever lever in leversArray)
        {
            lever.OnLeverTurnOn += Lever_OnLeverTurnOn;
        }

    }

    private void Lever_OnLeverTurnOn(object sender, EventArgs e)
    {
        correctLeversCount++;
        if (correctLeversCount == leversArray.Count)
        {
            OpenDoor();
            OnHintHidden?.Invoke(this, EventArgs.Empty);

        }
    }

    public void Trigger(Player player)
    {
        OnHintShow?.Invoke(this, EventArgs.Empty);
    }
}
