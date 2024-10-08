using System.Collections.Generic;
using UnityEngine;

public class PopupChestRoom : UIPanel
{
    [SerializeField] private List<ChestItem> chestItems;
    [SerializeField] private List<GameObject> keys;
    [SerializeField] private GameObject keyContainer;
    [SerializeField] private HCButton getKeyButton;
    
    public static PopupChestRoom Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupChestRoom;
    }

    public static bool Show()
    {
        if (GameManager.Instance.data.user.KeyCount < 3)
            return false;
        
        var newInstance = (PopupChestRoom) GUIManager.Instance.NewPanel(UiPanelType.PopupChestRoom);
        Instance = newInstance;
        newInstance.OnAppear();       
        return true;
    }

    public override void OnAppear()
    {
        if (isInited)
            return;

        base.OnAppear();

        Init();
    }

    private void Init()
    {
        int rdChestIndex = Random.Range(0, chestItems.Count);

        List<int> coinRewards = Cfg.gameCfg.extraFeatureConfig.chestRoomCoinRewardValues.Clone();
        
        for (int i = 0; i < 9; i++)
        {
            if (i == rdChestIndex)
            {
                chestItems[i].InitGift(() =>
                {
                    // TODO: Claim gift logic
                    
                    Gm.data.user.KeyCount--;
                    UpdateKey();
                });
            }
            else
            {
                int coinValue = coinRewards.GetRandom();
                coinRewards.Remove(coinValue);
                
                chestItems[i].InitCoin(coinValue, () =>
                {
                    Gm.data.user.KeyCount--;
                    Gm.AddMoney(coinValue); 
                    UpdateKey();
                });
            }
        }

        UpdateKey();
    }
    
    void UpdateKey()
    {
        if (Gm.data.user.KeyCount > 0)
        {
            if (!keyContainer.activeInHierarchy)
            {
                keyContainer.SetActive(true);
                chestItems.ForEach(item =>
                {
                    if (!item.Claimed)
                        item.HideReward();
                });
                
                getKeyButton.Hide(true);
            }
            
            for (var i = 0; i < keys.Count; i++)
            {
                keys[i].SetActive(i < Gm.data.user.KeyCount);
            }
        }
        else
        {
            if (keyContainer.activeInHierarchy)
            {
                keyContainer.SetActive(false);

                if (chestItems.Find(item => !item.Claimed))
                    getKeyButton.Show();
                
                chestItems.ForEach(item =>
                {
                    if (!item.Claimed)
                        item.PeekReward();
                });
            }
        }
    }

    public void GetKey()
    {
        Gm.data.user.KeyCount += 3;
        UpdateKey();
    }

    public override void OnDisappear()
    {
        base.OnDisappear();
        Instance = null;
    }
}