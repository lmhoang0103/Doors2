public partial class IAPManager
{
    private void OnPurchaseProductSuccess(string productId)
    {
        foreach (var packCfg in ConfigManager.Instance.iapConfig.products)
        {
            if (!packCfg.GetProductId().Equals(productId))
                continue;

            switch (packCfg.id)
            {
                case IapProductName.NoAds:
                    OnBuyNoAdsOffer();
                    break;
            }

            return;
        }
    }

    private void OnBuyNoAdsOffer()
    {
        GameManager.Instance.data.user.purchasedNoAds = true;
        Database.SaveData();
        EventGlobalManager.Instance.OnPurchaseNoAds.Dispatch();
        HCDebug.Log("IAP >No Ads Offer purchased! Remove all ads now.", HcColor.Orange);
    }
}