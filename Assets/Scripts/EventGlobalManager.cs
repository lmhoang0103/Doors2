#region

using System.Collections;
using Sigtrap.Relays;
using UnityEngine;

#endregion

//Events, do not rename
public class EventGlobalManager : Singleton<EventGlobalManager>
{
    public Relay OnChangeName = new Relay();

    public Relay OnFinishLoadScene = new Relay();

    public Relay OnGameInited = new Relay();

    public Relay OnPurchaseNoAds = new Relay();

    public Relay OnStartLoadScene = new Relay();

    public Relay OnUpdateSetting = new Relay();
    
    public Relay<bool> OnMoneyChange = new Relay<bool>();

    public Relay OnEverySecondTick = new Relay();

    public Relay<Vector3> OnCollectKey = new Relay<Vector3>();

    private void Start()
    {
        StartCoroutine(EverySecondTick());
    }

    IEnumerator EverySecondTick()
    {
        while (true)
        {
            OnEverySecondTick.Dispatch();
            yield return Yielders.Get(1);
        }
    }
}