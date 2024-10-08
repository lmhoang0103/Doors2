#region

using System;
using System.Collections;
using System.Collections.Generic;
#if !PROTOTYPE
using UnityEngine;
using UnityEngine.Purchasing;

#endif

#endregion

public partial class IAPManager : Singleton<IAPManager>
#if !PROTOTYPE
    , IStoreListener
#endif
{
#if !PROTOTYPE
    private static IStoreController _storeController; // The Unity Purchasing system.
    private static IExtensionProvider _storeExtensionProvider; // The store-specific Purchasing subsystems.

    private const float RealValueRevenue = 0.7f;

    public string GetLocalizedPrice(string productId)
    {
        if (IsInitialized())
        {
            var product = _storeController.products.WithID(productId);
            if (product == null)
                return null;
            return product.metadata.localizedPriceString;
        }

        return null;
    }

    private void Start()
    {
        if (_storeController == null)
        {
            BuildProducts();
            InitializePurchasing();
        }
    }

    private bool IsInitialized()
    {
        return _storeController != null && _storeExtensionProvider != null;
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
            return;

        StartCoroutine(Init());
    }

    public static Dictionary<string, ProductType> AllProducts = new Dictionary<string, ProductType>();

    private void BuildProducts()
    {
        AllProducts.Clear();
        foreach (var iapPack in ConfigManager.Instance.iapConfig.products)
        {
            AllProducts.Add(iapPack.GetProductId(), iapPack.canBuyOnce ? ProductType.NonConsumable : ProductType.Consumable);
        }
    }

    private IEnumerator Init()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var item in AllProducts)
            builder.AddProduct(item.Key, item.Value);

        while (!IsInitialized())
        {
            UnityPurchasing.Initialize(this, builder);
            yield return Yielders.Get(10f);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        HCDebug.Log("IAP >Initialized Success!", HcColor.Orange);

        _storeController = controller;
        _storeExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        HCDebug.Log("IAP >OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void BuyProduct(IapProductName id)
    {
        if (!GameManager.NetworkAvailable)
        {
            PopupNoInternet.Show();
            return;
        }

        var packCfg = ConfigManager.Instance.iapConfig.GetProduct(id);
        if (packCfg == null)
            return;

        if (IsInitialized())
        {
            var product = _storeController.products.WithID(packCfg.GetProductId());

            if (product != null && product.availableToPurchase)
            {
                HCDebug.Log(string.Format("IAP >Purchasing product asychronously: '{0}'", product.definition.id));
                _storeController.InitiatePurchase(product);
            }
            else
            {
                HCDebug.Log("IAP >BuyProductID: FAILED. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            HCDebug.Log("IAP >BuyProductID FAILED. Not initialized.");
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            HCDebug.Log("IAP >RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            HCDebug.Log("IAP >RestorePurchases started ...");

            var apple = _storeExtensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions(result => { HCDebug.Log("IAP >RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore."); });
        }
        else
        {
            HCDebug.Log("IAP >RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        try
        {
            OnPurchaseProductSuccess(args.purchasedProduct.definition.id);

            var gCurrency = args.purchasedProduct.metadata.isoCurrencyCode;
            var gPrice = ((float) args.purchasedProduct.metadata.localizedPrice * RealValueRevenue).ToString();
            var addparam = new Dictionary<string, string>
            {
                {AFInAppEvents.CONTENT_ID, args.purchasedProduct.definition.id},
                {AFInAppEvents.REVENUE, gPrice},
                {AFInAppEvents.CURRENCY, gCurrency}
            };

#if UNITY_ANDROID
            var googleReceipt = GooglePurchase.FromJson(args.purchasedProduct.receipt);
            HCDebug.Log("IAP >Receipt: " + googleReceipt.TransactionID + " ", HcColor.Yellow);
            AnalyticManager.Instance.AppflyerLogIapAndroid(googleReceipt.PayloadData.signature, googleReceipt.PayloadData.json, gPrice, gCurrency);
#endif

#if UNITY_IOS
        var appleReceipt = ApplePurchase.FromJson(args.purchasedProduct.receipt);
#endif

            return PurchaseProcessingResult.Complete;
        }
        catch (Exception e)
        {
            HCDebug.Log("IAP >Failed " + e.Message, HcColor.Red);
            return PurchaseProcessingResult.Pending;
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        HCDebug.Log(string.Format("IAP >OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId,
            failureReason), HcColor.Red);
    }

#endif
}

#if !PROTOTYPE

//Do not rename
public class GooglePurchase
{
    public string Payload;
    public PayloadData PayloadData;

    public string Store;
    public string TransactionID;

    public static GooglePurchase FromJson(string json)
    {
        var purchase = JsonUtility.FromJson<GooglePurchase>(json);
        purchase.PayloadData = PayloadData.FromJson(purchase.Payload);
        return purchase;
    }
}

//Do not rename
public class ApplePurchase
{
    public string Payload;
    public string Store;
    public string TransactionID;

    public static ApplePurchase FromJson(string json)
    {
        var purchase = JsonUtility.FromJson<ApplePurchase>(json);
        return purchase;
    }
}

//Do not rename
public class PayloadData
{
    public string json;
    public JsonData JsonData;

    public string signature;

    public static PayloadData FromJson(string json)
    {
        var payload = JsonUtility.FromJson<PayloadData>(json);
        payload.JsonData = JsonUtility.FromJson<JsonData>(payload.json);
        return payload;
    }
}

public class JsonData
{
    public string orderId;
    public string packageName;
    public string productId;
    public int purchaseState;
    public long purchaseTime;
    public string purchaseToken;
}
#endif