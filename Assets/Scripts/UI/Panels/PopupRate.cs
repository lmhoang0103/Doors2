#region

using HyperCatSdk;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class PopupRate : UIPanel
{
    private int _starCount = 4;

    [SerializeField]
    private Image[] imgStar;

    [SerializeField]
    private Sprite[] sprStar;

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupRate;
    }

    public static void Show()
    {
        var newInstance = (PopupRate) GUIManager.Instance.NewPanel(UiPanelType.PopupRate);
        newInstance.OnAppear();
    }

    public override void OnAppear()
    {
        if (isInited)
            return;

        base.OnAppear();

        _starCount = 4;
        SetStar();
    }

    private void SetStar()
    {
        for (var i = 0; i < 5; i++)
            if (i <= _starCount)
                imgStar[i].sprite = sprStar[0];
            else
                imgStar[i].sprite = sprStar[1];
    }

    public void OnClickRate(int index)
    {
        HCVibrate.Haptic(HcHapticTypes.SoftImpact);
        _starCount = index;
        SetStar();
    }

    public void OnConfirmRate()
    {
        Close();

        if (_starCount < 4)
        {
            PopupNotification.Show(GameConst.FeedbackThanks);
        }
        else
        {
#if UNITY_ANDROID
            Application.OpenURL(@"https://play.google.com/store/apps/details?id=" + GameManager.Instance.gameSetting.packageName);
#elif UNITY_IOS
        if (!Device.RequestStoreReview())
        {
            Application.OpenURL(@"https://apps.apple.com/us/app/id" + GameManager.Instance.GameSetting.AppstoreID);
        }
#else
            Debug.Log("Rated in store!");
#endif
        }

        Gm.data.user.rated = true;
        Database.SaveData();
        AnalyticManager.LogEvent(AnalyticEvent.rateAction, AnalyticParam.value, "Yes");
    }

    public void OnCancelRate()
    {
        AnalyticManager.LogEvent(AnalyticEvent.rateAction, AnalyticParam.value, "No");
        Close();
    }
}